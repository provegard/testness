// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Linq;
using TestNess.Lib.TestFramework;

namespace TestNess.Lib
{
    public class RelatedFrameworks
    {
        public readonly IEnumerable<IMockFramework> MockFrameworks;
        public readonly ITestFramework TestFramework;

        public RelatedFrameworks(ITestFramework framework, params IMockFramework[] others)
        {
            MockFrameworks = others.ToArray(); // make copy
            TestFramework = framework;
        }
    }
}