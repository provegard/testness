// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

namespace TestNess.Lib.Feature
{
    public abstract class AbstractFeature<T> : IFeature
    {
        private readonly TestCase _testCase;
        private readonly Features _features;

        private T _value;
        private bool _hasValue;

        protected AbstractFeature(TestCase testCase, Features features)
        {
            _testCase = testCase;
            _features = features;
        }

        public T Value
        {
            get
            {
                if (!_hasValue)
                {
                    _value = GenerateValue(_testCase, _features);
                    _hasValue = true;
                }
                return _value;
            }
        }

        protected abstract T GenerateValue(TestCase testCase, Features features);
    }
}
