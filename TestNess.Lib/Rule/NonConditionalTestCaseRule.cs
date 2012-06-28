// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace TestNess.Lib.Rule
{
    public class NonConditionalTestCaseRule : IRule
    {
        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            var count = BranchCount(testCase.TestMethod);
            if (count == 0)
            {
                yield break;
            }
            yield return new Violation(this, testCase);
        }

        public static int BranchCount(MethodDefinition method)
        {
            return method.HasBody ? method.Body.Instructions.Where(IsBranchingInstruction).Count() : 0;
        }

        private static bool IsBranchingInstruction(Instruction inst)
        {
            return inst.OpCode.FlowControl == FlowControl.Cond_Branch || inst.OpCode.FlowControl == FlowControl.Branch;
        }

        public override string ToString()
        {
            return "a test case should not be conditional";
        }
    }
}
