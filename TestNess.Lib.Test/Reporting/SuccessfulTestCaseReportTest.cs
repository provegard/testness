using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NSubstitute;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Reporting;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Reporting
{
    [TestFixture]
    public class SuccessfulTestCaseReportTest
    {
        private XDocument _doc;

        //[SetUp]
        //public void WithASingleSuccesfulTestCase()
        //{
        //    var testCase = typeof(IntegerCalculatorTest).FindTestCase("TestAddBasic()");
        //    var rule = new LimitAssertsPerTestCaseRule();

        //    var analyzerMock = Substitute.For<IAnalyzer>();
        //    analyzerMock.TestCases.Returns(new[] {testCase});
        //    analyzerMock.Rules.Returns(new[] {rule});
        //    analyzerMock.Violations.Returns(new Violation[0]);
        //    analyzerMock.Score.Returns(0);

        //    var reporter = new Reporter(analyzerMock);
        //    _doc = reporter.Generate();
        //}

        //[TestCase]
        //public void TestThatRootIsAssembly()
        //{
        //    Assert.AreEqual("assembly", _doc.Root.Name);
        //}

        //[TestCase]
        //public void TestThatRootIsAssembly()
        //{
        //    Assert.AreEqual("assembly", _doc.Root.Name);
        //}

    }
}
