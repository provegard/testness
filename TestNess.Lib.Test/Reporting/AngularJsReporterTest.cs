// Copyright (C) 2011-2013 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.IO;
using CsQuery;
using NSubstitute;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Reporting;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Reporting
{
    [TestFixture]
    public class AngularJsReporterTest
    {
        private CQ _cq;
        private string _text;

        [SetUp]
        public void GivenAReportBasedOnAnalysisOfTwoTestCases()
        {
            var tc1 = TestHelper.FindTestCase<IntegerCalculatorTest>(x => x.TestAddTwoAsserts());
            var tc2 = TestHelper.FindTestCase<IntegerCalculatorTest>(x => x.TestAddBasic());
            var rule = new LimitAssertsPerTestCaseRule();
            var scorer = Substitute.For<IViolationScorer>();
            scorer.CalculateScore(null).ReturnsForAnyArgs(1m);

            var results = AnalysisResults.Create(new[] { tc1, tc2 }, new[] { rule }, scorer);
            var report = new AngularJsReporter();
            var writer = new StringWriter();
            report.GenerateReport(writer, results);

            _text = writer.ToString();
            _cq = new CQ(_text);
        }

        [Test]
        public void ItShouldHaveHtml5Doctype()
        {
            Assert.AreEqual(DocType.HTML5, _cq.Document.DocType);
        }

        [Test]
        public void ItShouldReferToAngularAsScript()
        {
            Assert.AreEqual(1, _cq["script[src*=\"angular\"]"].Length);
        }

        [Test]
        public void ItShouldReferToBootstrapAsCss()
        {
            Assert.AreEqual(1, _cq["link[rel=\"stylesheet\"][href*=\"bootstrap\"]"].Length);
        }

        [Test]
        public void ItShouldPutAnalysisResultsInline()
        {
            StringAssert.Contains("var analysisResults = {", _text);
        }

        [Test]
        public void ItShouldCreateAnAngularApp()
        {
            Assert.AreEqual(1, _cq["html[data-ng-app=\"testNessApp\"]"].Length);
        }

        [Test]
        public void ItShouldHaveAControllerForTheBody()
        {
            Assert.AreEqual(1, _cq["body[data-ng-controller=\"MainController\"]"].Length);
        }
    }
}
