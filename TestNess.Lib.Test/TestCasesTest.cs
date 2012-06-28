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
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class TestCasesTest
    {
        [TestCase]
        public void TestThatMsTestCaseCanBeRetrievedFromRepository()
        {
            var repo = CreateTestCaseRepository();
            var testCase = repo.GetTestCaseByName("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            Assert.IsInstanceOf(typeof(TestCase), testCase);
        }

        [TestCase]
        public void TestThatRetrievalByNameFromRepositoryThrowsForNonTestMethod()
        {
            var repo = CreateTestCaseRepository();
            Assert.Throws<NotATestMethodException>(
                () => repo.GetTestCaseByName("TestNess.Target.IntegerCalculator::Add(System.Int32,System.Int32)"));
        }

        [TestCase]
        public void TestThatRetrievalByNameFromRepositoryThrowsForMissingMethod()
        {
            var repo = CreateTestCaseRepository();
            Assert.Throws<NotATestMethodException>(
                () => repo.GetTestCaseByName("TestNess.Target.IntegerCalculatorTest::NoSuchMethod()"));
        }

        [TestCase]
        public void TestThatAllTestCasesCanBeRetrievedFromRepository()
        {
            var repo = CreateTestCaseRepository();
            var testCases = repo.GetAllTestCases();
            var testCase1 = repo.GetTestCaseByName("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            var testCase2 = repo.GetTestCaseByName("TestNess.Target.IntegerCalculatorTest::TestSubtractBasic()");
            // Let's leave it at checking if at least some test cases are included
            CollectionAssert.IsSubsetOf(new List<TestCase> {testCase1, testCase2}, testCases.AsNonGeneric());
        }

        [TestCase]
        public void TestThatCollectionOfAllTestCasesIsEmptyIfNoTestCases()
        {
            var assembly = Assembly.GetCallingAssembly();
            var repo = TestCases.FromAssembly(assembly);
            var testCases = repo.GetAllTestCases();
            Assert.AreEqual(0, testCases.Count);
        }

        [TestCase]
        public void TestThatRepositoryCanBeLoadedFromFile()
        {
            var assembly = Assembly.GetCallingAssembly();
            var repo = TestCases.LoadFromFile(assembly.Location);
            Assert.IsNotNull(repo);
        }

        [TestCase]
        public void TestThatTestMethodsInRepositoryContainsSequencePoints()
        {
            var repo = TestCases.FromAssembly(TestHelper.GetTargetAssembly());
            var tc = repo.GetAllTestCases().First();
            var nonNullSequencePoints = tc.TestMethod.Body.Instructions.Where(i => i.SequencePoint != null);
            Assert.AreNotEqual(0, nonNullSequencePoints.Count());
        }
        
        private static TestCases CreateTestCaseRepository()
        {
            return TestCases.FromAssembly(TestHelper.GetTargetAssembly());
        }
    }
}
