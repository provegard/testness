using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Analysis.Node;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Analysis
{
    [TestFixture]
    public class AnalysisTreeVisitorTest
    {
        private IList<string> _visitOutput;

        [SetUp]
        public void GivenAnAnalyzerAfterAnalysis()
        {
            var tc1 = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddTwoAsserts());
            var analyzer = new Analyzer(new[] { tc1 }, new Rules(typeof(IRule).Assembly).Take(1));
            analyzer.Analyze();
            var tree = analyzer.AnalysisTree;

            var visitor = new TestVisitor();
            visitor.Traverse(tree);
            _visitOutput = visitor.Rows;
        }

        [Test]
        public void TestThatAssemblyNodeIsVisited()
        {
            CollectionAssert.Contains(_visitOutput, "assembly: TestNess.Target");
        }

        [Test]
        public void TestThatNamespaceNodeIsVisited()
        {
            CollectionAssert.Contains(_visitOutput, "  ns: TestNess");
        }

        [Test]
        public void TestThatTypeNodeIsVisited()
        {
            CollectionAssert.Contains(_visitOutput, "      type: TestNess.Target.IntegerCalculatorTest");
        }

        [Test]
        public void TestThatTestCaseNodeIsVisited()
        {
            const string str = "        test case: TestNess.Target.IntegerCalculatorTest::TestAddTwoAsserts()";
            var found = _visitOutput.Any(s => s.StartsWith(str));
            Assert.IsTrue(found);
        }

        internal class TestVisitor : AnalysisTreeVisitor
        {
            internal readonly IList<string> Rows = new List<string>();
            private int _level;

            private void LevelVisit(Action a)
            {
                _level++;
                a();
                _level--;
            }

            public override void Visit(AssemblyNode node)
            {
                Rows.Add(string.Format("{0}assembly: {1}", Ws(), node.Assembly.Name.Name));
                LevelVisit(() => base.Visit(node));
            }

            public override void Visit(NamespaceNode node)
            {
                Rows.Add(string.Format("{0}ns: {1}", Ws(), node.Name));
                LevelVisit(() => base.Visit(node));
            }

            public override void Visit(TypeNode node)
            {
                Rows.Add(string.Format("{0}type: {1}", Ws(), node.Type.FullName));
                LevelVisit(() => base.Visit(node));
            }

            public override void Visit(TestCaseNode node)
            {
                Rows.Add(string.Format("{0}test case: {1} [{2}]", Ws(), node.TestCase.Name, node.Rule));
                LevelVisit(() => base.Visit(node));
            }

            private const int WsLen = 2;

            private string Ws()
            {
                return new string(' ', WsLen * _level);
            }
        }
    }
}
