// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Linq;
using TestNess.Lib.Cil;

namespace TestNess.Lib.Feature
{
    public class HasLoops : AbstractFeature<bool>
    {
        public HasLoops(TestCase testCase, Features features) : base(testCase, features)
        {
        }

        protected override bool GenerateValue(TestCase testCase, Features features)
        {
            var paths = features.Get<InstructionPaths>().Value;
            return paths.Any(p => p.ContainsLoop());
        }
    }
}
