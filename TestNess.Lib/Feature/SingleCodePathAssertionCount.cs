// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace TestNess.Lib.Feature
{
    public class SingleCodePathAssertionCount : CountFeature
    {
        public SingleCodePathAssertionCount(TestCase testCase, Features f) : base(testCase, f)
        {
        }

        protected override int GenerateValue(TestCase tc, Features f)
        {
            var paths = f.Get<InstructionPaths>().Value;
            return CountAssertsThatDontOccurInAllInstructionPaths(tc, paths);
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
    }
}
