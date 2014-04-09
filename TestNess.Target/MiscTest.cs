// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNess.Target
{
    [TestClass]
    public class MiscTest
    {
        [TestMethod]
        public void ObjectConstruction()
        {
            var zero = new Decimal(0);
            Assert.AreEqual(zero, 0m);
        }

        [TestMethod]
        public void UseOfTypeOf()
        {
            Assert.AreEqual(typeof (MiscTest), GetType());
        }
    }
}
