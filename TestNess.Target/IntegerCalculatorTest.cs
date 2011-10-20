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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNess.Target
{
    [TestClass]
    public class IntegerCalculatorTest
    {
        [TestMethod]
        public void TestAddWithConditionalAndMultiAssert()
        {
            var calculator = new IntegerCalculator();

            if (DateTime.Now.Ticks > 0)
            {
                Assert.AreEqual(3, calculator.Add(1, 2));
                Assert.AreEqual(3, calculator.Add(0, 3));
            }
        }

        [TestMethod]
        public void TestAddWithIf()
        {
            var calculator = new IntegerCalculator();

            if (DateTime.Now.Ticks > 0)
            {
                Assert.AreEqual(3, calculator.Add(1, 2));
            }
        }

        [TestMethod]
        public void TestAddWithFor()
        {
            var calculator = new IntegerCalculator();

            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual(0, calculator.Add(i, -i));
            }
        }

        [TestMethod]
        public void TestAddWithWhile()
        {
            var calculator = new IntegerCalculator();

            var i = 0;
            while (i < 3)
            {
                Assert.AreEqual(0, calculator.Add(i, -i));
                i++;
            }
        }

        [TestMethod]
        public void TestAddWithDoWhile()
        {
            var calculator = new IntegerCalculator();

            var i = 0;
            do
            {
                Assert.AreEqual(0, calculator.Add(i, -i));
                i++;
            } while (i < 3);
        }

        [TestMethod]
        public void TestAddWithSwitchCase()
        {
            var calculator = new IntegerCalculator();

            var i = new Random().Next(2);
            switch (i)
            {
                case 0:
                    Assert.AreEqual(3, calculator.Add(1, 2));
                    break;
                case 1:
                    Assert.AreEqual(10, calculator.Add(5, 5));
                    break;
            }
        }

        [TestMethod]
        public void TestAddTwoAsserts()
        {
            var calculator = new IntegerCalculator();

            Assert.AreEqual(3, calculator.Add(1, 2));
            Assert.AreEqual(10, calculator.Add(1, 9));
        }

        [TestMethod]
        public void TestAddBasic()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);

            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void TestSubtractBasic()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Subtract(1, 2);

            Assert.AreEqual(-1, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestDivideWithException()
        {
            var calculator = new IntegerCalculator();
            calculator.Divide(5, 0); // should throw
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestMultiAssertWithExpectedException()
        {
            var calculator = new IntegerCalculator();
            Assert.AreEqual(1, calculator.Divide(1, 1));
            Assert.AreEqual(2, calculator.Divide(4, 2));
            calculator.Divide(5, 0); // should throw
        }

        [TestMethod]
        public void DivideByZeroWithTryCatch()
        {
            var calculator = new IntegerCalculator();
            try
            {
                calculator.Divide(5, 0); // should throw
                Assert.Fail("Should've thrown!");
            }
            catch (DivideByZeroException)
            {
                // expected
            }
        }
    }
}
