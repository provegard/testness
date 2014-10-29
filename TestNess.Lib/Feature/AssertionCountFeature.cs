// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

namespace TestNess.Lib.Feature
{
    public class AssertionCountFeature : CountFeature
    {
        public AssertionCountFeature(TestCase testCase, Features f) : base(testCase, f)
        {
        }

        protected override int GenerateValue(TestCase tc, Features f)
        {
            return tc.GetCalledAssertingMethods().Count;
        }
    }
}
