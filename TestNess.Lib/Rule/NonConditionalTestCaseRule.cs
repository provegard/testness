// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using TestNess.Lib.Feature;

namespace TestNess.Lib.Rule
{
    public class NonConditionalTestCaseRule : IRule
    {
        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            var features = testCase.Features;
            var count = features.Get<SingleCodePathAssertionCount>().Value;
            var hasLoops = features.Get<HasLoops>().Value;
            if (count == 0 && !hasLoops)
            {
                yield break;
            }
            yield return new Violation(this, testCase);
        }

        public override string ToString()
        {
            return "a test case should not be conditional";
        }
    }
}
