using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Analysis
{
    [TestFixture]
    public class AnalysisResultsTest
    {
        private AnalysisResults _results;

        [TestFixtureSetUp]
        public void GivenAnalysisResults()
        {
            var rule = new DelayRule();
            var testCase = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddTwoAsserts());
            var app = new TestCaseRuleApplication(testCase, rule, new ViolationScorer());
            _results = new AnalysisResults(Enumerable.Repeat(app, 1));
        }

        [Test]
        public void TestThatTheAnalysisTimeIsNow()
        {
            var diff = DateTime.Now - _results.AnalysisTime;
            Assert.LessOrEqual(diff.TotalMilliseconds, 500);
        }

        [Test]
        public void TestThatTheTotalElapsedTimeIsMeasured()
        {
            var ms = _results.ElapsedTimeInMilliseconds;
            Assert.Greater(ms, 0);
        }

        [Test]
        public void TestThatTheIndividualApplicationsAreExposedAsAList()
        {
            var list = _results.Applications;
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void TestThatTheApplicationsListIsReadOnly()
        {
            var list = _results.Applications;
            Assert.Throws<NotSupportedException>(list.Clear);
        }

        private class DelayRule : IRule
        {
            public IEnumerable<Violation> Apply(TestCase testCase)
            {
                Thread.Sleep(10);
                yield return new Violation(this, testCase, "foobar");
            }
        }
    }
}
