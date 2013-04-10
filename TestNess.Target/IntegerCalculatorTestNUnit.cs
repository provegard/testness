// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using NUnit.Framework;

namespace TestNess.Target
{
    [TestFixture]
    public class IntegerCalculatorTestNUnit
    {
        //[Test]
        //public void TestAddTwoAsserts()
        //{
        //    var calculator = new IntegerCalculator();

        //    Assert.AreEqual(3, calculator.Add(1, 2));
        //    Assert.AreEqual(10, calculator.Add(1, 9));
        //}

        [TestCase(1, 2, 3)]
        public void TestDataDriven(int a, int b, int sum)
        {
            var calculator = new IntegerCalculator();
            Assert.AreEqual(sum, calculator.Add(a, b));
        }

        [Test]
        public void TestAddTwoAssertsCalledInHelper()
        {
            var calculator = new IntegerCalculator();
            AssertAddition(calculator);
        }

        private void AssertAddition(IntegerCalculator calculator)
        {
            Assert.AreEqual(3, calculator.Add(1, 2));
            Assert.AreEqual(10, calculator.Add(1, 9));
        }

        [Test]
        public void TestAddBasic()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);

            Assert.AreEqual(3, actual);
        }

        [Test]
        public void TestSubtractBasic()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Subtract(1, 2);

            Assert.AreEqual(-1, actual);
        }

        [Test]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestDivideWithException()
        {
            var calculator = new IntegerCalculator();
            calculator.Divide(5, 0); // should throw
        }

        //[Test]
        //[ExpectedException(typeof(DivideByZeroException))]
        //public void TestMultiAssertWithExpectedException()
        //{
        //    var calculator = new IntegerCalculator();
        //    Assert.AreEqual(1, calculator.Divide(1, 1));
        //    Assert.AreEqual(2, calculator.Divide(4, 2));
        //    calculator.Divide(5, 0); // should throw
        //}

        //[Test]
        //public void DivideByZeroWithTryCatch()
        //{
        //    var calculator = new IntegerCalculator();
        //    try
        //    {
        //        calculator.Divide(5, 0); // should throw
        //        Assert.Fail("Should've thrown!");
        //    }
        //    catch (DivideByZeroException)
        //    {
        //        // expected
        //    }
        //}
    }
}
