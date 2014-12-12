// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace TestNess.Lib.Rule
{
    public class AssertDistributionRule : IRule
    {
        private const int HiddenLine = 0xfeefee;

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            if (!testCase.TestMethod.DeclaringType.Module.HasSymbols)
                yield break; //TODO: decide what to do here!

            var assertingMethods = testCase.GetCalledAssertingMethods();

            // Note: The Mono compiler appears to emit multiple sequence points with the same start line,
            // i.e. there are multiple instructions with sequence points that refer to the same line.
            // Therefore, let's store line numbers and associate line numbers with asserting calls.

            var sequencePointsStartLines = new SortedSet<int>();
            var assertingSequencePointsStartLines = new SortedSet<int>();

            var tm = testCase.TestMethod;
            foreach (var ins in tm.Body.Instructions)
            {
                var sp = ins.SequencePoint;
                if (sp != null && IsSignificantSequencePoint(ins, sp))
                {
                    sequencePointsStartLines.Add(sp.StartLine);
                }
                if (sequencePointsStartLines.Count > 0 && IsAssertCall(ins, assertingMethods))
                {
                    // As sequence point, use the last one added, which isn't necessarily sp,
                    // since the asserting instruction may lack sequence point.
                    var lastSpLineNumber = sequencePointsStartLines.Last();
                    assertingSequencePointsStartLines.Add(lastSpLineNumber);
                }
            }

            if (assertingSequencePointsStartLines.Count == 0)
                yield break; // this rule doesn't apply
            // If the X asserting sps are the X last ones, then it's ok!
            if (sequencePointsStartLines.EndsWith(assertingSequencePointsStartLines))
                yield break;
            yield return new Violation(this, testCase);
        }

        private bool IsSignificantSequencePoint(Instruction ins, SequencePoint sp)
        {
            if (ins.OpCode == OpCodes.Nop)
                return false;
            if (ins.OpCode == OpCodes.Ret && ins.Next == null)
                return false;
            return sp.StartLine != HiddenLine;
        }

        private bool IsAssertCall(Instruction instruction, IEnumerable<MethodCall> assertingMethods)
        {
            if (instruction.OpCode.FlowControl != FlowControl.Call)
                return false;
            return assertingMethods.Any(mc => mc.Instruction == instruction);
        }

        public override string ToString()
        {
            return "all asserts in a test case should be placed together last in the test method";
        }
    }
}
