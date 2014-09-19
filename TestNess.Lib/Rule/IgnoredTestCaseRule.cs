// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Collections.Generic;
using TestNess.Lib.TestFramework;

namespace TestNess.Lib.Rule
{
    public class IgnoredTestCaseRule : IRule
    {
        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            string reason;
            if (testCase.Framework.IsIgnoredTest(testCase.TestMethod, out reason))
            {
                // Ignored with a reason    => normal severity
                // Ignored without a reason => high severity
                var severityFactory = reason != null ? 1m : 1.5m;
                var msg = string.Format("Test case is ignored (with{0} reason).",
                    reason != null ? "" : "out");
                yield return new Violation(this, testCase, msg, severityFactory);
            }
        }

        public override string ToString()
        {
            return "a test case should not be ignored";
        }
    }
}
