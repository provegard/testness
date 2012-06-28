// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphBuilder
{
    /// <summary>
    /// A graph of nodes (vertices). This is in fact a directed multigraph.
    /// </summary>
    /// <typeparam name="TNode">The type of nodes (vertices) in this graph.</typeparam>
    public class Graph<TNode> where TNode: class
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

        public IEnumerable<TNode> Walk(TNode start = null)
        {
            var node = start ?? Root;
            return Dfs(node);
        }

        private IEnumerable<TNode> Dfs(TNode node)
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
                yield return u;
                var unvisited = HeadsFor(u).Where(h => !visited[h]);
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

        /// <summary>
        /// Determines if the given node belongs to this graph.
        /// </summary>
        /// <param name="node">A node.</param>
        /// <returns>A flag indicating whether or not the node belongs to this graph.</returns>
        public bool Contains(TNode node)
        {
            return _digraph.ContainsKey(node);
        }

        public IEnumerable<TNode> TailsFor(TNode head)
        {
            return from key in _digraph.Keys
                   let headCount = _digraph[key].Count(node => node.Equals(head))
                   where headCount > 0
                   from repeatedKey in Enumerable.Repeat(key, headCount)
                   select repeatedKey;
        }

        public IEnumerable<IList<TNode>> FindPaths(TNode start, TNode end)
        {
            var paths = new List<IList<TNode>>();
            FindPaths(start, end, new List<Edge>(), paths);
            return paths;
        }

        private void FindPaths(TNode start, TNode end, ICollection<Edge> visitedEdges, ICollection<IList<TNode>> paths)
        {
            if (start.Equals(end))
            {
                var path = visitedEdges.Select(edge => edge.Source).ToList();
                path.Add(end);
                paths.Add(path);
                return;
            }

            var edges = HeadsFor(start).Select(head => new Edge {Source = start, Target = head}).Where(e => !visitedEdges.Contains(e));
            foreach (var edge in edges)
            {
                visitedEdges.Add(edge);
                var target = edge.Target;
                FindPaths(target, end, visitedEdges, paths);
                visitedEdges.Remove(edge);
            }
        }

        private struct Edge
        {
            internal TNode Source;
            internal TNode Target;
        }
    }
}
