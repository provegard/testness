﻿/**
 * Copyright (C) 2011 by Per Rovegård (per@rovegard.se)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;

namespace TestNess.Lib.Rule
{
    public class LimitAssertsPerTestCaseRule : IRule
    {
        private readonly ITestFramework _framework = TestFrameworks.Instance;
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
            var assertMethodCount = testCase.GetCalledAssertingMethods().Count;
            if (0 < assertMethodCount && assertMethodCount <= _maxAsserts)
                yield break; // no violation
            if (assertMethodCount == 0 && _framework.HasExpectedException(testCase.TestMethod))
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
