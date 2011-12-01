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
        [TestCase("TestAddWithStaticReadonlyExpectation()", 1, Description = "Static readonly is not treated as Const!")]
        [TestCase("TestAddWithExpectationFromProperty()", 1)]
        [TestCase("TestAddWithManuallyComparedConstantExpectation()", 0)]
        [TestCase("TestAddWithManuallyComparedExternallyCalculatedExpectation()", 1)]
        [TestCase("TestAddWithLteComparedExternallyCalculatedExpectation()", 1)]
        [TestCase("TestAddWithNeqComparedExternallyCalculatedExpectation()", 1)]
        [TestCase("TestWithUnconditionalFailure()", 0)]
        public void TestViolationCountForDifferentMethods(string method, int expectedViolationCount)
        {
            var violations = FindViolations(method);
            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [TestCase]
        public void TestThatToStringDescribesRule()
        {
            var rule = new LocalExpectationRule();
            Assert.AreEqual("an assert expectation should be locally produced", rule.ToString());
        }
    }
}
