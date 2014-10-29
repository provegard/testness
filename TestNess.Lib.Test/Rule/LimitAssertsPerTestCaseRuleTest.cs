// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Linq;
using TestNess.Lib.Rule;
using TestNess.Target;
using NUnit.Framework;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class LimitAssertsPerTestCaseRuleTest : AbstractRuleTest<LimitAssertsPerTestCaseRule, IntegerCalculatorTest>
    {
        [TestCase("TestAddBasic()", 0)]
        [TestCase("TestAddTwoAsserts()", 1)]
        [TestCase("TestDivideWithException()", 0, Description = "No assert needed when there is an expected exception!")]
        [TestCase("TestMultiAssertWithExpectedException()", 1, Description = "Violation due to multiple asserts, expected exception doesn't change the picture!")]
        [TestCase("TestAddTwoAssertsCalledInHelper()", 1, Description = "Multiple asserts cannot be hidden in a helper!")]
        public void TestViolationCountForDifferentMethods(string method, int expectedViolationCount)
        {
            var violations = FindViolations(method);
            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [Test]
        public void TestThatViolationMessageContainsTestCaseSpecificInformation()
        {
            var violation = FindViolations("TestAddTwoAsserts()").First();
            Assert.AreEqual("test case contains 2 asserts (limit is 1)", violation.Message);
        }

        [Test]
        public void TestThatToStringDescribesRule()
        {
            var rule = new LimitAssertsPerTestCaseRule();
            Assert.AreEqual("a test case should have 1 assert or expect an exception", rule.ToString());
        }

        [Test]
        public void TestThatToStringDescribesConfiguredRule()
        {
            var rule = new LimitAssertsPerTestCaseRule { MaxNumberOfAsserts = 2 };
            Assert.AreEqual("a test case should have 1 to 2 asserts or expect an exception", rule.ToString());
        }


        [Test]
        public void TestThatRuleCanBeConfiguredWithAcceptedNumberOfAsserts()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddTwoAsserts());
            var rule = new LimitAssertsPerTestCaseRule { MaxNumberOfAsserts = 2 };

            Assert.AreEqual(0, rule.Apply(tc).Count());
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestThatRuleCannotBeConfiguredWithZeroAsserts()
        {
            // should throw
            new LimitAssertsPerTestCaseRule { MaxNumberOfAsserts = 0 };
        }

        [Test]
        public void TestThatAcceptableAssertsIsOneByDefault()
        {
            var rule = new LimitAssertsPerTestCaseRule();

            Assert.AreEqual(1, rule.MaxNumberOfAsserts);
        }

    }
}
