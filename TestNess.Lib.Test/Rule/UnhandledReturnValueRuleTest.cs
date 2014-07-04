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
    public class UnhandledReturnValueRuleTest : AbstractRuleTest<UnhandledReturnValueRule, IntegerCalculatorReturnValueTest>
    {
        [TestCase("TestMultiplyWithUsedReturnValueFromStaticMethod()", 0)]
        [TestCase("TestAddWithUnhandledReturnValueFromStaticMethod()", 1)]
        [TestCase("TestAddWithUnhandledReturnValueFromInstanceMethod()", 1)]
        [TestCase("TestAddWithUnhandledReturnValueFromVirtualMethod()", 1)]
        [TestCase("TestAddWithCallToNonReturningMethod()", 0)]
        [TestCase("TestDivideWithException()", 0, Description = "Unhandled value is ok if an exception is expected.")]
        [TestCase("TestAddWithDoubleUnhandledReturnValueFromStaticMethod()", 2)]
        public void TestViolationCountForDifferentMethods(string method, int expectedViolationCount)
        {
            var violations = FindViolations(method);
            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [Test]
        public void TestThatViolationContainsSourceLocation()
        {
            var violation = FindViolations("TestAddWithUnhandledReturnValueFromStaticMethod()").First();
            Assert.IsNotNull(violation.Location);
        }

        [Test]
        public void TestThatViolationMessageRefersToMethod()
        {
            var violation = FindViolations("TestAddWithUnhandledReturnValueFromStaticMethod()").First();
            Assert.AreEqual("return value of method StaticMethod() is not used", violation.Message);
        }

        [Test]
        public void TestThatToStringDescribesRule()
        {
            var rule = new UnhandledReturnValueRule();
            Assert.AreEqual("a test case should deal with all method return values", rule.ToString());
        }
    }
}
