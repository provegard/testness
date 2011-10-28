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
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;

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

        private bool IsAssertCall(Instruction instruction, ICollection<MethodDefinition> assertingMethods)
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
