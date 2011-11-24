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

using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TestNess.Lib.Cil;

namespace TestNess.Lib.Rule
{
    public class LocalExpectationRule : IRule
    {
        private static readonly IList<OpCode> FieldLoadOpCodes = new List<OpCode> {OpCodes.Ldfld, OpCodes.Ldflda, OpCodes.Ldsfld, OpCodes.Ldsflda}; 

        private readonly ITestFramework _framework = TestFrameworks.Instance;

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            var calledAssertingMethods = testCase.GetCalledAssertingMethods();
            // TODO: Resolve needed for Reference -> Definition .. can we get rid of it??
            var calledMethods =
                testCase.TestMethod.CalledMethods().Where(cm => calledAssertingMethods.Contains(cm.Method.Resolve()));
            MethodValueTracker tracker;
            try
            {
                tracker = new MethodValueTracker(testCase.TestMethod);
            }
            catch (BranchingNotYetSupportedException)
            {
                //TODO: MethodValueTracker doesn't handle branching yet, so this ugly
                // hack must be here until we've implemented branching support.
                yield break;
            }

            // For each asserting method with >= 1 parameters:
            foreach (var cm in calledMethods.Where(cm => cm.Method.HasParameters))
            {
                //TODO: if the method is a helper, we need to "unfold" the helper
                // to get to the real asserting methods, and this will require us
                // to join value-generation graphs across method calls...
                var paramPurposes = _framework.GetParameterPurposes(cm.Method);
                if (paramPurposes == null) continue; // unknown method, rule does not apply

                var consumedValues = tracker.GetConsumedValues(cm.Instruction).ToList();
                var interestingConsumedValues =
                    consumedValues.Where(v => IsPerhapsExpectation(paramPurposes[consumedValues.IndexOf(v)]));

                var argLocality = interestingConsumedValues.Select(v => IsLocallyProduced(tracker, v));
                var hasAtLeastOneLocallyProducedExpectation = argLocality.Any(l => l);

                if (!hasAtLeastOneLocallyProducedExpectation)
                {
                    yield return new Violation(this, testCase);
                    break; // currently only one violation per test case!
                }
            }

            yield break;
        }

        private static bool IsPerhapsExpectation(ParameterPurpose parameterType)
        {
            return parameterType == ParameterPurpose.Expected || parameterType == ParameterPurpose.ExpectedOrActual;
        }

        private bool IsLocallyProduced(MethodValueTracker tracker, MethodValueTracker.Value v)
        {
            var hasForbiddenProducer = tracker.ValueGraph.Walk(v).Any(HasForbiddenProducer);
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
