// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using TestNess.Lib.Feature;

namespace TestNess.Lib.Rule
{
    public class LongTestCaseRule : IRule
    {
        private int _maxLinesOfCode;

        public LongTestCaseRule()
        {
            _maxLinesOfCode = 10;
        }

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            var metric = testCase.Features.Get<StatementCount>();
            var actual = metric.Value;
            if (actual <= _maxLinesOfCode)
                yield break; // no violation
            yield return new Violation(this, testCase, CreateMessage(actual));
        }

        private string CreateMessage(int actualStatementCount)
        {
            return string.Format("test case contains {0} code statements (limit is {1})", actualStatementCount,
                                 _maxLinesOfCode);
        }

        public override string ToString()
        {
            return string.Format("a test case should contain at most {0} code statements", _maxLinesOfCode);
        }

        /// <summary>
        /// The limit for the acceptable number of code statements that a test case can contain. By 
        /// default, this value is 10. It cannot be set to a values less than 1.
        /// </summary>
        public int MaxNumberOfLinesOfCode
        {
            get { return _maxLinesOfCode; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Value must be >= 1");
                _maxLinesOfCode = value;
            }
        }
    }
}
