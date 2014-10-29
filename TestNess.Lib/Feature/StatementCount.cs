// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace TestNess.Lib.Feature
{
    public class StatementCount : CountFeature
    {
        // Found empirically, but the value is hardly universal, so more data
        // are needed!!
        private const float IL_PER_LOC = 4.0f;

        public StatementCount(TestCase testCase, Features f) : base(testCase, f)
        {
        }

        protected override int GenerateValue(TestCase tc, Features f)
        {
            var instructions = tc.TestMethod.Body.Instructions.Where(IsRelevant).ToList();
            var ilCount = instructions.Count;
            var sequencePoints = instructions.Select(i => i.SequencePoint).Where(sp => sp != null).ToList();
            var lineNumbers = new HashSet<int>(sequencePoints.Select(sp => sp.StartLine));
            var spCount = sequencePoints.Count;

            return spCount > 0 ? lineNumbers.Count : (int)(ilCount / IL_PER_LOC);
        }

        private static bool IsRelevant(Instruction instruction)
        {
            if (instruction.OpCode == OpCodes.Nop && instruction.Previous == null)
                return false;
            if (instruction.OpCode == OpCodes.Ret && instruction.Next == null)
                return false;
            return true;
        }
    }
}
