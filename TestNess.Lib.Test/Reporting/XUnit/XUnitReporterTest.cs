using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public class XUnitReporterTest
    {
        private XDocument _doc;

        [SetUp]
        public void GivenAReportBasedOnAnalysisOfTwoTestCases()
        {
            var tc1 = TestHelper.FindTestCase<IntegerCalculatorTest>(x => x.TestAddTwoAsserts());
            var tc2 = TestHelper.FindTestCase<IntegerCalculatorTest>(x => x.TestAddBasic());
            var rule = new LimitAssertsPerTestCaseRule();
            var scorer = Substitute.For<IViolationScorer>();
            scorer.CalculateScore(null).ReturnsForAnyArgs(1m);

            var results = AnalysisResults.Create(new[] {tc1, tc2}, new[] {rule}, scorer);
            var ms = new MemoryStream();
            var writer = XmlWriter.Create(ms);
            var report = new XUnitReporter(writer);
            _doc = report.GenerateXml(results);
        }

        [Test]
        public void ThenDocumentEncodingIsUtf8()
        {
            Assert.AreEqual("utf-8", _doc.Declaration.Encoding);
        }

        [Test]
        public void ThenDocumentIsStandAlone()
        {
            Assert.AreEqual("yes", _doc.Declaration.Standalone);
        }

        [TestCase("/assembly", @"^$")]
        [TestCase("/assembly/@name", @"^TestNess\.Target$")] //TODO: this should be path
        [TestCase("/assembly/@run-date", @"^\d{4}-\d{2}-\d{2}$")]
        [TestCase("/assembly/@run-time", @"^\d{2}:\d{2}:\d{2}$")]
        [TestCase("/assembly/@configFile", @"^$")] //TODO: ??
        [TestCase("/assembly/@environment", @"^(32|64)-bit \.NET [\.\d]+$")]
        [TestCase("/assembly/@test-framework", @"^TestNess$")] //TODO: version
        [TestCase("/assembly/@total", @"^2$")]
        [TestCase("/assembly/class", "^$")]
        [TestCase("/assembly/class/@name", @"^TestNess\.Target\.IntegerCalculatorTest$")]
        [TestCase("/assembly/class/@skipped", @"^0$")]
        [TestCase("/assembly/class/@failed", @"^1$")]
        [TestCase("/assembly/class/@passed", @"^1$")]
        [TestCase("/assembly/class/@total", @"^2$")]
        [TestCase("/assembly/class/@time", @"^\d\.\d{3}$")]
        [TestCase("/assembly/class/test[1]", @"^$")]
        [TestCase("/assembly/class/test[1]/@name", @"^TestNess\.Target\.IntegerCalculatorTest\.TestAddTwoAsserts \[.*assert.*\]$")]
        [TestCase("/assembly/class/test[1]/@type", @"^TestNess\.Target\.IntegerCalculatorTest$")]
        [TestCase("/assembly/class/test[1]/@method", @"^TestAddTwoAsserts$")]
        [TestCase("/assembly/class/test[1]/@result", @"^Fail$")]
        [TestCase("/assembly/class/test[1]/@time", @"^\d\.\d{3}$")]
        [TestCase("/assembly/class/test[1]/failure", @"^$")]
        [TestCase("/assembly/class/test[1]/failure/@exception-type", @"^.+$")] //TODO: ??
        [TestCase("/assembly/class/test[1]/failure/message", @"test case contains")]
        [TestCase("/assembly/class/test[1]/failure/message", @"^((?!IntegerCalculatorTest\.cs).)*$")]
        [TestCase("/assembly/class/test[2]/@result", @"^Pass$")]
        [TestCase("/assembly/class/test[2]/failure", null)]
        public void ThenDocumentHasExpectedProperty(string xpath, string valueRegEx)
        {
            var raw = _doc.XPathEvaluate(xpath);
            var obj = ((IEnumerable) raw).Cast<XObject>().FirstOrDefault();

            if (valueRegEx == null)
            {
                Assert.IsNull(obj);
            }
            else
            {
                string value = null;
                if (obj is XElement)
                    value = TextValue((XElement) obj);
                else if (obj is XAttribute)
                    value = ((XAttribute) obj).Value;
                StringAssert.IsMatch(valueRegEx, value);
            }
        }

        [Test]
        public void ThenTheTotalElapsedTimeIsReasonable()
        {
            // Test case added because of an initial error whereby elapsed time
            // was reported in milliseconds rather than seconds.
            var value = Convert.ToDouble(_doc.XPathEvaluate("string(/assembly/@time)"), CultureInfo.InvariantCulture);
            Assert.LessOrEqual(value, 1.0d);
        }

        private string TextValue(XContainer elem)
        {
            return elem.Nodes().OfType<XText>().Aggregate("", (s, text) => s + text);
        }
    }
}
