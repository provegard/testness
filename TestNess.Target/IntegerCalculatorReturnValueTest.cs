// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
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
