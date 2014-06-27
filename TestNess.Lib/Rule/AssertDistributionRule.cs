// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
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

            var sequencePointCount = 0;
            var assertingSequencePoints = new List<int>();

            var tm = testCase.TestMethod;
            foreach (var ins in tm.Body.Instructions)
            {
                var sp = ins.SequencePoint;
                if (sp != null && IsSignificantSequencePoint(ins, sp))
                {
                    sequencePointCount++;
                }
                if (sequencePointCount > 0 && IsAssertCall(ins, assertingMethods))
                {
                    assertingSequencePoints.Add(sequencePointCount - 1);
                }
            }

            if (assertingSequencePoints.Count == 0)
                yield break; // this rule doesn't apply
            // If the X asserting sps are the X last ones, then it's ok!
            if (assertingSequencePoints[0] + assertingSequencePoints.Count == sequencePointCount)
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

        private bool IsAssertCall(Instruction instruction, IEnumerable<MethodDefinition> assertingMethods)
        {
            if (instruction.OpCode.FlowControl != FlowControl.Call)
                return false;
            var methodReference = instruction.Operand as MethodReference;
            var found = assertingMethods.Where(m => m.FullName.Equals(methodReference.FullName));
            return found.Count() > 0;
        }

        public override string ToString()
        {
            return "all asserts in a test case should be placed together last in the test method";
        }
    }
}
