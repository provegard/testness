// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphBuilder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TestNess.Lib.Cil;
using TestNess.Lib.TestFramework;

namespace TestNess.Lib.Rule
{
    public class LocalExpectationRule : IRule
    {
        private const string GetTypeFromHandleSignature =
            "System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)";
        private static readonly IList<OpCode> BinaryComparisonOpCodes = new List<OpCode> { OpCodes.Ceq, OpCodes.Cgt, OpCodes.Cgt_Un,
            OpCodes.Clt, OpCodes.Clt_Un};
        private static readonly IList<OpCode> FieldLoadOpCodes = new List<OpCode> { OpCodes.Ldfld, OpCodes.Ldflda, OpCodes.Ldsfld, OpCodes.Ldsflda };
        private static readonly IList<OpCode> StaticFieldLoadOpCodes = new List<OpCode> { OpCodes.Ldsfld, OpCodes.Ldsflda };

        private ITestFramework _framework;

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            // Store the framework so that we don't have to pass it around everywhere.
            _framework = testCase.Framework;

            var calledAssertingMethods = testCase.GetCalledAssertingMethods();
            // TODO: Resolve needed for Reference -> Definition .. can we get rid of it??
            var calledMethods =
                testCase.TestMethod.CalledMethods().Where(cm => calledAssertingMethods.Contains(cm.Method.Resolve()));
            var tracker = new MethodValueTracker(testCase.TestMethod);

            var whitelistedFields = FindWhitelistedFields(testCase.TestMethod.DeclaringType);

            // For each asserting method with >= 1 parameters:
            foreach (var cm in calledMethods.Where(cm => cm.Method.HasParameters))
            {
                //TODO: if the method is a helper, we need to "unfold" the helper
                // to get to the real asserting methods, and this will require us
                // to join value-generation graphs across method calls...
                var paramPurposes = _framework.GetParameterPurposes(cm.Method);
                if (paramPurposes == null) continue; // unknown method, rule does not apply

                foreach (var valueGraph in tracker.ValueGraphs)
                {
                    IList<MethodValueTracker.Value> consumedValues = tracker.GetConsumedValues(valueGraph, cm.Instruction).ToList();
                    if (consumedValues.Count == 0)
                        continue; // not part of value graph

                    // Build a list of arguments with the details we need to know if the rule applies.
                    var arguments = cm.Method.Parameters
                        .Select((p, index) => new ArgumentDetails { Method = cm.Method, Index = index, Purpose = paramPurposes[index], ConsumedValue = consumedValues[index] }).ToList();

                    // Handle cases like Assert.IsTrue(x == 5) by expanding arguments
                    ExpandIfSingleTruthCheckingMethod(cm.Method, ref arguments);

                    // We're only interested in arguments that represent expectations!
                    var interestingArguments = arguments.Where(a => IsPerhapsExpectation(a.Purpose)).ToList();

                    // This might happen with for example Assert.Fail("some reason").
                    if (interestingArguments.Count == 0)
                        continue;

                    // Add in the "forbidden producer", if any, for each argument. A forbidden producer is an 
                    // instruction that generates a value externally, such as a call.
                    interestingArguments = interestingArguments.Select(
                            a => { a.ForbiddenProducer = FirstForbiddenProducer(valueGraph, a.ConsumedValue, whitelistedFields); return a; }).ToList();

                    // If there is at least one locally produced argument, the rule doesn't apply.
                    if (interestingArguments.Any(IsLocallyProduced))
                        continue;

                    if (interestingArguments.All(a => a.Purpose == ParameterPurpose.ExpectedOrActual))
                    {
                        // Since we don't know exactly which parameter that represents the expectation, we
                        // just generate a single violation.
                        yield return new Violation(this, testCase, interestingArguments[0].ConsumedValue.Consumer,
                            CreateViolationMessageForUncertainCase(interestingArguments[0]));
                        continue;
                    }

                    foreach (var a in interestingArguments.Where(IsExternallyProduced))
                    {

                        // Generate a violation at the location of the forbidden producer!
                        yield return new Violation(this, testCase, a.ForbiddenProducer, CreateViolationMessage(a));
                    }
                }
            }
        }

        private IList<FieldDefinition> FindWhitelistedFields(TypeDefinition typeDefinition)
        {
            var setupMethods = typeDefinition.Methods.Where(m => _framework.IsSetupMethod(m));
            return typeDefinition.Fields.Where(f => IsAssignedInSetupMethod(f, setupMethods)).ToList();
        }

