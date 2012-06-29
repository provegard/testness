// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
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
