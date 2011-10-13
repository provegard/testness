using System;
using System.Collections.Generic;

namespace GraphBuilder
{
    public class Graph<TNode>
    {
        private readonly IDictionary<TNode, IList<TNode>> _digraph;

        public Graph(IDictionary<TNode, IList<TNode>> digraph, TNode root)
        {
            _digraph = digraph;
            Root = root;
        }

        public TNode Root { get; private set; }

        public int Order { get { return _digraph.Count;  } }

        public IEnumerable<TNode> HeadsFor(TNode tail)
        {
            return GetHeads(tail);
        }

        public TNode HeadByIndex(TNode tail, int index)
        {
            return GetHeads(tail)[index];
        }

        public int OutDegreeOf(TNode tail)
        {
            return GetHeads(tail).Count;
        }

        private IList<TNode> GetHeads(TNode tail)
        {
            IList<TNode> heads;
            if (!_digraph.TryGetValue(tail, out heads))
            {
                throw new ArgumentException("Unknown node: " + tail);
            }
            return heads;
        }
    }
}
