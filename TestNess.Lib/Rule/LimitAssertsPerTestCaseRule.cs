// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using TestNess.Lib.Feature;

namespace TestNess.Lib.Rule
{
    public class LimitAssertsPerTestCaseRule : IRule
    {
        private int _maxAsserts;

        public LimitAssertsPerTestCaseRule()
        {
            _maxAsserts = 1;
        }

        /// <summary>
        /// The maximum number of acceptable asserts that a test case can contain. By default, this value
        /// is 1. It cannot be set to a values less than 1.
        /// </summary>
        public int MaxNumberOfAsserts
        {
            get { return _maxAsserts; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Value must be >= 1");
                _maxAsserts = value;
            }
        }

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            var assertMethodCount = testCase.Features.Get<AssertionCountFeature>().Value;
            if (0 < assertMethodCount && assertMethodCount <= _maxAsserts)
                yield break; // no violation
            if (assertMethodCount == 0 && testCase.Framework.HasExpectedException(testCase.TestMethod))
                yield break; // no violation
            yield return new Violation(this, testCase, CreateViolationMessage(assertMethodCount));
        }

        private string CreateViolationMessage(int assertMethodCount)
        {
            return string.Format("test case contains {0} asserts (limit is {1})", assertMethodCount, MaxNumberOfAsserts);
        }

        public override string ToString()
        {
            var range = _maxAsserts == 1 ? "1" : "1 to " + _maxAsserts;
            var plural = _maxAsserts == 1 ? "" : "s";
            return string.Format("a test case should have {0} assert{1} or expect an exception", range, plural);
        }
    }
}
