// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using NUnit.Framework;

namespace GraphBuilder.Test
{
    [TestFixture]
    public class GraphBuilderTest
    {
        private static IEnumerable<SimpleNode> GetChildren(SimpleNode node)
        {
            return node.GetChildren();
        }

        [TestCase]
        public void TestThatGraphCanBeCreatedWithSingleValueNode()
        {
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(new SimpleNode("root"));
            Assert.AreEqual(1, graph.Order);
        }

        [TestCase]
        public void TestThatGraphCanBeCreatedWithTwoConnectedNodes()
        {
            var root = new SimpleNode("root");
            root.AddChild(new SimpleNode("child"));
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            Assert.AreEqual(2, graph.Order);
        }

        [TestCase]
        public void TestThatBuilderHandlesCycle()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            child.AddChild(root);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            Assert.AreEqual(2, graph.Order);
        }
    }
}
