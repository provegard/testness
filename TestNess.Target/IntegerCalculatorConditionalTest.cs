// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNess.Target
{
    [TestClass]
    public class IntegerCalculatorConditionalTest
    {
        private IntegerCalculator _calc;

        [TestInitialize]
        public void GivenACalculator()
        {
            _calc = new IntegerCalculator();
        }

        [TestMethod]
        public void TestAddNonConditional()
        {
            Assert.AreEqual(3, _calc.Add(1, 2));
        }

        [TestMethod]
        public void TestAddWithConditionalAndMultiAssert()
        {
            if (DateTime.Now.Ticks > 0)
            {
                Assert.AreEqual(3, _calc.Add(1, 2));
                Assert.AreEqual(3, _calc.Add(0, 3));
            }
        }

        [TestMethod]
        public void TestAddWithIf()
        {
            if (DateTime.Now.Ticks > 0)
            {
                Assert.AreEqual(3, _calc.Add(1, 2));
            }
        }

        [TestMethod]
        public void TestAddWithFor()
        {
            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual(0, _calc.Add(i, -i));
            }
        }

        [TestMethod]
        public void TestAddWithWhile()
        {
            var i = 0;
            while (i < 3)
            {
                Assert.AreEqual(0, _calc.Add(i, -i));
                i++;
            }
        }

        [TestMethod]
        public void TestAddWithDoWhile()
        {
            var i = 0;
            do
            {
                Assert.AreEqual(0, _calc.Add(i, -i));
                i++;
            } while (i < 3);
        }

        [TestMethod]
        public void TestAddWithSwitchCase()
        {
            var i = new Random().Next(2);
            switch (i)
            {
                case 0:
                    Assert.AreEqual(3, _calc.Add(1, 2));
                    break;
                case 1:
                    Assert.AreEqual(10, _calc.Add(5, 5));
                    break;
            }
        }
    }
}
