/**
 * Copyright (C) 2011-2012 by Per Rovegård (per@rovegard.se)
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
using TestNess.Lib.Analysis.Node;
using TestNess.Lib.Rule;
using GraphBuilder;

namespace TestNess.Lib.Analysis
{
    public class Analyzer
    {
        private Graph<AnalysisNode> _graph;

        public Analyzer(IEnumerable<TestCase> testCases, IEnumerable<IRule> rules)
        {
            TestCases = testCases;
            Rules = rules;
        }

        public IEnumerable<IRule> Rules { get; private set; }

        public IEnumerable<TestCase> TestCases { get; private set; }
        
        public IEnumerable<Violation> Violations
        {
            get
            {
                if (_graph == null)
                    return Enumerable.Empty<Violation>();
                return _graph.Root.GetViolations(_graph);
            }
        }

        public Graph<AnalysisNode> AnalysisTree
        {
            get { return _graph; }
        }

        public void Analyze()
        {
            // Tree data, node -> [node, node, ...]
            var dict = new Dictionary<AnalysisNode, IList<AnalysisNode>>();

            // Create a list of test case nodes. These are tree leaves.
            var tcNodes = TestCases.SelectMany(tc => Rules.Select(r => new TestCaseNode(tc, r))).ToList();
            tcNodes.ForEach(tc => dict.Add(tc, new List<AnalysisNode>()));

            // Get the assembly (for the root node), from one of the test cases.
            //TODO: handle empty node list here
            var ass = tcNodes.First().TestCase.TestMethod.DeclaringType.Module.Assembly;

            var typeNodeGroups = tcNodes.GroupBy(n => new TypeNode(n.TestCase.TestMethod.DeclaringType)).ToList();
            typeNodeGroups.ForEach(g => dict.Add(g.Key, g.OfType<AnalysisNode>().ToList()));

            var namespaceNodeGroups = typeNodeGroups.Select(g => g.Key).GroupBy(tn => new NamespaceNode(tn.Type.Namespace), tn => (AnalysisNode) tn).ToList();
            while (namespaceNodeGroups.Any(g => g.Key.Name.Contains(".")))
            {
                namespaceNodeGroups.ForEach(g => dict.Add(g.Key, g.ToList()));
                namespaceNodeGroups = namespaceNodeGroups.Select(g => g.Key).GroupBy(NodeForParentNamespace, ns => (AnalysisNode) ns).ToList();
            }
            namespaceNodeGroups.ForEach(g => dict.Add(g.Key, g.ToList()));

            var assNode = new AssemblyNode(ass);
            dict.Add(assNode, namespaceNodeGroups.Select(g => (AnalysisNode) g.Key).ToList());

            _graph = new Graph<AnalysisNode>(dict, assNode);
        }

        private NamespaceNode NodeForParentNamespace(NamespaceNode node)
        {
            //TODO: verify that this works with different-length namespaces!
            var name = node.Name;
            var idx = name.LastIndexOf(".", StringComparison.Ordinal);
            if (idx < 0)
                return node;
            var parentName = name.Substring(0, idx);
            return new NamespaceNode(parentName);
        }
    }
}
