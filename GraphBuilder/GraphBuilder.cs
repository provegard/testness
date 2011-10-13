﻿/**
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

using System.Collections.Generic;
using System.Linq;

namespace GraphBuilder
{
    public class GraphBuilder<TNode> where TNode : class
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
