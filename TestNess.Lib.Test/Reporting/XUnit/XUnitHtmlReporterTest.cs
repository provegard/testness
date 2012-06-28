using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Reporting.XUnit;
using TestNess.Lib.Rule;
// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using TestNess.Target;

namespace TestNess.Lib.Test.Reporting.XUnit
{
    [TestFixture]
    public class XUnitHtmlReporterTest
    {
        private string _html;

        [SetUp]
        public void GivenAReportBasedOnAnalysisOfOneTestCase()
        {
            //TODO: Make this simpler!!
            var tc1 = TestHelper.FindTestCase<IntegerCalculatorTest>(x => x.TestAddTwoAsserts());
            var rule = new LimitAssertsPerTestCaseRule();
            var scorer = Substitute.For<IViolationScorer>();
            scorer.CalculateScore(null).ReturnsForAnyArgs(1m);

            var results = AnalysisResults.Create(new[] { tc1 }, new[] { rule }, scorer);
            var sb = new StringBuilder();
            var writer = new XmlTextWriter(new StringWriter(sb));
            var report = new XUnitHtmlReporter(writer);
            report.GenerateReport(results);

            _html = sb.ToString();
        }

        [Test]
        public void ThenOuputContainsHtml()
        {
            StringAssert.Contains("<html>", _html);
        }
    }
}
