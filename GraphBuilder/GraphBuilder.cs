using System.Collections.Generic;
using System.Linq;

namespace GraphBuilder
{
    public class GraphBuilder<TNode>
    {
        private readonly HeadFinder _headFinder;

        public delegate IEnumerable<TNode> HeadFinder(TNode tail);

        public GraphBuilder(HeadFinder headFinder)
        {
            _headFinder = headFinder;
        }

        public Graph<TNode> Build(TNode root)
        {
            IDictionary<TNode, IList<TNode>> digraph = new Dictionary<TNode, IList<TNode>>();
            Populate(digraph, root);

            return new Graph<TNode>(digraph, root);
        }

        private void Populate(IDictionary<TNode, IList<TNode>> digraph, TNode node)
        {
            if (digraph.ContainsKey(node))
                return;
            var heads = _headFinder(node).ToList();
            digraph.Add(node, heads);
            foreach (var head in heads)
            {
                Populate(digraph, head);
            }
        }
    }
}
