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
    public class IntegerCalculatorReturnValueTest
    {
        private IntegerCalculator _calc;

        [TestInitialize]
        public void GivenACalculator()
        {
            _calc = new IntegerCalculator();
        }

        [TestMethod]
        public void TestAddWithUnhandledReturnValueFromStaticMethod()
        {
            StaticMethod();
            Assert.AreEqual(2, _calc.Add(1, 1));
        }

        [TestMethod]
        public void TestAddWithDoubleUnhandledReturnValueFromStaticMethod()
        {
            StaticMethod();
            StaticMethod();
            Assert.AreEqual(2, _calc.Add(1, 1));
        }

        [TestMethod]
        public void TestAddWithUnhandledReturnValueFromInstanceMethod()
        {
            InstanceMethod();
            Assert.AreEqual(2, _calc.Add(1, 1));
        }

        [TestMethod]
        public void TestAddWithUnhandledReturnValueFromVirtualMethod()
        {
            VirtualMethod();
            Assert.AreEqual(2, _calc.Add(1, 1));
        }

        [TestMethod]
        public void TestAddWithCallToNonReturningMethod()
        {
            NonReturningMethod();
            Assert.AreEqual(2, _calc.Add(1, 1));
        }

        [TestMethod]
        public void TestMultiplyWithUsedReturnValueFromStaticMethod()
        {
            var value = StaticMethod();
            Assert.AreEqual(0, _calc.Multiply(value, 0));
        }

        [TestMethod, ExpectedException(typeof(DivideByZeroException))]
        public void TestDivideWithException()
        {
            _calc.Divide(5, 0); // should throw
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
    }
}
