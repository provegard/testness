// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NSubstitute;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Reporting.XUnit;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Reporting.XUnit
{
    [TestFixture]
    public class XUnitReporterSpecialCasesTest
    {
        [Test]
        public void TestThatTestCasesInTwoClassesResultsInXmlWithTwoClassElements()
        {
            var tc1 = TestHelper.FindTestCase<IntegerCalculatorTest>(x => x.TestAddTwoAsserts());
            var tc2 = TestHelper.FindTestCase<IntegerCalculatorConditionalTest>(x => x.TestAddWithIf());
            var rule = new LimitAssertsPerTestCaseRule();
            var scorer = Substitute.For<IViolationScorer>();
            scorer.CalculateScore(null).ReturnsForAnyArgs(1m);

            var results = AnalysisResults.Create(new[] { tc1, tc2 }, new[] { rule }, scorer);
            var report = new XUnitReporter();
            var doc = report.GenerateXml(results);

            var raw = Convert.ToInt32(doc.XPathEvaluate("count(/assembly/class)"));
            Assert.AreEqual(2, raw);
        }

        [Test]
        public void TestThatEmptyAnalysisResultsIsHandled()
        {
            var results = new AnalysisResults(new TestCaseRuleApplication[0]);
            var reporter = new XUnitReporter();
            Assert.Throws<ArgumentException>(() => reporter.GenerateReport(new StringWriter(), results));
        }

        [Test]
        public void TestThatMultipleViolationsPerTestCaseArePointedOut()
        {
            var tc1 = TestHelper.FindTestCase<IntegerCalculatorExpectationTest>(x => x.TestAddWithConditionalExpectationViolations());
            var rule = new LocalExpectationRule();
            var scorer = Substitute.For<IViolationScorer>();
            scorer.CalculateScore(null).ReturnsForAnyArgs(1m);

            var results = AnalysisResults.Create(new[] { tc1 }, new[] { rule }, scorer);
            var report = new XUnitReporter();
            var doc = report.GenerateXml(results);

            var message = doc.XPathSelectElement("//message").Value;
            StringAssert.Contains("2 violations", message);
        }
    }
}
