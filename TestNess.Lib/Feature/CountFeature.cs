// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

namespace TestNess.Lib.Feature
{
    public abstract class CountFeature : AbstractFeature<int>
    {
        protected CountFeature(TestCase testCase, Features features): base(testCase, features)
        {
        }
    }
}
