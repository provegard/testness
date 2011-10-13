using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphBuilder
{
    public class Graph<TNode> where TNode: class
    {
        private readonly IDictionary<TNode, IList<TNode>> _digraph;

        public delegate void Walker(TNode node);

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

        public void Walk(Walker walker, TNode start = null)
        {
            var node = start ?? Root;
            Dfs(node, walker);
        }

        private void Dfs(TNode node, Walker walker)
        {
            var stack = new Stack<TNode>();
            var vertices = _digraph.Keys;
            var visited = new Dictionary<TNode, bool>();
            foreach (var v in vertices)
            {
                visited[v] = false;
            }
            stack.Push(node);
            while (stack.Count > 0)
            {
                var u = stack.Pop();
                if (visited[u])
                    continue;
                visited[u] = true;
                walker(u);
                var unvisited = from h in HeadsFor(u) where !visited[h] select h;
                // Add in reverse order to maintain head order when visiting
                foreach (var neighbor in unvisited.Reverse())
                {
                    stack.Push(neighbor);
                }
            }
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
