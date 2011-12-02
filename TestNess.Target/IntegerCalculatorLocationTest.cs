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
