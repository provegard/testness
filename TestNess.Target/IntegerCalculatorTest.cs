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
        private static readonly int Eight = 8;
        private const int Nine = 9;
        private int _one = 1;

        public TestContext TestContext { get; set; }

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

        [TestMethod]
        public void TestMultiplyWithUsedReturnValueFromStaticMethod()
        {
            var calculator = new IntegerCalculator();
            var value = StaticMethod();
            Assert.AreEqual(0, calculator.Multiply(value, 0));
        }

        [TestMethod]
        public void TestAddWithUnhandledReturnValueFromStaticMethod()
        {
            var calculator = new IntegerCalculator();
            StaticMethod();
            Assert.AreEqual(2, calculator.Add(1, 1));
        }

        [TestMethod]
        public void TestAddWithUnhandledReturnValueFromInstanceMethod()
        {
            var calculator = new IntegerCalculator();
            InstanceMethod();
            Assert.AreEqual(2, calculator.Add(1, 1));
        }

        [TestMethod]
        public void TestAddWithUnhandledReturnValueFromVirtualMethod()
        {
            var calculator = new IntegerCalculator();
            VirtualMethod();
            Assert.AreEqual(2, calculator.Add(1, 1));
        }

        [TestMethod]
        public void TestAddWithCallToNonReturningMethod()
        {
            var calculator = new IntegerCalculator();
            NonReturningMethod();
            Assert.AreEqual(2, calculator.Add(1, 1));
        }

        [TestMethod]
        public void TestAddWithLocallyCalculatedExpectation()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);

            Assert.AreEqual(1 + 2, actual);
        }

        [TestMethod]
        public void TestAddWithExternallyCalculatedExpectation()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);
            var expected = Add(1, 2);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAddWithExpectationFromProperty()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);
            var expected = new Adder(1, 2).Result;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAddWithExpectationDerivedFromField()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);
            var expected = _one + 2;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DeploymentItem("TestNess.Target\\AddTestData.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
                           "|DataDirectory|\\AddTestData.xml",
                           "Row",
                            DataAccessMethod.Sequential)]
        public void TestDataDrivenAdd()
        {
            var x = Convert.ToInt32(TestContext.DataRow["x"]);
            var y = Convert.ToInt32(TestContext.DataRow["y"]);
            var expected = Convert.ToInt32(TestContext.DataRow["expected"]);

            var calculator = new IntegerCalculator();
            var actual = calculator.Add(x, y);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAddWithConstExpectation()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(4, 5);

            // There is no ldc.i4.9
            Assert.AreEqual(Nine, actual);
        }

        [TestMethod]
        public void TestAddWithStaticReadonlyExpectation()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(3, 5);

            Assert.AreEqual(Eight, actual);
        }

        [TestMethod]
        public void TestAddWith9To32BitConstExpectation()
        {
            const long expectation = 512L;
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(256, 256);

            // Should result in ldc.i4 + conv.i8
            Assert.AreEqual(expectation, actual);
        }

        [TestMethod]
        public void TestAddWith8OrFewerBitConstExpectation()
        {
            const long expectation = 128L;
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(64, 64);

            // Should result in ldc.i4.s + conv.i8
            Assert.AreEqual(expectation, actual);
        }

        [TestMethod]
        public void TestAddWithSwitchedActualAndExpectation()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);

            Assert.AreEqual(actual, 3);
        }

        [TestMethod]
        public void TestAddWithManuallyComparedConstantExpectation()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);

            Assert.IsTrue(actual == 3, "Comparison failed");
        }

        [TestMethod]
        public void TestAddWithManuallyComparedExternallyCalculatedExpectation()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);
            var expected = Add(1, 2);

            Assert.IsTrue(actual == expected, "Comparison failed");
        }

        [TestMethod]
        public void TestAddWithLteComparedExternallyCalculatedExpectation()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);
            var bigger = Add(2, 2);

            // <= results in two CIL comparisons, cgt followed by ceq!
            Assert.IsTrue(actual <= bigger, "Comparison failed");
        }

        [TestMethod]
        public void TestAddWithNeqComparedExternallyCalculatedExpectation()
        {
            var calculator = new IntegerCalculator();
            var actual = calculator.Add(1, 2);
            var expected = Add(1, 2);

            // != results in two CIL comparisons, ceq followed by ceq!
            Assert.IsFalse(actual != expected, "Comparison failed");
        }


        private int Add(int i1, int i2)
        {
            return i1 + i2;
        }

        public int InstanceMethod()
        {
            return new Random().Next();
        }

        public virtual int VirtualMethod()
        {
            return new Random().Next();
        }

        public static int StaticMethod()
        {
            return new Random().Next();
        }

        public static void NonReturningMethod()
        {
            StaticMethod();
        }

        private class Adder
        {
            internal int Result { get; private set; }

            internal Adder(int a, int b)
            {
                Result = a + b;
            }
        }
    }
}
