// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
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
            var sequencePoints = instructions.Select(i => i.SequencePoint).Where(sp => sp != null).ToList();
            var lineNumbers = new HashSet<int>(sequencePoints.Select(sp => sp.StartLine));
            var spCount = sequencePoints.Count;

            var actual = spCount > 0 ? lineNumbers.Count : (int)(ilCount / IL_PER_LOC);
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
