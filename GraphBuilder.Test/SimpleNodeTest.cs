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
