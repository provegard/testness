﻿// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class ViolationTest
    {
        [TestCase]
        public void TestThatViolationExposesRule()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());
            var rule = new SomeRule();

            var violation = new Violation(rule, tc);

            Assert.AreSame(rule, violation.Rule);
        }

        [TestCase]
        public void TestThatViolationExposesTestCase()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());

            var violation = new Violation(new SomeRule(), tc);

            Assert.AreSame(tc, violation.TestCase);
        }

        [TestCase]
        public void TestThatToStringWithoutDebugSymbolsIncludesTypeAndMethod()
        {
            var tm = typeof (IntegerCalculatorTest).FindMethod("TestAddBasic()");
            var rule = new SomeRule();

            var violation = new Violation(rule, new TestCase(tm));

            const string expected = "TestNess.Target.IntegerCalculatorTest(TestAddBasic()): violation of \"some rule\"";
            Assert.AreEqual(expected, violation.ToString());
        }

        [TestCase]
        public void TestThatViolationExposesMessage()
        {
            var tm = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            var rule = new SomeRule();
            var violation = new Violation(rule, new TestCase(tm));

            Assert.AreEqual("violation of \"some rule\"", violation.Message);
        }

        [TestCase]
        public void TestThatMessageCanBeCustomized()
        {
            var tm = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            var rule = new SomeRule();
            var violation = new Violation(rule, new TestCase(tm), "a message");

            Assert.AreEqual("a message", violation.Message);
        }

        [TestCase]
        public void TestThatCustomizedMessageOccursInToString()
        {
            var tm = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            var rule = new SomeRule();
            var violation = new Violation(rule, new TestCase(tm), "a message");

            StringAssert.EndsWith(": a message", violation.ToString());
        }

        internal class SomeRule : IRule
        {
            public IEnumerable<Violation> Apply(TestCase testCase)
            {
                throw new NotImplementedException();
            }

            public override string ToString()
            {
                return "some rule";
            }
        }
    }
}
