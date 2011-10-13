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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNess.Lib.Test
{
    [TestClass]
    public class TestCaseTest
    {
        [TestMethod]
        public void TestThatTestCaseExposesTestCaseName()
        {
            var testCase = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            Assert.AreEqual("TestNess.Target.IntegerCalculatorTest::TestAddBasic()", testCase.Name);
        }

        [TestMethod]
        public void TestThatTestCaseExposesTestMethod()
        {
            var testCase = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            StringAssert.Contains(testCase.TestMethod.FullName, "TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
        }

        [TestMethod]
        public void TestThatTestCaseIsNotEqualToNull()
        {
            var testCase = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            Assert.IsFalse(testCase.Equals(null));
        }

        [TestMethod]
        public void TestThatTestCaseIsNotEqualToNonTestCase()
        {
            var testCase = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            Assert.IsFalse(testCase.Equals("string"));
        }

        [TestMethod]
        public void TestThatTestCasesAreEqualBasedOnTestMethod()
        {
            var testCase1 = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            var testCase2 = new TestCase(testCase1.TestMethod);
            Assert.AreEqual(testCase1, testCase2);
        }

        [TestMethod]
        public void TestThatTestCasesWithSameTestMethodHaveSameHashCode()
        {
            var testCase1 = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            var testCase2 = new TestCase(testCase1.TestMethod);
            Assert.AreEqual(testCase1.GetHashCode(), testCase2.GetHashCode());
        }

        [TestMethod]
        public void TestThatTestCasesWithDifferentTestMethodsAreNotEqual()
        {
            var testCase1 = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            var testCase2 = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestSubtractBasic()");
            Assert.AreNotEqual(testCase1, testCase2);
        }

        [TestMethod]
        public void TestThatTestCasesWithDifferentTestMethodsHaveDifferentHashCode()
        {
            var testCase1 = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            var testCase2 = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestSubtractBasic()");
            Assert.AreNotEqual(testCase1.GetHashCode(), testCase2.GetHashCode());
        }

        [TestMethod]
        public void TestThatToStringIncludesName()
        {
            var testCase = FindTestCase("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            var str = testCase.ToString();
            StringAssert.Contains(str, testCase.Name);
        }

        private static TestCase FindTestCase(string testMethodName)
        {
            return new TestCaseRepository(TestHelper.GetTargetAssembly()).GetTestCaseByName(testMethodName);
        }
    }
}
