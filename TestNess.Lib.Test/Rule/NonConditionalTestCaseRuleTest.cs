// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using System.Linq;
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Lib.TestFramework;
using TestNess.Target;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class NonConditionalTestCaseRuleTest : AbstractRuleTest<NonConditionalTestCaseRule, IntegerCalculatorConditionalTest>
    {
        [TestCase("TestAddNonConditional()", 0)]
        [TestCase("TestAddWithIf()", 1)]
        [TestCase("TestAddWithFor()", 1)]
        [TestCase("TestAddWithWhile()", 1)]
        [TestCase("TestAddWithDoWhile()", 1)]
        [TestCase("TestAddWithSwitchCase()", 1)]
        [TestCase("TestAddWithIf()", 1)]
        public void TestViolationCountForDifferentMethods(string method, int expectedViolationCount)
        {
            var violations = FindViolations(method);
            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [Test]
        public void TestThatToStringDescribesRule()
        {
            var rule = new NonConditionalTestCaseRule();
            Assert.AreEqual("a test case should not be conditional", rule.ToString());
        }

        [Test]
        public void TestThatUseOfLinqIsntSeenAsViolation()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorConditionalTest>(_ => _.ATestWithNonConditionalLinq());
            var rule = new NonConditionalTestCaseRule();
            var violations = rule.Apply(tc);
            Assert.AreEqual(0, violations.Count());
        }

        [Test]
        public void ItShouldNotComplainAboutConditionalSetup()
        {
            Action a = () =>
            {
                var actual = new Random().Next(10) < 5 ? "foo" : "bar";
                Assert.AreNotEqual(actual, "baz");
            };
            var rule = new NonConditionalTestCaseRule();
            var violations = rule.Apply(a.AsTestCase(new NUnitTestFramework()));
            Assert.AreEqual(0, violations.Count());
        }
    }
}
