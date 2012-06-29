// Copyright (C) 2011-2012 Per Roveg�rd, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Analysis
{
    public class AnalysisResults
    {
        public AnalysisResults(IEnumerable<TestCaseRuleApplication> applications)
        {
            Measure(applications);
        }

        private void Measure(IEnumerable<TestCaseRuleApplication> applications)
        {
            AnalysisTime = DateTime.Now;
            var sw = Stopwatch.StartNew();
            Applications = new ReadOnlyCollection<TestCaseRuleApplication>(applications.ToList());
            sw.Stop();
            ElapsedTimeInMilliseconds = sw.ElapsedMilliseconds;
        }

        public DateTime AnalysisTime { get; private set; }

        public long ElapsedTimeInMilliseconds { get; private set; }

        public IList<TestCaseRuleApplication> Applications { get; private set; }

        public static AnalysisResults Create(IEnumerable<TestCase> testCases, IEnumerable<IRule> rules, IViolationScorer scorer)
        {
            return new AnalysisResults(rules.SelectMany(r => testCases, (r, tc) => new TestCaseRuleApplication(tc, r, scorer)));
        }
    }
}