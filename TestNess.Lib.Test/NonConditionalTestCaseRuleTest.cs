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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestNess.Target;

namespace TestNess.Lib.Test
{
    [TestClass]
    public class NonConditionalTestCaseRuleTest
    {
        [TestMethod]
        public void TestThatNoViolationIsGeneratedForTestMethodWithoutCondition()
        {
            var tc = typeof(IntegerCalculatorTest).FindTestCase("TestAddBasic()");
            var rule = new NonConditionalTestCaseRule();
            var violations = rule.Apply(tc);

            Assert.AreEqual(0, violations.Count());
        }

        [TestMethod]
        public void TestThatViolationIsGeneratedForTestMethodWithIf()
        {
            var violations = FindViolations("TestAddWithIf()");
            Assert.AreEqual(1, violations.Count());
        }

        [TestMethod]
        public void TestThatViolationIsGeneratedForTestMethodWithFor()
        {
            var violations = FindViolations("TestAddWithFor()");
            Assert.AreEqual(1, violations.Count());
        }

        [TestMethod]
        public void TestThatViolationIsGeneratedForTestMethodWithWhile()
        {
            var violations = FindViolations("TestAddWithWhile()");
            Assert.AreEqual(1, violations.Count());
        }

        [TestMethod]
        public void TestThatViolationIsGeneratedForTestMethodWithDoWhile()
        {
            var violations = FindViolations("TestAddWithDoWhile()");
            Assert.AreEqual(1, violations.Count());
        }

        [TestMethod]
        public void TestThatViolationIsGeneratedForTestMethodWithSwitchCase()
        {
            var violations = FindViolations("TestAddWithSwitchCase()");
            Assert.AreEqual(1, violations.Count());
        }

        [TestMethod]
        public void TestThatToStringDescribesRule()
        {
            var rule = new NonConditionalTestCaseRule();
            Assert.AreEqual("a test case should not be conditional", rule.ToString());
        }

        private IEnumerable<Violation> FindViolations(string method)
        {
            var tc = typeof(IntegerCalculatorTest).FindTestCase(method);
            var rule = new NonConditionalTestCaseRule();
            return rule.Apply(tc);
        }
    }
}
