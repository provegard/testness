// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using TestNess.Lib.Cil;

namespace TestNess.Lib.Rule
{
    public class NonConditionalTestCaseRule : IRule
    {
        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            var paths = testCase.GetInstructionGraph().FindInstructionPaths().ToList();
            var count = CountAssertsThatDontOccurInAllInstructionPaths(testCase, paths);
            if (count == 0 && !ContainsLoopingPaths(paths))
            {
                yield break;
            }
            yield return new Violation(this, testCase);
        }

        private static int CountAssertsThatDontOccurInAllInstructionPaths(TestCase testCase, IEnumerable<IList<Instruction>> paths)
        {
            //TODO: Copied from LocalExpectationRule, need a better abstraction for this!
            var calledAssertingMethods = testCase.GetCalledAssertingMethods();
            var calledAssertingMethodsWithInstruction =
                testCase.TestMethod.CalledMethods().Where(cm => calledAssertingMethods.Contains(cm.Method.Resolve()));

            return
                calledAssertingMethodsWithInstruction.Count(cmi => paths.Any(path => !path.Contains(cmi.Instruction)));
        }

        private static bool ContainsLoopingPaths(IEnumerable<IList<Instruction>> paths)
        {
            return paths.Any(p => p.ContainsLoop());
        }

        public override string ToString()
        {
            return "a test case should not be conditional";
        }
    }
}
