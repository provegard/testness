// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
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
        [Test]
        public void TestThatViolationExposesRule()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());
            var rule = new SomeRule();

            var violation = new Violation(rule, tc);

            Assert.AreSame(rule, violation.Rule);
        }

        [Test]
        public void TestThatViolationExposesTestCase()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());

            var violation = new Violation(new SomeRule(), tc);

            Assert.AreSame(tc, violation.TestCase);
        }

        [Test]
        public void TestThatToStringWithoutDebugSymbolsIncludesTypeAndMethod()
        {
            var tm = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            var rule = new SomeRule();

            var violation = new Violation(rule, new TestCase(tm, null));

            const string expected = "TestNess.Target.IntegerCalculatorTest(TestAddBasic()): violation of \"some rule\"";
            Assert.AreEqual(expected, violation.ToString());
        }

        [Test]
        public void TestThatViolationExposesMessage()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(x => x.TestAddBasic());
            var rule = new SomeRule();
            var violation = new Violation(rule, tc);

            Assert.AreEqual("violation of \"some rule\"", violation.Message);
        }

        [Test]
        public void TestThatMessageCanBeCustomized()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());
            var rule = new SomeRule();
            var violation = new Violation(rule, tc, "a message");

            Assert.AreEqual("a message", violation.Message);
        }

        [Test]
        public void TestThatCustomizedMessageOccursInToString()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());
            var rule = new SomeRule();
            var violation = new Violation(rule, tc, "a message");

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
