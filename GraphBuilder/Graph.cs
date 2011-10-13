/**
 * Copyright (C) 2011 by Per Rovegård (per@rovegard.se)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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

        public IEnumerable<TNode> TailsFor(TNode head)
        {
            return from key in _digraph.Keys
                   where _digraph[key].Contains(head)
                   select key;
        }

        public IEnumerable<IList<TNode>> FindPaths(TNode start, TNode end)
        {
            var paths = new List<IList<TNode>>();
            var visited = new List<TNode> {start};
            Bfs(visited, end, paths);
            return paths;
        }

        private void Bfs(IList<TNode> visited, TNode end, ICollection<IList<TNode>> paths)
        {
            var nodes = HeadsFor(visited.Last()).ToList();
            // examine adjacent nodes
            foreach (var node in nodes)
            {
                if (visited.Contains(node))
                    continue;

                if (node.Equals(end))
                {
                    visited.Add(node);
                    paths.Add(new List<TNode>(visited));
                    visited.RemoveAt(visited.Count - 1);
                    break;
                }
            }
            // in breadth-first, recursion needs to come after visiting adjacent nodes
            foreach (var node in nodes)
            {
                if (visited.Contains(node) || node.Equals(end))
                    continue;
                visited.Add(node);
                Bfs(visited, end, paths);
                visited.RemoveAt(visited.Count - 1);
            }
        }
    }
}
