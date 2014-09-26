// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Linq;
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class AssertDistributionRuleTest : AbstractRuleTest<AssertDistributionRule, IntegerCalculatorDistributionTest>
    {
        [TestCase("TestAddWithSingleAssert()", 0)]
        [TestCase("TestAddWithSpreadAsserts()", 1)]
        [TestCase("TestAddWithAssertsInTheMiddle()", 1)]
        [TestCase("TestAddWithWhitespaceSeparatedAssertsTowardsTheEnd()", 0)]
        public void TestViolationCountForDifferentMethods(string method, int expectedViolationCount)
        {
            var violations = FindViolations(method);
            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [Test]
        public void TestThatToStringDescribesRule()
        {
            var rule = new AssertDistributionRule();
            Assert.AreEqual("all asserts in a test case should be placed together last in the test method", rule.ToString());
        }
    }
}
