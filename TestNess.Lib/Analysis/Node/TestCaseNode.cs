using System.Collections.Generic;
using System.Linq;
using GraphBuilder;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Analysis.Node
{
    public class TestCaseNode : AnalysisNode
    {
        private IList<Violation> _violations;
        public TestCase TestCase { get; private set; }
        public IRule Rule { get; private set; }

        public TestCaseNode(TestCase testCase, IRule rule)
        {
            TestCase = testCase;
            Rule = rule;
        }

        public override IList<Violation> GetViolations(Graph<AnalysisNode> graph)
        {
            if (_violations != null)
                return _violations;

            _violations = new List<Violation>();
            //TODO: parallelize here??
            foreach (var v in Rule.Apply(TestCase))
            {
                _violations.Add(v);
            }

            return _violations;
        }

        public override void Accept(AnalysisTreeVisitor v)
        {
            v.Visit(this);
        }
    }
}
