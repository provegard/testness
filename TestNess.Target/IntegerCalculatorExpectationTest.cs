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
    public class IntegerCalculatorExpectationTest
    {
        private static readonly int Eight = 8;
        private const int Nine = 9;
        private int _one = 1;

        private IntegerCalculator _calc;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void GivenACalculator()
        {
            _calc = new IntegerCalculator();
        }

        [TestMethod, Ignore]
        public void TestWithUnconditionalFailure()
        {
            Assert.Fail("This fails!");
        }

        [TestMethod]
        public void TestAddWithLiteralExpectation()
        {
            Assert.AreEqual(3, _calc.Add(1, 2));
        }

        [TestMethod]
        public void TestAddWithConstExpectation()
        {
            var actual = _calc.Add(4, 5);

            // There is no ldc.i4.9
            Assert.AreEqual(Nine, actual);
        }

        [TestMethod]
        public void TestAddWithStaticReadonlyExpectation()
        {
            var actual = _calc.Add(3, 5);

            Assert.AreEqual(Eight, actual);
        }

        [TestMethod]
        public void TestAddWith9To32BitConstExpectation()
        {
            const long expectation = 512L;
            var actual = _calc.Add(256, 256);

            // Should result in ldc.i4 + conv.i8
            Assert.AreEqual(expectation, actual);
        }

        [TestMethod]
        public void TestAddWith8OrFewerBitConstExpectation()
        {
            const long expectation = 128L;
            var actual = _calc.Add(64, 64);

            // Should result in ldc.i4.s + conv.i8
            Assert.AreEqual(expectation, actual);
        }

        [TestMethod]
        public void TestAddWithSwitchedActualAndExpectation()
        {
            var actual = _calc.Add(1, 2);

            Assert.AreEqual(actual, 3);
        }

        [TestMethod]
        public void TestAddWithManuallyComparedConstantExpectation()
        {
            var actual = _calc.Add(1, 2);

            Assert.IsTrue(actual == 3, "Comparison failed");
        }

        [TestMethod]
        public void TestAddWithManuallyComparedExternallyCalculatedExpectation()
        {
            var actual = _calc.Add(1, 2);
            var expected = Add(1, 2);

            Assert.IsTrue(actual == expected, "Comparison failed");
        }

        [TestMethod]
        public void TestAddWithLteComparedExternallyCalculatedExpectation()
        {
            var actual = _calc.Add(1, 2);
            var bigger = Add(2, 2);

            // <= results in two CIL comparisons, cgt followed by ceq!
            Assert.IsTrue(actual <= bigger, "Comparison failed");
        }

        [TestMethod]
        public void TestAddWithNeqComparedExternallyCalculatedExpectation()
        {
            var actual = _calc.Add(1, 2);
            var expected = Add(1, 2);

            // != results in two CIL comparisons, ceq followed by ceq!
            Assert.IsFalse(actual != expected, "Comparison failed");
        }

        [TestMethod]
        public void TestAddWithLocallyCalculatedExpectation()
        {
            var actual = _calc.Add(1, 2);

            Assert.AreEqual(1 + 2, actual);
        }

        [TestMethod]
        public void TestAddWithExternallyCalculatedExpectation()
        {
            var actual = _calc.Add(1, 2);
            var expected = Add(1, 2);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAddWithExpectationFromProperty()
        {
            var actual = _calc.Add(1, 2);
            var expected = new Adder(1, 2).Result;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAddWithExpectationDerivedFromField()
        {
            var actual = _calc.Add(1, 2);
            var expected = _one + 2;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAddWithMultipleExpectationViolations()
        {
            var actual = _calc.Add(1, 2);
            var expected = Add(1, 2);

            // Two uses of a calculated expected value
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAddWithConditionalExpectationViolations()
        {
            var actual = _calc.Add(1, 2);
            int expected;
            // Two paths, two expected value calculations
            if (new Random().Next(2) == 0)
                expected = Add(1, 2);
            else
                expected = Add(0, 3);
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

        private int Add(int i1, int i2)
        {
            return i1 + i2;
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
