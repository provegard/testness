// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNess.Target
{
    [TestClass]
    public class IntegerCalculatorDistributionTest
    {
        [TestMethod]
        public void TestAddWithSingleAssert()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);

            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void TestAddWithSpreadAsserts()
        {
            var calculator = new IntegerCalculator();
            Assert.IsNotNull(calculator);

            var actual = calculator.Add(1, 2);

            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void TestAddWithAssertsInTheMiddle()
        {
            var calculator = new IntegerCalculator();

            Assert.IsNotNull(calculator);
            Assert.AreEqual(3, calculator.Add(1, 2));

            calculator.Multiply(4, 4);
        }

        [TestMethod]
        public void TestAddWithWhitespaceSeparatedAssertsTowardsTheEnd()
        {
            var calculator = new IntegerCalculator();

            Assert.AreEqual(1, calculator.Divide(1, 1));


            // lots and lots of ws here...



            Assert.AreEqual(2, calculator.Divide(4, 2));
        }
    }
}
