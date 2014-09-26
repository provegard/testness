// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GraphBuilder.Test
{
    [TestFixture]
    public class SimpleNodeTest
    {
        [Test]
        public void TestNodeHasValue()
        {
            var sn = new SimpleNode("test");
            Assert.AreEqual("test", sn.Value);
        }

        [Test]
        public void TestNodeToStringIsValue()
        {
            var sn = new SimpleNode("test");
            Assert.AreEqual("test", sn.ToString());
        }

        [Test]
        public void TestNodesWithSameValueAreEqual()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test");
            Assert.AreEqual(sn1, sn2);
        }

        [Test]
        public void TestNodesWithDifferentValuesAreNotEqual()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test2");
            Assert.AreNotEqual(sn1, sn2);
        }

        [Test]
        public void TestNodesWithSameValueHaveSameHashCode()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test");
            Assert.AreEqual(sn1.GetHashCode(), sn2.GetHashCode());
        }

        [Test]
        public void TestNodesWithDifferentValuesHaveDifferentHashCodes()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test2");
            Assert.AreNotEqual(sn1.GetHashCode(), sn2.GetHashCode());
        }

        [Test]
        public void TestChildListIsInitiallyEmpty()
        {
            var sn1 = new SimpleNode("test");
            Assert.AreEqual(0, sn1.GetChildren().Count());
        }

        [Test]
        public void TestAddedNodeIsReturnedInChildEnumeration()
        {
            var child = new SimpleNode("child");
            var sn1 = new SimpleNode("test");
            sn1.AddChild(child);
            CollectionAssert.Contains(new List<SimpleNode>(sn1.GetChildren()), child);
        }
    }
}