        private bool IsAssignedInSetupMethod(FieldDefinition field, IEnumerable<MethodDefinition> setupMethods)
        {
            return setupMethods.SelectMany(GetInstructions).Any(ins => StoresInField(ins, field));
        }

        private bool StoresInField(Instruction ins, FieldDefinition field)
        {
            // As a side effect, we don't allow static fields here!
            if (ins.OpCode != OpCodes.Stfld)
                return false;
            var operand = ins.Operand as FieldReference;
            return operand != null && operand.Resolve() == field;
        }

        private IEnumerable<Instruction> GetInstructions(MethodDefinition methodDef)
        {
            return methodDef.HasBody ? (IList<Instruction>) methodDef.Body.Instructions : new Instruction[0];
        }

        private string CreateViolationMessageForUncertainCase(ArgumentDetails a)
        {
            return string.Format("the expected value used by {0} should be produced locally", a.Method.Name);
        }

        private string CreateViolationMessage(ArgumentDetails a)
        {
            var sp = a.ConsumedValue.Consumer.FindSequencePoint();
            var lineInfo = sp == null ? "" : string.Format(" on line {0}", sp.StartLine);
            return string.Format(
                    "external production of (possibly) expected value (argument {0} of {1}{2})",
                    a.Index + 1, a.Method.Name, lineInfo);
        }

        private static void ExpandIfSingleTruthCheckingMethod(MethodReference method, ref List<ArgumentDetails> arguments)
        {
            IList<MethodValueTracker.Value> binaryComparisonOperands;
            if (IsSingleTruthCheckingMethod(method, arguments) &&
                IsProducedByBinaryComparison(arguments[0].ConsumedValue, out binaryComparisonOperands))
            {
                // "Expand" to arguments that represent the values that were compared using a binary comparison.
                var index = arguments[0].Index;
                arguments = binaryComparisonOperands.Select(
                        v => new ArgumentDetails { ConsumedValue = v, Purpose = ParameterPurpose.ExpectedOrActual, Index = index, Method = method }).ToList();
            }
        }

        private static bool IsProducedByBinaryComparison(MethodValueTracker.Value value, out IList<MethodValueTracker.Value> operands)
        {
            var producer = value.Producer;
            if (IsBinaryComparison(producer))
            {
                // Note: <=, => and != result in cgt, lgt and ceq, respectively, followed by 
                // ceq with one operand being 0 (so produced by ldc.i4.0). Therefore, to handle 
                // these comparisons, we need to backtrack one value in this particular case.
                MethodValueTracker.Value originalComparisonOutcome;
                if (IsProducedByDoubleCilComparison(value, out originalComparisonOutcome))
                {
                    // Backtrack to the value produced by the comparison behind ceq!
                    value = originalComparisonOutcome;
                }
                operands = value.Parents.ToList();
                return true;
            }
            operands = null;
            return false;
        }

        private static bool IsProducedByDoubleCilComparison(MethodValueTracker.Value value, out MethodValueTracker.Value originalComparisonOutcome)
        {
            // Assumption: The instruction is a binary comparison.
            originalComparisonOutcome = null;
            var parents = value.Parents.ToList();
            var producer = value.Producer;
            if (producer.OpCode != OpCodes.Ceq)
                return false; // lte/gte/neq = cgt/clt/ceq followed by ceq
            var ldc0 = parents.Where(v => v.Producer.OpCode == OpCodes.Ldc_I4_0).FirstOrDefault();
            if (ldc0 == null)
                return false; // one of the ceq operands should've been produced by ldc.i4.0
            var other = parents[1 - parents.IndexOf(ldc0)];
            if (!IsBinaryComparison(other.Producer))
                return false; // the other operand should've been produced by a binary comparison
            originalComparisonOutcome = other;
            return true; // match!
        }

        private static bool IsBinaryComparison(Instruction i)
        {
            return BinaryComparisonOpCodes.Contains(i.OpCode);
        }

        private static bool IsSingleTruthCheckingMethod(MethodReference method, List<ArgumentDetails> arguments)
        {
            var reduced = method.ReduceToShortestOverload();
            return reduced.Parameters.Count == 1 && method.Parameters[0].ParameterType.FullName == "System.Boolean" &&
                   arguments[0].Purpose == ParameterPurpose.Actual;
        }

        private static bool IsPerhapsExpectation(ParameterPurpose parameterType)
        {
            return parameterType == ParameterPurpose.Expected || parameterType == ParameterPurpose.ExpectedOrActual;
        }

