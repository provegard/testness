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
        [TestCase]
        public void TestNodeHasValue()
        {
            var sn = new SimpleNode("test");
            Assert.AreEqual("test", sn.Value);
        }

        [TestCase]
        public void TestNodeToStringIsValue()
        {
            var sn = new SimpleNode("test");
            Assert.AreEqual("test", sn.ToString());
        }

        [TestCase]
        public void TestNodesWithSameValueAreEqual()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test");
            Assert.AreEqual(sn1, sn2);
        }

        [TestCase]
        public void TestNodesWithDifferentValuesAreNotEqual()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test2");
            Assert.AreNotEqual(sn1, sn2);
        }

        [TestCase]
        public void TestNodesWithSameValueHaveSameHashCode()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test");
            Assert.AreEqual(sn1.GetHashCode(), sn2.GetHashCode());
        }

        [TestCase]
        public void TestNodesWithDifferentValuesHaveDifferentHashCodes()
        {
            var sn1 = new SimpleNode("test");
            var sn2 = new SimpleNode("test2");
            Assert.AreNotEqual(sn1.GetHashCode(), sn2.GetHashCode());
        }

        [TestCase]
        public void TestChildListIsInitiallyEmpty()
        {
            var sn1 = new SimpleNode("test");
            Assert.AreEqual(0, sn1.GetChildren().Count());
        }

        [TestCase]
        public void TestAddedNodeIsReturnedInChildEnumeration()
        {
            var child = new SimpleNode("child");
            var sn1 = new SimpleNode("test");
            sn1.AddChild(child);
            CollectionAssert.Contains(new List<SimpleNode>(sn1.GetChildren()), child);
        }
    }
}
