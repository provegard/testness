/**
 * Copyright (C) 2011 by Per Rovegård (per@rovegard.se)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections.Generic;
using System.Linq;
using GraphBuilder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TestNess.Lib.Cil;

namespace TestNess.Lib.Rule
{
    public class LocalExpectationRule : IRule
    {
        private static readonly IList<OpCode> BinaryComparisonOpCodes = new List<OpCode> { OpCodes.Ceq, OpCodes.Cgt, OpCodes.Cgt_Un,
            OpCodes.Clt, OpCodes.Clt_Un};
        private static readonly IList<OpCode> FieldLoadOpCodes = new List<OpCode> { OpCodes.Ldfld, OpCodes.Ldflda, OpCodes.Ldsfld, OpCodes.Ldsflda }; 

        private readonly ITestFramework _framework = TestFrameworks.Instance;

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            var calledAssertingMethods = testCase.GetCalledAssertingMethods();
            // TODO: Resolve needed for Reference -> Definition .. can we get rid of it??
            var calledMethods =
                testCase.TestMethod.CalledMethods().Where(cm => calledAssertingMethods.Contains(cm.Method.Resolve()));
            var tracker = new MethodValueTracker(testCase.TestMethod);

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

                    // Handle cases like Assert.IsTrue(x == 5)
                    ExpandIfSingleTruthCheckingMethod(cm.Method, ref consumedValues, ref paramPurposes);

                    // Get the consumed values that correspond to parameters that represent
                    // (or perhaps represent) expectations.
                    var interestingConsumedValues =
                        consumedValues.Where(v => IsPerhapsExpectation(paramPurposes[consumedValues.IndexOf(v)]));

                    // See if there is at least one locally produced value among the surviving
                    // consumed values.
                    var hasAtLeastOneLocallyProducedExpectation =
                        interestingConsumedValues.Any(v => IsLocallyProduced(tracker, valueGraph, v));

                    if (hasAtLeastOneLocallyProducedExpectation) 
                        continue;
                    
                    // Generate a violation!
                    yield return new Violation(this, testCase);
                    break; // currently only one violation per test case!
                }
            }

            yield break;
        }

        private static void ExpandIfSingleTruthCheckingMethod(MethodReference method, ref IList<MethodValueTracker.Value> consumedValues,
            ref IList<ParameterPurpose> parameterPurposes)
        {
            IList<MethodValueTracker.Value> binaryComparisonOperands;
            if (IsSingleTruthCheckingMethod(method, parameterPurposes) &&
                IsProducedByBinaryComparison(consumedValues[0], out binaryComparisonOperands))
            {
                // "Expand" the parameter purposes and consumed values to represent the
                // values that were compared using a binary comparison.
                consumedValues = binaryComparisonOperands;
                parameterPurposes = binaryComparisonOperands.Select(x => ParameterPurpose.ExpectedOrActual).ToList();
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

        private static bool IsSingleTruthCheckingMethod(MethodReference method, IList<ParameterPurpose> parameterPurposes)
        {
            var reduced = method.ReduceToShortestOverload();
            return reduced.Parameters.Count == 1 && method.Parameters[0].ParameterType.FullName == "System.Boolean" &&
                   parameterPurposes[0] == ParameterPurpose.Actual;
        }

        private static bool IsPerhapsExpectation(ParameterPurpose parameterType)
        {
            return parameterType == ParameterPurpose.Expected || parameterType == ParameterPurpose.ExpectedOrActual;
        }

        private bool IsLocallyProduced(MethodValueTracker tracker, Graph<MethodValueTracker.Value> valueGraph, MethodValueTracker.Value v)
        {
            var hasForbiddenProducer = valueGraph.Walk(v).Any(HasForbiddenProducer);
            return !hasForbiddenProducer;
        }

        private bool HasForbiddenProducer(MethodValueTracker.Value value)
        {
            var producer = value.Producer;
            if (IsUnapprovedCall(producer) || IsFieldLoad(producer))
                return true;
            return false;
        }

        private static bool IsFieldLoad(Instruction i)
        {
            return FieldLoadOpCodes.Contains(i.OpCode);
        }

        private bool IsUnapprovedCall(Instruction i)
        {
            if (i.OpCode.FlowControl != FlowControl.Call)
                return false;
            var calledMethod = i.Operand as MethodReference;
            if (IsDataConversionCall(calledMethod))
                return false;
            if (_framework.IsDataAccessorMethod(calledMethod))
                return false;
            return true;
        }
        
        private static bool IsDataConversionCall(MethodReference method)
        {
            return "System.Convert".Equals(method.DeclaringType.FullName);
        }

        public override string ToString()
        {
            return "an assert expectation should be locally produced";
        }
    }
}
