using System.Linq;
using System.Collections.Generic;
using GraphBuilder;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Analysis.Node
{
    public abstract class AnalysisNode
    {
        private IList<Violation> _violations; 

        public virtual IList<Violation> GetViolations(Graph<AnalysisNode> graph)
        {
            if (_violations != null)
                return _violations;

            _violations = new List<Violation>();
            foreach (var v in graph.HeadsFor(this).SelectMany(h => h.GetViolations(graph)))
            {
                _violations.Add(v);
            }

            return _violations;
        }

        public abstract void Accept(AnalysisTreeVisitor v);
    }
}