        private Instruction FirstForbiddenProducer(Graph<MethodValueTracker.Value> valueGraph, MethodValueTracker.Value v, IList<FieldDefinition> whitelistedFields)
        {
            return valueGraph.Walk(v).Where(val => HasForbiddenProducer(val, whitelistedFields)).Select(value => value.Producer).FirstOrDefault();
        }

        private bool HasForbiddenProducer(MethodValueTracker.Value value, IList<FieldDefinition> whitelistedFields)
        {
            var producer = value.Producer;
            if (IsProducedByUnapprovedCall(value) || (IsFieldLoad(producer, whitelistedFields) && !IsStaticReadonlyFieldLoad(producer)))
                return true;
            return false;
        }

        private static bool IsFieldLoad(Instruction i, IList<FieldDefinition> whitelistedFields)
        {
            var fieldDef = GetFieldOperand(i);
            var isWhitelisted = fieldDef != null && whitelistedFields.Contains(fieldDef);
            return FieldLoadOpCodes.Contains(i.OpCode) && !isWhitelisted;
        }

        private static bool IsStaticReadonlyFieldLoad(Instruction i)
        {
            var fieldDef = GetFieldOperand(i);
            return StaticFieldLoadOpCodes.Contains(i.OpCode) && fieldDef != null && fieldDef.IsInitOnly;
        }

        private static FieldDefinition GetFieldOperand(Instruction i)
        {
            var fieldRef = i.Operand as FieldReference;
            return fieldRef != null ? fieldRef.Resolve() : null;
        }

        private bool IsProducedByUnapprovedCall(MethodValueTracker.Value value)
        {
            var i = value.Producer;
            if (i.OpCode.FlowControl != FlowControl.Call)
                return false;
            var calledMethod = i.Operand as MethodReference;
            if (calledMethod.IsConstructor())
                return false; // constructing an object locally is ok
            var propertyDef = TryGetPropertyDefForPropertyGetter(calledMethod);
            if (propertyDef != null && IsStaticGetOnlyProperty(propertyDef))
                return false;
            if (IsDataConversionCall(calledMethod))
                return false;
            if (_framework.IsDataAccessorMethod(calledMethod))
                return false;
            if (IsGeneratedByTypeOf(value))
                return false;
            return true;
        }

        private static bool IsStaticGetOnlyProperty(PropertyDefinition propertyDef)
        {
            if (propertyDef.HasThis)
                return false; // non-static
            return propertyDef.SetMethod == null;
        }

        private static PropertyDefinition TryGetPropertyDefForPropertyGetter(MethodReference m)
        {
            var mdef = m.Resolve();
            if (!mdef.IsSpecialName)
                return null;
            if (m.Parameters.Count > 0)
                return null;
            if (TrackerHelper.IsVoidMethod(m))
                return null;
            var name = m.Name;
            if (!name.StartsWith("get_"))
                return null;
            var propName = name.Substring(4);
            var prop = mdef.DeclaringType.Properties.FirstOrDefault(p => p.Name == propName);
            return prop;
        }

        private bool IsGeneratedByTypeOf(MethodValueTracker.Value value)
        {
            var i = value.Producer;
            var calledMethod = i.Operand as MethodReference;
            // typeof(X) generates a call to System.Type::GetTypeFromHandle, which can be manually
            // called as well. Check if its argument is placed on the stack through ldtoken, in
            // which case we assume it wasn't.
            if (!GetTypeFromHandleSignature.Equals(calledMethod.FullName))
                return false;
            var parent = value.Parents.FirstOrDefault();
            if (parent == null)
                return false;
            var parentProducer = parent.Producer;
            return parentProducer.OpCode == OpCodes.Ldtoken;
        }

        private static bool IsDataConversionCall(MethodReference method)
        {
            return "System.Convert".Equals(method.DeclaringType.FullName);
        }

        public override string ToString()
        {
            return "an assert expectation should be locally produced";
        }

        private static readonly Func<ArgumentDetails, bool> IsLocallyProduced = a => a.ForbiddenProducer == null;
        private static readonly Func<ArgumentDetails, bool> IsExternallyProduced = a => a.ForbiddenProducer != null;
        
        private class ArgumentDetails
        {
            internal ParameterPurpose Purpose;
            internal int Index;
            internal Instruction ForbiddenProducer;
            internal MethodValueTracker.Value ConsumedValue;
            internal MethodReference Method;
        }
    }
}
