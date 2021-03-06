﻿// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Analysis
{
    // Applies a rule to a test case and collects information about the outcome.
    public class TestCaseRuleApplication
    {
        private decimal _score = -1;
        private readonly IViolationScorer _scorer;

        public TestCaseRuleApplication(TestCase testCase, IRule rule, IViolationScorer scorer)
        {
            _scorer = scorer;
            TestCase = testCase;
            Rule = rule;
            Apply();
        }

        public TestCase TestCase { get; private set; }
        public IRule Rule { get; private set; }
        public long ElapsedTimeInMilliseconds { get; private set; }
        public IEnumerable<Violation> Violations { get; private set; }

        public decimal Score
        {
            get
            {
                if (_score < 0)
                {
                    _score = Violations.Sum(v => _scorer.CalculateScore(v));
                }
                return _score;
            }
        }

        private void Apply()
        {
            var sw = Stopwatch.StartNew();
            Violations = Rule.Apply(TestCase).ToList();
            sw.Stop();
            ElapsedTimeInMilliseconds = sw.ElapsedMilliseconds;
        }
    }
}
