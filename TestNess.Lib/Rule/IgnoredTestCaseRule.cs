// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Collections.Generic;
using TestNess.Lib.TestFramework;

namespace TestNess.Lib.Rule
{
    public class IgnoredTestCaseRule : IRule
    {
        private readonly ITestFramework _framework = TestFrameworks.Instance;

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            if (_framework.IsIgnoredTest(testCase.TestMethod))
                yield return new Violation(this, testCase, "Test case is ignored.");
        }

        public override string ToString()
        {
            return "a test case should not be ignored";
        }
    }
}
