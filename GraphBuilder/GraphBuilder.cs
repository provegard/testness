// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphBuilder
{
    public class GraphBuilder<TNode> where TNode : class
    {
        private readonly Func<TNode, IEnumerable<TNode>> _headFinder;

        public GraphBuilder(Func<TNode, IEnumerable<TNode>> headFinder)
        {
            _headFinder = headFinder;
        }

        public Graph<TNode> Build(TNode root)
        {
            return Build(new[] {root});
        }

        public Graph<TNode> Build(IEnumerable<TNode> nodes)
        {
            var enumerator = nodes.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new ArgumentException("Empty node list.");
            }
            var first = enumerator.Current;

            IDictionary<TNode, IList<TNode>> digraph = new Dictionary<TNode, IList<TNode>>();
            do
            {
                Populate(digraph, enumerator.Current);
            } while (enumerator.MoveNext());

            return new Graph<TNode>(digraph, first);
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
