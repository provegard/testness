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
    /// <summary>
    /// This class defines unit test cases for the <see cref="TestCase"/> class.
    ///</summary>
    [TestClass]
    public class TestCaseTest
    {
        [TestMethod]
        public void TestThatTestCaseExposesTestCaseName()
        {
            // Given
            var testCase = FindTestCase("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");

            // When
            var name = testCase.Name;

            // Then
            Assert.AreEqual("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()", name);
        }

        [TestMethod]
        public void TestThatTestCaseExposesTestMethod()
        {
            // Given
            var testCase = FindTestCase("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");

            // When
            var method = testCase.TestMethod;

            // Then
            Assert.IsTrue(method.FullName.Contains("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()"));
        }

        [TestMethod]
        public void TestThatTestCaseIsNotEqualToNull()
        {
            // Given
            var testCase = FindTestCase("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");

            // Then
            Assert.IsFalse(testCase.Equals(null));
        }

        [TestMethod]
        public void TestThatTestCaseIsNotEqualToNonTestCase()
        {
            // Given
            var testCase = FindTestCase("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");

            // Then
            Assert.IsFalse(testCase.Equals("string"));
        }

        [TestMethod]
        public void TestThatTestCasesAreEqualBasedOnTestMethod()
        {
            // Given
            var testCase1 = FindTestCase("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");
            var testCase2 = new TestCase(testCase1.TestMethod);

            // Then
            Assert.AreEqual(testCase1, testCase2);
        }

        [TestMethod]
        public void TestThatTestCasesWithSameTestMethodHaveSameHashCode()
        {
            // Given
            var testCase1 = FindTestCase("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");
            var testCase2 = new TestCase(testCase1.TestMethod);

            // Then
            Assert.AreEqual(testCase1.GetHashCode(), testCase2.GetHashCode());
        }

        [TestMethod]
        public void TestThatTestCasesWithDifferentTestMethodsAreNotEqual()
        {
            // Given
            var testCase1 = FindTestCase("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");
            var testCase2 = FindTestCase("TestNess.Target.MsTestIntegerCalculatorTest::TestSubtractBasic()");

            // Then
            Assert.AreNotEqual(testCase1, testCase2);
        }

        [TestMethod]
        public void TestThatTestCasesWithDifferentTestMethodsHaveDifferentHashCode()
        {
            // Given
            var testCase1 = FindTestCase("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");
            var testCase2 = FindTestCase("TestNess.Target.MsTestIntegerCalculatorTest::TestSubtractBasic()");

            // Then
            Assert.AreNotEqual(testCase1.GetHashCode(), testCase2.GetHashCode());
        }

        private TestCase FindTestCase(string testMethodName)
        {
            return new TestCaseRepository(TestHelper.GetTargetAssembly()).GetTestCaseByName(testMethodName);
        }
    }
}
