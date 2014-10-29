// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using TestNess.Lib.Cil;

namespace TestNess.Lib.Feature
{
    public class InstructionPaths : AbstractFeature<IList<IList<Instruction>>>
    {
        public InstructionPaths(TestCase testCase, Features features) : base(testCase, features)
        {
        }

        protected override IList<IList<Instruction>> GenerateValue(TestCase testCase, Features features)
        {
            return testCase.GetInstructionGraph().FindInstructionPaths().ToList();
        }
    }
}
