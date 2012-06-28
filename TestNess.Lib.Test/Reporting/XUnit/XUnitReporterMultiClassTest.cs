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
    public partial class XUnitReporterMultiClassTest
    {
        private XDocument _doc;

        [SetUp]
        public void GivenAReportBasedOnAnalysisOfTestCasesInTwoClasses()
        {
            var tc1 = TestHelper.FindTestCase<IntegerCalculatorTest>(x => x.TestAddTwoAsserts());
            var tc2 = TestHelper.FindTestCase<IntegerCalculatorConditionalTest>(x => x.TestAddWithIf());
            var rule = new LimitAssertsPerTestCaseRule();
            var scorer = Substitute.For<IViolationScorer>();
            scorer.CalculateScore(null).ReturnsForAnyArgs(1m);

            //TODO: lot of duplicated code from XUnitReporterTest
            var results = AnalysisResults.Create(new[] { tc1, tc2 }, new[] { rule }, scorer);
            var ms = new MemoryStream();
            var writer = XmlWriter.Create(ms);

            var report = new XUnitReporter(writer);
            _doc = report.GenerateXml(results);
        }

        [Test]
        public void ThenTheXmlContainsTwoClassElements()
        {
            var raw = Convert.ToInt32(_doc.XPathEvaluate("count(/assembly/class)"));
            Assert.AreEqual(2, raw);
        }
    }
}
