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
    public class LocalExpectationRuleTest : AbstractRuleTest<LocalExpectationRule, IntegerCalculatorExpectationTest>
    {
        [TestCase("TestAddWithLiteralExpectation()", 0)]
        [TestCase("TestAddWithExternallyCalculatedExpectation()", 1)]
        [TestCase("TestAddWithConstExpectation()", 0)]
        [TestCase("TestAddWithSwitchedActualAndExpectation()", 1)]
        [TestCase("TestAddWith9To32BitConstExpectation()", 0, Description = "Automatic data type conversion should not cause a violation.")]
        [TestCase("TestAddWith8OrFewerBitConstExpectation()", 0, Description = "Automatic data type conversion should not cause a violation.")]
        [TestCase("TestDataDrivenAdd()", 0, Description = "Data-driven testing requires external expectations.")]
        [TestCase("TestAddWithLocallyCalculatedExpectation()", 0)]
        [TestCase("TestAddWithExpectationDerivedFromField()", 1)]
        [TestCase("TestAddWithStaticReadonlyExpectation()", 0, Description = "Static readonly is treated as Const since it's required when the value isn't a compile-time constant!")]
        [TestCase("TestAddWithExpectationFromProperty()", 1)]
        [TestCase("TestAddWithManuallyComparedConstantExpectation()", 0)]
        [TestCase("TestAddWithManuallyComparedExternallyCalculatedExpectation()", 1)]
        [TestCase("TestAddWithLteComparedExternallyCalculatedExpectation()", 1)]
        [TestCase("TestAddWithNeqComparedExternallyCalculatedExpectation()", 1)]
        [TestCase("TestWithUnconditionalFailure()", 0)]
        [TestCase("TestAddWithMultipleExpectationViolations()", 2)]
        [TestCase("TestAddWithConditionalExpectationViolations()", 2)]
        [TestCase("TestAddWithExpectationFromStaticGetOnlyProperty()", 0)]
        [TestCase("TestAddWithExpectationFromStaticAutoGetPrivSetProperty()", 1)]
        [TestCase("TestAddWithExpectationFromStaticAutoGetSetProperty()", 1)]
        public void TestViolationCountForDifferentMethods(string method, int expectedViolationCount)
        {
            var violations = FindViolations(method);
            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [TestCase]
        public void TestThatViolationContainsLocation()
        {
            var violation = FindViolations("TestAddWithExternallyCalculatedExpectation()").First();
            Assert.IsNotNull(violation.Location);
        }

        [TestCase]
        public void TestThatViolationContainsLocationOfExternalSource()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorLocationTest>(t => t.TestAddWithExternallyCalculatedExpectation());
            var violation = new LocalExpectationRule().Apply(tc).First();
            // The source of the violation is the second call
            Assert.AreEqual(24, violation.Location.StartLine);
        }

        [TestCase]
        public void TestThatViolationMessageRefersToConsumerIfPossible()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorLocationTest>(t => t.TestAddWithExternallyCalculatedExpectation());
            var violation = new LocalExpectationRule().Apply(tc).First();
            Assert.AreEqual("external production of (possibly) expected value (argument 1 of AreEqual on line 26)", violation.Message);
        }

        [TestCase]
        public void TestThatViolationForUncertainCaseContainsLocationOfValueUsage()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorLocationTest>(t => t.TestAddWithManuallyComparedExternallyCalculatedExpectation());
            var violation = new LocalExpectationRule().Apply(tc).First();
            // The values are used one line 35, by the assert call.
            Assert.AreEqual(35, violation.Location.StartLine);
        }

        [TestCase]
        public void TestThatViolationMessageForUncertainCaseRefersToValueUser()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorLocationTest>(t => t.TestAddWithManuallyComparedExternallyCalculatedExpectation());
            var violation = new LocalExpectationRule().Apply(tc).First();
            Assert.AreEqual("the expected value used by IsTrue should be produced locally", violation.Message);
        }

        [TestCase]
        public void TestThatToStringDescribesRule()
        {
            var rule = new LocalExpectationRule();
            Assert.AreEqual("an assert expectation should be locally produced", rule.ToString());
        }

        [TestCase]
        public void TestThatObjectConstructionCountsAsLocalProduction()
        {
            var rule = new LocalExpectationRule();
            var tc = TestHelper.FindTestCase<MiscTest>(t => t.ObjectConstruction());
            var violations = rule.Apply(tc);
            Assert.AreEqual(0, violations.Count());
        }

        [TestCase]
        public void TestThatTypeOfCountsAsLocalProduction()
        {
            var rule = new LocalExpectationRule();
            var tc = TestHelper.FindTestCase<MiscTest>(t => t.UseOfTypeOf());
            var violations = rule.Apply(tc);
            Assert.AreEqual(0, violations.Count());
        }
    }
}
