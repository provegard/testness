using GraphBuilder;
using TestNess.Lib.Analysis.Node;

namespace TestNess.Lib.Analysis
{
    public class AnalysisTreeVisitor
    {
        private Graph<AnalysisNode> _tree; 

        public void Traverse(Graph<AnalysisNode> tree)
        {
            _tree = tree;
            tree.Root.Accept(this);
        }
        
        public virtual void Visit(AssemblyNode node)
        {
            VisitHeads(node);
        }

        public virtual void Visit(NamespaceNode node)
        {
            VisitHeads(node);
        }

        public virtual void Visit(TypeNode node)
        {
            VisitHeads(node);
        }

        public virtual void Visit(TestCaseNode node)
        {
            VisitHeads(node);
        }

        private void VisitHeads(AnalysisNode node)
        {
            foreach (var head in _tree.HeadsFor(node))
            {
                head.Accept(this);
            }
        }
    }
}
