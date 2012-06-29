// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Linq;
using GraphBuilder;
using Mono.Cecil;
using NUnit.Framework;
using TestNess.Target;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class TestCaseTest
    {
        private TestCase _testCase;

        [SetUp]
        public void FindFirstTestCase()
        {
            _testCase = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());
        }

        [TestCase]
        public void TestThatTestCaseExposesTestCaseName()
        {
            Assert.AreEqual("TestNess.Target.IntegerCalculatorTest::TestAddBasic()", _testCase.Name);
        }

        [TestCase]
        public void TestThatTestCaseExposesTestMethod()
        {
            StringAssert.Contains("TestNess.Target.IntegerCalculatorTest::TestAddBasic()", _testCase.TestMethod.FullName);
        }

        [TestCase]
        public void TestThatTestCaseIsNotEqualToNull()
        {
            Assert.IsFalse(_testCase.Equals(null));
        }

        [TestCase]
        public void TestThatTestCaseIsNotEqualToNonTestCase()
        {
            Assert.IsFalse(_testCase.Equals("string"));
        }

        [TestCase]
        public void TestThatTestCasesAreEqualBasedOnTestMethod()
        {
            var testCase2 = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());
            Assert.AreEqual(_testCase, testCase2);
        }

        [TestCase]
        public void TestThatTestCasesWithSameTestMethodHaveSameHashCode()
        {
            var testCase2 = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());
            Assert.AreEqual(_testCase.GetHashCode(), testCase2.GetHashCode());
        }

        [TestCase]
        public void TestThatTestCasesWithDifferentTestMethodsAreNotEqual()
        {
            var testCase2 = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestSubtractBasic());
            Assert.AreNotEqual(_testCase, testCase2);
        }

        [TestCase]
        public void TestThatTestCasesWithDifferentTestMethodsHaveDifferentHashCode()
        {
            var testCase2 = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestSubtractBasic());
            Assert.AreNotEqual(_testCase.GetHashCode(), testCase2.GetHashCode());
        }

        [TestCase]
        public void TestThatToStringIncludesName()
        {
            var str = _testCase.ToString();
            StringAssert.Contains(_testCase.Name, str);
        }

        [TestCase]
        public void TestThatTestCaseExposesCallGraph()
        {
            Assert.AreEqual(typeof(Graph<MethodReference>), _testCase.CallGraph.GetType());
        }

        [TestCase]
        public void TestThatTestCaseCallGraphContainsMethodCalls()
        {
            var graph = _testCase.CallGraph;
            var result = graph.Walk().Aggregate("", (str, reference) => str + reference.Name + "\n");
            StringAssert.StartsWith("TestAddBasic\nAdd\nAreEqual\n", result);
        }

        [TestCase]
        public void TestThatTestCaseExposesCalledAssertingMethods()
        {
            var assertMethods = _testCase.GetCalledAssertingMethods();
            var names = assertMethods.Select(m => m.Name);
            CollectionAssert.Contains(names, "AreEqual");
        }

        [TestCase]
        public void TestThatTestCaseExposesAllCallsToSingleAssertingMethod()
        {
            var testCase = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddTwoAsserts());
            var assertMethods = testCase.GetCalledAssertingMethods();
            Assert.AreEqual(2, assertMethods.Count);
        }

        [TestCase]
        public void TestThatTestCaseCallGraphContainsResolvedMethodsWherePossible()
        {
            var graph = _testCase.CallGraph;
            var result = graph.Walk().Aggregate("", (str, reference) => str + string.Format("{0} ({1})\n", reference.Name, reference.IsDefinition));
            StringAssert.StartsWith("TestAddBasic (True)\nAdd (True)\nAreEqual (True)\n", result);
        }

        [TestCase]
        public void TestThatOriginContainsAssembly()
        {
            Assert.AreSame(_testCase.TestMethod.Module.Assembly, _testCase.Origin.Assembly);
        }

        [TestCase]
        public void TestThatOriginContainsAssemblyFileName()
        {
            Assert.That(_testCase.Origin.AssemblyFileName, Contains.Substring("TestNess.Target.dll").IgnoreCase);
        }

        [TestCase]
        public void TestThatTestCaseWithUnspecifiedOriginFindsOriginAssemblyInTestMethod()
        {
            var tc = new TestCase(_testCase.TestMethod);
            Assert.AreSame(tc.TestMethod.Module.Assembly, _testCase.Origin.Assembly);
        }
    }
}
