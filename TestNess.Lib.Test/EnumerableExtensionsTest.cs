// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using NUnit.Framework;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void EndsWithShouldHandleHappyCase()
        {
            var coll = new[] {1, 2, 3};
            var tail = new[] {2, 3};
            Assert.True(coll.EndsWith(tail));
        }

        [Test]
        public void EndsWithShouldHandleListsWithSameLength()
        {
            var coll = new[] { 2, 3 };
            var tail = new[] { 2, 3 };
            Assert.True(coll.EndsWith(tail));
        }

        [Test]
        public void EndsWithShouldFailIfTailIsLonger()
        {
            var coll = new[] { 2, 3 };
            var tail = new[] { 2, 3, 4 };
            Assert.False(coll.EndsWith(tail));
        }

        [Test]
        public void EndsWithShouldFailIfTailDoesntMatch()
        {
            var coll = new[] { 1, 2, 3 };
            var tail = new[] { 2, 4 };
            Assert.False(coll.EndsWith(tail));
        }

        [Test]
        public void EndsWithShouldHandleNulls()
        {
            var coll = new[] { "a", "b", "c" };
            var tail = new[] { "b", null };
            Assert.False(coll.EndsWith(tail));
        }
    }
}
