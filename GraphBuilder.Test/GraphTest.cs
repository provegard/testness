﻿// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace GraphBuilder.Test
{
    [TestFixture]
    public class GraphTest
    {
        private static IEnumerable<SimpleNode> GetChildren(SimpleNode node)
        {
            return node.GetChildren();
        }

        [Test]
        public void TestThatGraphExposesRootNode()
        {
            var root = new SimpleNode("root");
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            Assert.AreSame(root, graph.Root);
        }

        [Test]
        public void TestThatGraphCanBeWalkedFromRoot()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var result = graph.Walk().Aggregate("", (str, node) => str + node.ToString());
            Assert.AreEqual("rootchild", result);
        }

        [Test]
        public void TestThatGraphWithCycleCanBeWalked()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            child.AddChild(root);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var result = graph.Walk().Aggregate("", (str, node) => str + node.ToString());
            Assert.AreEqual("rootchild", result);
        }

        [Test]
        public void TestThatWalkingIsDepthFirst()
        {
            var root = new SimpleNode("root");
            var child11 = new SimpleNode("child11");
            var child12 = new SimpleNode("child12");
            var child111 = new SimpleNode("child111");
            root.AddChild(child11);
            root.AddChild(child12);
            child11.AddChild(child111);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var result = graph.Walk().Aggregate("", (str, node) => str + node.ToString());
            Assert.AreEqual("rootchild11child111child12", result);
        }

        [Test]
        public void TestThatHeadsAreOrderedBasedOnInput()
        {
            var root = new SimpleNode("root");
            var child1 = new SimpleNode("child1");
            var child2 = new SimpleNode("child2");
            root.AddChild(child1);
            root.AddChild(child2);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            Assert.AreSame(child1, graph.HeadByIndex(root, 0));
        }

        [Test]
        public void TestThatWalkingCanStartWithAnyNode()
        {
            var root = new SimpleNode("root");
            var child11 = new SimpleNode("child11");
            var child111 = new SimpleNode("child111");
            root.AddChild(child11);
            child11.AddChild(child111);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var result = graph.Walk(child11).Aggregate("", (str, node) => str + node.ToString());
            Assert.AreEqual("child11child111", result);
        }

        [Test]
        public void TestThatRootHeadsAreReachableThroughEnumeration()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            CollectionAssert.Contains(new List<SimpleNode>(graph.HeadsFor(root)), child);
        }

        [Test]
        public void TestThatNodeTailsAreReachableThroughEnumeration()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            CollectionAssert.Contains(new List<SimpleNode>(graph.TailsFor(child)), root);
        }

        [Test]
        public void TestThatRootHeadsAreReachableThroughIndex()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            Assert.AreSame(child, graph.HeadByIndex(root, 0));
        }

        [Test]
        public void TestThatRootHasOutDegree()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            Assert.AreEqual(1, graph.OutDegreeOf(root));
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void TestThatRequestForHeadsForUnknownNodeThrows()
        {
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(new SimpleNode("root"));
            graph.HeadsFor(new SimpleNode("dummy")); // should throw
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void TestThatRequestForHeadByIndexForUnknownNodeThrows()
        {
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(new SimpleNode("root"));
            graph.HeadByIndex(new SimpleNode("dummy"), 0); // should throw
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void TestThatRequestForOutDegreeForUnknownNodeThrows()
        {
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(new SimpleNode("root"));
            graph.OutDegreeOf(new SimpleNode("dummy")); // should throw
        }

        [Test]
        public void ThatThatThereIsSinglePathBetweenNodeAndItself()
        {
            var root = new SimpleNode("root");
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var paths = graph.FindPaths(root, root);
            Assert.AreEqual("root", DescribePaths(paths));
        }

        [Test]
        public void ThatThatPathBetweenNodeAndChildContainsNodeAndChild()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var paths = graph.FindPaths(root, child);
            Assert.AreEqual("root child", DescribePaths(paths));
        }

        [Test]
        public void TestThatTwoPathsAreFoundInDiamondGraph()
        {
            var root = new SimpleNode("root");
            var child1 = new SimpleNode("child1");
            var child2 = new SimpleNode("child2");
            var end = new SimpleNode("end");
            root.AddChild(child1);
            root.AddChild(child2);
            child1.AddChild(end);
            child2.AddChild(end);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var paths = graph.FindPaths(root, end);
            Assert.AreEqual("root child1 end\nroot child2 end", DescribePaths(paths));
        }

        [Test]
        public void TestThatHeadsForRootWithTwoEdgesToChildIncludesChildTwice()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            Assert.AreEqual(2, graph.HeadsFor(root).Count());
        }

        [Test]
        public void TestThatTailsForChildWithTwoEdgesToRootIncludesRootTwice()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            Assert.AreEqual(2, graph.TailsFor(child).Count());
        }

        [Test]
        public void TestThatTwoPathsAreFoundForTwoEdgesBetweenVertices()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var paths = graph.FindPaths(root, child);
            Assert.AreEqual("root child\nroot child", DescribePaths(paths));
        }

        [Test]
        public void TestThatPathsCanBeFoundWhenThereIsACycle()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            var target = new SimpleNode("target");
            var back = new SimpleNode("back");
            
            root.AddChild(child);
            child.AddChild(target);
            child.AddChild(back);
            back.AddChild(root);

            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var paths = graph.FindPaths(root, target);

            var result = string.Join("|", paths.Select(path => string.Join(",", path)));
            Assert.AreEqual("root,child,target|root,child,back,root,child,target", result);
        }

        private static string DescribePaths(IEnumerable<IList<SimpleNode>> paths)
        {
            var strPaths = "";
            foreach (var path in paths)
            {
                var strPath = path.Aggregate("", (str, node) => str += node.ToString() + " ").TrimEnd();
                strPaths += strPath + "\n";
            }
            return strPaths.TrimEnd();
        }
    }
}