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
    public class NoTryCatchInTestCaseRuleTest : AbstractRuleTest<NoTryCatchInTestCaseRule, IntegerCalculatorTest>
    {
        [TestCase("TestAddBasic()", 0)]
        [TestCase("DivideByZeroWithTryCatch()", 1)]
        [TestCase("AddWithForEach()", 0)]
        public void TestViolationCountForDifferentMethods(string method, int expectedViolationCount)
        {
            var violations = FindViolations(method);
            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [TestCase]
        public void TestThatToStringDescribesRule()
        {
            var rule = new NoTryCatchInTestCaseRule();
            Assert.AreEqual("a test case should not contain try-catch", rule.ToString());
        }
    }
}
