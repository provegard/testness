/**
 * Copyright (C) 2011 by Per Rovegård (per@rovegard.se)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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
            var tm = typeof (IntegerCalculatorMethodLengthTest).FindMethod(method);
            var rule = new LongTestCaseRule();
            var violations = rule.Apply(new TestCase(tm));

            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [TestCase]
        public void TestThatViolationMessageContainsTestCaseSpecificInformation()
        {
            var violation = FindViolations("ThenWeShouldBeAbleToAdd_Long()").First();
            Assert.AreEqual("test case contains 11 code statements (limit is 10)", violation.Message);
        }

        [TestCase]
        public void TestThatToStringDescribesRule()
        {
            var rule = new LongTestCaseRule();
            Assert.AreEqual("a test case should contain at most 10 code statements", rule.ToString());
        }

        [TestCase]
        public void TestThatToStringDescribesConfiguredRule()
        {
            var rule = new LongTestCaseRule { MaxNumberOfLinesOfCode = 20 };
            Assert.AreEqual("a test case should contain at most 20 code statements", rule.ToString());
        }

        [TestCase]
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

        [TestCase]
        public void TestThatAcceptableLocIsTenByDefault()
        {
            var rule = new LongTestCaseRule();

            Assert.AreEqual(10, rule.MaxNumberOfLinesOfCode);
        }
    }
}
