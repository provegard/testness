// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNess.Target
{
    [TestClass]
    public class IntegerCalculatorLocationTest
    {
        // DO NOT ALTER THIS CLASS, ONLY ADD - LINE AND COLUMN NUMBERS MUST NOT CHANGE
        [TestMethod]
        public void TestAdd()
        {
            var calculator = 
                new IntegerCalculator();
            Assert.AreEqual(3, calculator.Add(1, 2));
        }

        [TestMethod]
        public void TestAddWithExternallyCalculatedExpectation()
        {
            var actual = new IntegerCalculator().Add(1, 2);
            var expected = Add(1, 2);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAddWithManuallyComparedExternallyCalculatedExpectation()
        {
            var actual = new IntegerCalculator().Add(1, 2);
            var expected = Add(1, 2);

            Assert.IsTrue(actual == expected, "Comparison failed");
        }

        private int Add(int i, int j)
        {
            return i + j;
        }
    }
}
