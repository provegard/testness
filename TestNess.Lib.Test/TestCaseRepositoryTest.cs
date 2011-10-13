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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;

namespace TestNess.Lib.Test
{
    [TestClass]
    public class TestCaseRepositoryTest
    {
        [TestMethod]
        public void TestThatMsTestCaseCanBeRetrievedFromRepository()
        {
            var repo = CreateTestCaseRepository();
            var testCase = repo.GetTestCaseByName("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            Assert.IsInstanceOfType(testCase, typeof(TestCase));
        }

        [TestMethod]
        [ExpectedException(typeof(NotATestMethodException))]
        public void TestThatRetrievalByNameFromRepositoryThrowsForNonTestMethod()
        {
            var repo = CreateTestCaseRepository();
            repo.GetTestCaseByName("TestNess.Target.IntegerCalculator::Add(System.Int32,System.Int32)"); // should throw
        }

        [TestMethod]
        [ExpectedException(typeof(NotATestMethodException))]
        public void TestThatRetrievalByNameFromRepositoryThrowsForMissingMethod()
        {
            var repo = CreateTestCaseRepository();
            repo.GetTestCaseByName("TestNess.Target.IntegerCalculatorTest::NoSuchMethod()"); // should throw
        }

        [TestMethod]
        public void TestThatRepositoryCachesTestCaseInstances()
        {
            var repo = CreateTestCaseRepository();
            var testCase1 = repo.GetTestCaseByName("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            var testCase2 = repo.GetTestCaseByName("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            Assert.AreSame(testCase1, testCase2);
        }

        [TestMethod]
        public void TestThatAllTestCasesCanBeRetrievedFromRepository()
        {
            var repo = CreateTestCaseRepository();
            var testCases = repo.GetAllTestCases();
            var testCase1 = repo.GetTestCaseByName("TestNess.Target.IntegerCalculatorTest::TestAddBasic()");
            var testCase2 = repo.GetTestCaseByName("TestNess.Target.IntegerCalculatorTest::TestSubtractBasic()");
            // Let's leave it at checking if at least some test cases are included
            CollectionAssert.IsSubsetOf(new List<TestCase> {testCase1, testCase2}, testCases.AsNonGeneric());
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestThatCollectionOfAllTestCasesIsImmutable()
        {
            var repo = CreateTestCaseRepository();
            var testCases = repo.GetAllTestCases();
            testCases.Clear(); // should throw
        }

        [TestMethod]
        public void TestThatCollectionOfAllTestCasesIsEmptyIfNoTestCases()
        {
            var assembly = Assembly.GetCallingAssembly();
            var assemblyDef = AssemblyDefinition.ReadAssembly(assembly.Location);
            var repo = new TestCaseRepository(assemblyDef);
            var testCases = repo.GetAllTestCases();
            Assert.AreEqual(0, testCases.Count);
        }

        [TestMethod]
        public void TestThatRepositoryCanBeLoadedFromFile()
        {
            var assembly = Assembly.GetCallingAssembly();
            var repo = TestCaseRepository.LoadFromFile(assembly.Location);
            Assert.IsNotNull(repo);
        }
        
        private static TestCaseRepository CreateTestCaseRepository()
        {
            return new TestCaseRepository(TestHelper.GetTargetAssembly());
        }
    }
}
