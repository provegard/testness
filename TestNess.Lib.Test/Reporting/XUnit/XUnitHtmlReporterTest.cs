// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using NSubstitute;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Reporting;
using TestNess.Lib.Reporting.XUnit;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Reporting.XUnit
{
    [TestFixture]
    public class XUnitHtmlReporterTest
    {
        private IReportReceiver _receiver;

        [SetUp]
        public void GivenAReportBasedOnAnalysisOfOneTestCase()
        {
            var tc1 = TestHelper.FindTestCase<IntegerCalculatorTest>(x => x.TestAddTwoAsserts());
            var rule = new LimitAssertsPerTestCaseRule();
            var scorer = Substitute.For<IViolationScorer>();
            scorer.CalculateScore(null).ReturnsForAnyArgs(1m);

            var results = AnalysisResults.Create(new[] { tc1 }, new[] { rule }, scorer);
            var report = new XUnitHtmlReporter();
            _receiver = Substitute.For<IReportReceiver>();
            report.GenerateReport(_receiver, results);
        }

        [Test]
        public void ThenOuputContainsHtml()
        {
            _receiver.Received().GenerateReport(Arg.Is<string>(s => s.Contains("<html>")));
        }
    }
}
