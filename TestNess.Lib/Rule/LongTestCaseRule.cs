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
using Mono.Cecil.Cil;

namespace TestNess.Lib.Rule
{
    public class LongTestCaseRule : IRule
    {
        // Found empirically, but the value is hardly universal, so more data
        // are needed!!
        private const float IL_PER_LOC = 4.0f;

        private int _maxLinesOfCode;

        public LongTestCaseRule()
        {
            _maxLinesOfCode = 10;
        }

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            var instructions = testCase.TestMethod.Body.Instructions.Where(IsRelevant).ToList();
            var ilCount = instructions.Count;
            var spCount = instructions.Where(i => i.SequencePoint != null).Count();

            var actual = spCount > 0 ? spCount : (int) (ilCount / IL_PER_LOC);
            if (actual <= _maxLinesOfCode)
                yield break; // no violation
            yield return new Violation(this, testCase, CreateMessage(actual));
        }

        private string CreateMessage(int actualStatementCount)
        {
            return string.Format("test case contains {0} code statements (limit is {1})", actualStatementCount,
                                 _maxLinesOfCode);
        }

        private static bool IsRelevant(Instruction instruction)
        {
            if (instruction.OpCode == OpCodes.Nop && instruction.Previous == null)
                return false;
            if (instruction.OpCode == OpCodes.Ret && instruction.Next == null)
                return false;
            return true;
        }

        public override string ToString()
        {
            return string.Format("a test case should contain at most {0} code statements", _maxLinesOfCode);
        }

        /// <summary>
        /// The limit for the acceptable number of code statements that a test case can contain. By 
        /// default, this value is 10. It cannot be set to a values less than 1.
        /// </summary>
        public int MaxNumberOfLinesOfCode
        {
            get { return _maxLinesOfCode; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Value must be >= 1");
                _maxLinesOfCode = value;
            }
        }
    }
}
