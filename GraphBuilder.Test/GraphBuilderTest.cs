using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphBuilder.Test
{
    [TestClass]
    public class GraphBuilderTest
    {
        private static IEnumerable<SimpleNode> GetChildren(SimpleNode node)
        {
            return node.GetChildren();
        }

        [TestMethod]
        public void TestThatGraphCanBeCreatedWithSingleValueNode()
        {
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(new SimpleNode("root"));
            Assert.AreEqual(1, graph.Order);
        }

        [TestMethod]
        public void TestThatGraphExposesRootNode()
        {
            var root = new SimpleNode("root");
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            Assert.AreSame(root, graph.Root);
        }

        [TestMethod]
        public void TestThatGraphCanBeCreatedWithTwoConnectedNodes()
        {
            var root = new SimpleNode("root");
            root.AddChild(new SimpleNode("child"));
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            Assert.AreEqual(2, graph.Order);
        }

        [TestMethod]
        public void TestThatRootHeadsAreReachableThroughEnumeration()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            CollectionAssert.Contains(new List<SimpleNode>(graph.HeadsFor(root)), child);
        }

        [TestMethod]
        public void TestThatRootHeadsAreReachableThroughIndex()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            Assert.AreSame(child, graph.HeadByIndex(root, 0));
        }

        [TestMethod]
        public void TestThatRootHasOutDegree()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            Assert.AreEqual(1, graph.OutDegreeOf(root));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestThatRequestForHeadsForUnknownNodeThrows()
        {
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(new SimpleNode("root"));
            graph.HeadsFor(new SimpleNode("dummy")); // should throw
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestThatRequestForHeadByIndexForUnknownNodeThrows()
        {
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(new SimpleNode("root"));
            graph.HeadByIndex(new SimpleNode("dummy"), 0); // should throw
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestThatRequestForOutDegreeForUnknownNodeThrows()
        {
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(new SimpleNode("root"));
            graph.OutDegreeOf(new SimpleNode("dummy")); // should throw
        }

        [TestMethod]
        public void TestThatBuilderHandlesCycle()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            child.AddChild(root);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);
            Assert.AreEqual(2, graph.Order);
        }

        [TestMethod]
        public void TestThatGraphCanBeWalkedFromRoot()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var result = "";
            graph.Walk(node => result += node.ToString());
            Assert.AreEqual("rootchild", result);
        }

        [TestMethod]
        public void TestThatGraphWithCycleCanBeWalked()
        {
            var root = new SimpleNode("root");
            var child = new SimpleNode("child");
            root.AddChild(child);
            child.AddChild(root);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var result = "";
            graph.Walk(node => result += node.ToString());
            Assert.AreEqual("rootchild", result);
        }

        [TestMethod]
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

            var result = "";
            graph.Walk(node => result += node.ToString());
            Assert.AreEqual("rootchild11child111child12", result);
        }

        [TestMethod]
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

        [TestMethod]
        public void TestThatWalkingCanStartWithAnyNode()
        {
            var root = new SimpleNode("root");
            var child11 = new SimpleNode("child11");
            var child111 = new SimpleNode("child111");
            root.AddChild(child11);
            child11.AddChild(child111);
            var graph = new GraphBuilder<SimpleNode>(GetChildren).Build(root);

            var result = "";
            graph.Walk(node => result += node.ToString(), child11);
            Assert.AreEqual("child11child111", result);
        }
    }
}
