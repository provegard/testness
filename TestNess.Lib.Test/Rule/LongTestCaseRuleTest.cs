// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Linq;
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class LongTestCaseRuleTest : AbstractRuleTest<LongTestCaseRule, IntegerCalculatorMethodLengthTest>
    {
        [TestCase("ThenWeShouldBeAbleToAdd_Short()", 0)]
        [TestCase("ThenWeShouldBeAbleToAdd_Long()", 1)]
        public void TestViolationCountForDifferentMethods(string method, int expectedViolationCount)
        {
            var violations = FindViolations(method);
            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [TestCase("ThenWeShouldBeAbleToAdd_Short()", 0)]
        [TestCase("ThenWeShouldBeAbleToAdd_Long()", 1)]
        public void TestViolationCountForDifferentMethodsWithoutSequencePoints(string method, int expectedViolationCount)
        {
            // "Manual" test case creation to avoid PDB loading...
            var tc = typeof (IntegerCalculatorMethodLengthTest).FindTestCase(method);
            var rule = new LongTestCaseRule();
            var violations = rule.Apply(tc);

            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [Test]
        public void TestThatViolationMessageContainsTestCaseSpecificInformation()
        {
            var violation = FindViolations("ThenWeShouldBeAbleToAdd_Long()").First();
            Assert.AreEqual("test case contains 11 code statements (limit is 10)", violation.Message);
        }

        [Test]
        public void TestThatToStringDescribesRule()
        {
            var rule = new LongTestCaseRule();
            Assert.AreEqual("a test case should contain at most 10 code statements", rule.ToString());
        }

        [Test]
        public void TestThatToStringDescribesConfiguredRule()
        {
            var rule = new LongTestCaseRule { MaxNumberOfLinesOfCode = 20 };
            Assert.AreEqual("a test case should contain at most 20 code statements", rule.ToString());
        }

        [Test]
        public void TestThatRuleCanBeConfiguredWithAcceptedNumberOfAsserts()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorMethodLengthTest>(t => t.ThenWeShouldBeAbleToAdd_Long());
            var rule = new LongTestCaseRule { MaxNumberOfLinesOfCode = 20 };

            Assert.AreEqual(0, rule.Apply(tc).Count());
        }

        [TestCase, ExpectedException(typeof(ArgumentException))]
        public void TestThatRuleCannotBeConfiguredWithZeroAsserts()
        {
            // should throw
            new LongTestCaseRule { MaxNumberOfLinesOfCode = 0 };
        }

        [Test]
        public void TestThatAcceptableLocIsTenByDefault()
        {
            var rule = new LongTestCaseRule();

            Assert.AreEqual(10, rule.MaxNumberOfLinesOfCode);
        }
    }
}
