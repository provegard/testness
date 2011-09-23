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
using TestNess.Lib;

namespace TestNess.Lib.Test
{
    /// <summary>
    /// This class defines unit test cases for the <see cref="TestCaseRepository"/> class.
    /// </summary>
    [TestClass]
    public class TestCaseRepositoryTest
    {
        [TestMethod]
        public void TestThatMsTestCaseCanBeRetrievedFromRepository()
        {
            // Given
            var repo = new TestCaseRepository(TestHelper.GetTargetAssembly());

            // When
            var method = repo.GetTestCaseByName("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");

            // Then
            Assert.IsInstanceOfType(method, typeof(TestCase));
        }

        [TestMethod]
        [ExpectedException(typeof(NotATestMethodException))]
        public void TestThatRetrievalByNameFromRepositoryThrowsForNonTestMethod()
        {
            // Given
            var repo = new TestCaseRepository(TestHelper.GetTargetAssembly());

            // When, should throw!
            repo.GetTestCaseByName("TestNess.Target.IntegerCalculator::Add(System.Int32,System.Int32)");
        }

        [TestMethod]
        [ExpectedException(typeof(NotATestMethodException))]
        public void TestThatRetrievalByNameFromRepositoryThrowsForMissingMethod()
        {
            // Given
            var repo = new TestCaseRepository(TestHelper.GetTargetAssembly());

            // When, should throw!
            repo.GetTestCaseByName("TestNess.Target.MsTestIntegerCalculatorTest::NoSuchMethod()");
        }

        [TestMethod]
        public void TestThatRepositoryCachesTestCaseInstances()
        {
            // Given
            var repo = new TestCaseRepository(TestHelper.GetTargetAssembly());

            // When
            var method1 = repo.GetTestCaseByName("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");
            var method2 = repo.GetTestCaseByName("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");

            // Then
            Assert.AreSame(method1, method2);
        }
    }
}
