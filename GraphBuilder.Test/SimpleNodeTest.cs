using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphBuilder.Test
{
    [TestClass]
    public class SimpleNodeTest
    {
        [TestMethod]
        public void TestNodeHasValue()
        {
            var sn = new SimpleNode("test");
            Assert.AreEqual("test", sn.Value);
        }

        [TestMethod]
        public void TestNodeToStringIsValue()
        {
            var sn = new SimpleNode("test");
            Assert.AreEqual("test", sn.ToString());
        }

        [TestMethod]
        public void TestNodesWithSameValueAreEqual()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test");
            Assert.AreEqual(sn1, sn2);
        }

        [TestMethod]
        public void TestNodesWithDifferentValuesAreNotEqual()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test2");
            Assert.AreNotEqual(sn1, sn2);
        }

        [TestMethod]
        public void TestNodesWithSameValueHaveSameHashCode()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test");
            Assert.AreEqual(sn1.GetHashCode(), sn2.GetHashCode());
        }

        [TestMethod]
        public void TestNodesWithDifferentValuesHaveDifferentHashCodes()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test2");
            Assert.AreNotEqual(sn1.GetHashCode(), sn2.GetHashCode());
        }

        [TestMethod]
        public void TestChildListIsInitiallyEmpty()
        {
            var sn1 = new SimpleNode("test");
            Assert.AreEqual(0, sn1.GetChildren().Count());
        }

        [TestMethod]
        public void TestAddedNodeIsReturnedInChildEnumeration()
        {
            var child = new SimpleNode("child");
            var sn1 = new SimpleNode("test");
            sn1.AddChild(child);
            CollectionAssert.Contains(new List<SimpleNode>(sn1.GetChildren()), child);
        }
    }
}
