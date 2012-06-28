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