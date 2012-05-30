using System;
using System.Linq;
using System.Text.RegularExpressions;
using GraphBuilder;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Analysis.Node;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Analysis
{
    [TestFixture]
    public class AnalyzerAfterAnalysisTest
    {
        private Analyzer _analyzer;

        [SetUp]
        public void GivenAnAnalyzerAfterAnalysis()
        {
            var tc1 = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddTwoAsserts());
            var tc2 = TestHelper.FindTestCase<IntegerCalculatorMethodLengthTest>(t => t.ThenWeShouldBeAbleToAdd_Long());
            _analyzer = new Analyzer(new[] {tc1, tc2}, new Rules(typeof(IRule).Assembly));
            _analyzer.Analyze();
        }

        [TestCase]
        public void TestThatThereAreViolations()
        {
            Assert.AreEqual(2, _analyzer.Violations.Count());
        }

        [TestCase]
        public void TestThatRootOfTreeIsAssemblyNode()
        {
            var root = _analyzer.AnalysisTree.Root;
            Assert.IsInstanceOf<AssemblyNode>(root);
        }

        [TestCase]
        public void TestThatTreeStructureContainsAssembly()
        {
            var str = DescribeTree(_analyzer.AnalysisTree);
            StringAssert.Contains("assembly: TestNess.Target", str);
        }

        [TestCase]
        public void TestThatTreeStructureContainsRootNamespace()
        {
            var str = DescribeTree(_analyzer.AnalysisTree);
            Assert.IsTrue(Regex.IsMatch(str, @"ns: TestNess\r?$", RegexOptions.Multiline));
        }

        [TestCase]
        public void TestThatTreeStructureContainsFullNamespace()
        {
            var str = DescribeTree(_analyzer.AnalysisTree);
            StringAssert.Contains("ns: TestNess.Target", str);
        }

        [TestCase]
        public void TestThatTreeStructureContainsType()
        {
            var str = DescribeTree(_analyzer.AnalysisTree);
            StringAssert.IsMatch(@"\bTestNess\.Target\.IntegerCalculatorTest\b", str);
        }

        [TestCase]
        public void TestThatTreeStructureContainsTestCase()
        {
            var str = DescribeTree(_analyzer.AnalysisTree);
            StringAssert.Contains("IntegerCalculatorTest::TestAddTwoAsserts()", str);
        }

        private string DescribeTree(Graph<AnalysisNode> tree)
        {
            var visitor = new AnalysisTreeVisitorTest.TestVisitor();
            visitor.Traverse(tree);
            return string.Join(Environment.NewLine, visitor.Rows);
        }
    }
}
