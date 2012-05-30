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
using System.Collections.Generic;
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class ViolationTest
    {
        [TestCase]
        public void TestThatViolationExposesRule()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());
            var rule = new SomeRule();

            var violation = new Violation(rule, tc);

            Assert.AreSame(rule, violation.Rule);
        }

        [TestCase]
        public void TestThatViolationExposesTestCase()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());

            var violation = new Violation(new SomeRule(), tc);

            Assert.AreSame(tc, violation.TestCase);
        }

        [TestCase]
        public void TestThatToStringWithoutDebugSymbolsIncludesTypeAndMethod()
        {
            var tm = typeof (IntegerCalculatorTest).FindMethod("TestAddBasic()");
            var rule = new SomeRule();

            var violation = new Violation(rule, new TestCase(tm));

            const string expected = "TestNess.Target.IntegerCalculatorTest(TestAddBasic()): violation of \"some rule\"";
            Assert.AreEqual(expected, violation.ToString());
        }

        [TestCase]
        public void TestThatViolationExposesMessage()
        {
            var tm = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            var rule = new SomeRule();
            var violation = new Violation(rule, new TestCase(tm));

            Assert.AreEqual("violation of \"some rule\"", violation.Message);
        }

        [TestCase]
        public void TestThatMessageCanBeCustomized()
        {
            var tm = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            var rule = new SomeRule();
            var violation = new Violation(rule, new TestCase(tm), "a message");

            Assert.AreEqual("a message", violation.Message);
        }

        [TestCase]
        public void TestThatCustomizedMessageOccursInToString()
        {
            var tm = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            var rule = new SomeRule();
            var violation = new Violation(rule, new TestCase(tm), "a message");

            StringAssert.EndsWith(": a message", violation.ToString());
        }

        internal class SomeRule : IRule
        {
            public IEnumerable<Violation> Apply(TestCase testCase)
            {
                throw new NotImplementedException();
            }

            public override string ToString()
            {
                return "some rule";
            }
        }
    }
}
