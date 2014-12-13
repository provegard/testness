// Copyright (C) 2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2014.

using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TestNess.Lib.Cil;
using TestNess.Lib.TestFramework;

namespace TestNess.Lib.Rule
{
    public class AvoidAssertBooleanRule : IRule
    {
        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            var calledAssertingMethods = testCase.GetCalledAssertingMethods();
            var tracker = new MethodValueTracker(testCase.TestMethod);

            // For each asserting method with >= 1 parameters:
            foreach (var cm in calledAssertingMethods)
            {
                var methodRef = cm.MethodReference;
                var parameterPurposes = testCase.Framework.GetParameterPurposes(methodRef);
                if (!IsSingleTruthCheckingMethod(methodRef, parameterPurposes))
                    continue;

                foreach (var valueGraph in tracker.ValueGraphs)
                {
                    IList<MethodValueTracker.Value> consumedValues =
                        tracker.GetConsumedValues(valueGraph, cm.Instruction).ToList();
                    if (consumedValues.Count == 0)
                        continue; // not part of value graph
                    var interestingValue = consumedValues[0];
                    var producers = UltimateProducers(interestingValue);
                    if (producers.Count > 1)
                    {
                        yield return new Violation(this, testCase, cm.Instruction, string.Format("{0}.{1} performs a boolean test on a composite boolean value",
                            cm.MethodReference.DeclaringType.Name,
                            cm.MethodReference.Name));
                    }
                }
            }
        }

        private ICollection<Instruction> UltimateProducers(MethodValueTracker.Value v)
        {
            // We track backwards and record values without parents. The producers of
            // those values are considered the ultimate producers of the given value.
            var result = new List<Instruction>();
            var values = new Queue<MethodValueTracker.Value>();
            values.Enqueue(v);
            while (values.Count > 0)
            {
                var aValue = values.Dequeue();
                if (aValue.Parents.Count == 0)
                {
                    result.Add(aValue.Producer);
                    continue;
                }
                foreach (var parent in aValue.Parents)
                {
                    values.Enqueue(parent);
                }
            }
            return result;
        }

        // TODO: Copied (almost) from LocalExpectationRule
        private static bool IsSingleTruthCheckingMethod(MethodReference method, IList<ParameterPurpose> purposes)
        {
            var reduced = method.ReduceToShortestOverload();
            return reduced.Parameters.Count == 1 && method.Parameters[0].ParameterType.FullName == "System.Boolean" &&
                   purposes[0] == ParameterPurpose.Actual;
        }

        public override string ToString()
        {
            return "a test case should not do a boolean assertion of a composite boolean value";
        }
    }
}
