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
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;

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
            var repo = CreateTestCaseRepository();

            // When
            var testCase = repo.GetTestCaseByName("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");

            // Then
            Assert.IsInstanceOfType(testCase, typeof(TestCase));
        }

        [TestMethod]
        [ExpectedException(typeof(NotATestMethodException))]
        public void TestThatRetrievalByNameFromRepositoryThrowsForNonTestMethod()
        {
            // Given
            var repo = CreateTestCaseRepository();

            // When, should throw!
            repo.GetTestCaseByName("TestNess.Target.IntegerCalculator::Add(System.Int32,System.Int32)");
        }

        [TestMethod]
        [ExpectedException(typeof(NotATestMethodException))]
        public void TestThatRetrievalByNameFromRepositoryThrowsForMissingMethod()
        {
            // Given
            var repo = CreateTestCaseRepository();

            // When, should throw!
            repo.GetTestCaseByName("TestNess.Target.MsTestIntegerCalculatorTest::NoSuchMethod()");
        }

        [TestMethod]
        public void TestThatRepositoryCachesTestCaseInstances()
        {
            // Given
            var repo = CreateTestCaseRepository();

            // When
            var testCase1 = repo.GetTestCaseByName("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");
            var testCase2 = repo.GetTestCaseByName("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");

            // Then
            Assert.AreSame(testCase1, testCase2);
        }

        [TestMethod]
        public void TestThatAllTestCasesCanBeRetrievedFromRepository()
        {
            // Given
            var repo = CreateTestCaseRepository();

            // When
            var testCases = repo.GetAllTestCases();
            var testCase1 = repo.GetTestCaseByName("TestNess.Target.MsTestIntegerCalculatorTest::TestAddBasic()");
            var testCase2 = repo.GetTestCaseByName("TestNess.Target.MsTestIntegerCalculatorTest::TestSubtractBasic()");

            // Then (let's leave it at checking if at least some test cases are included)
            CollectionAssert.IsSubsetOf(new List<TestCase> {testCase1, testCase2}, testCases.AsNonGeneric());
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestThatCollectionOfAllTestCasesIsImmutable()
        {
            // Given
            var repo = CreateTestCaseRepository();
            var testCases = repo.GetAllTestCases();

            // When (should throw)
            testCases.Clear();
        }

        [TestMethod]
        public void TestThatCollectionOfAllTestCasesIsEmptyIfNoTestCases()
        {
            // Given
            var assembly = Assembly.GetCallingAssembly();
            var assemblyDef = AssemblyDefinition.ReadAssembly(assembly.Location);
            var repo = new TestCaseRepository(assemblyDef);
            
            // When
            var testCases = repo.GetAllTestCases();

            // Then
            Assert.AreEqual(0, testCases.Count);
        }

        [TestMethod]
        public void TestThatRepositoryCanBeLoadedFromFile()
        {
            // Given
            var assembly = Assembly.GetCallingAssembly();

            // When
            var repo = TestCaseRepository.LoadFromFile(assembly.Location);

            // Then
            Assert.IsNotNull(repo);
        }
        
        private static TestCaseRepository CreateTestCaseRepository()
        {
            return new TestCaseRepository(TestHelper.GetTargetAssembly());
        }
    }
}
