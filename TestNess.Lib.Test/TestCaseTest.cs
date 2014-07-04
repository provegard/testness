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

        [Test]
        public void TestThatTestCaseExposesTestCaseName()
        {
            Assert.AreEqual("TestNess.Target.IntegerCalculatorTest::TestAddBasic()", _testCase.Name);
        }

        [Test]
        public void TestThatTestCaseExposesTestMethod()
        {
            StringAssert.Contains("TestNess.Target.IntegerCalculatorTest::TestAddBasic()", _testCase.TestMethod.FullName);
        }

        [Test]
        public void TestThatTestCaseIsNotEqualToNull()
        {
            Assert.IsFalse(_testCase.Equals(null));
        }

        [Test]
        public void TestThatTestCaseIsNotEqualToNonTestCase()
        {
            Assert.IsFalse(_testCase.Equals("string"));
        }

        [Test]
        public void TestThatTestCasesAreEqualBasedOnTestMethod()
        {
            var testCase2 = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());
            Assert.AreEqual(_testCase, testCase2);
        }

        [Test]
        public void TestThatTestCasesWithSameTestMethodHaveSameHashCode()
        {
            var testCase2 = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());
            Assert.AreEqual(_testCase.GetHashCode(), testCase2.GetHashCode());
        }

        [Test]
        public void TestThatTestCasesWithDifferentTestMethodsAreNotEqual()
        {
            var testCase2 = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestSubtractBasic());
            Assert.AreNotEqual(_testCase, testCase2);
        }

        [Test]
        public void TestThatTestCasesWithDifferentTestMethodsHaveDifferentHashCode()
        {
            var testCase2 = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestSubtractBasic());
            Assert.AreNotEqual(_testCase.GetHashCode(), testCase2.GetHashCode());
        }

        [Test]
        public void TestThatToStringIncludesName()
        {
            var str = _testCase.ToString();
            StringAssert.Contains("TestAddBasic", str);
        }

        [Test]
        public void TestThatTestCaseExposesCallGraph()
        {
            Assert.AreEqual(typeof(Graph<MethodReference>), _testCase.CallGraph.GetType());
        }

        [Test]
        public void TestThatTestCaseCallGraphContainsMethodCalls()
        {
            var graph = _testCase.CallGraph;
            var result = graph.Walk().Aggregate("", (str, reference) => str + reference.Name + "\n");
            StringAssert.StartsWith("TestAddBasic\nAdd\nAreEqual\n", result);
        }

        [Test]
        public void TestThatTestCaseExposesCalledAssertingMethods()
        {
            var assertMethods = _testCase.GetCalledAssertingMethods();
            var names = assertMethods.Select(m => m.Name);
            CollectionAssert.Contains(names, "AreEqual");
        }

        [Test]
        public void TestThatTestCaseExposesAllCallsToSingleAssertingMethod()
        {
            var testCase = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddTwoAsserts());
            var assertMethods = testCase.GetCalledAssertingMethods();
            Assert.AreEqual(2, assertMethods.Count);
        }

        [Test]
        public void TestThatTestCaseCallGraphContainsResolvedMethodsWherePossible()
        {
            var graph = _testCase.CallGraph;
            var result = graph.Walk().Aggregate("", (str, reference) => str + string.Format("{0} ({1})\n", reference.Name, reference.IsDefinition));
            StringAssert.StartsWith("TestAddBasic (True)\nAdd (True)\nAreEqual (True)\n", result);
        }

        [Test]
        public void TestThatOriginContainsAssembly()
        {
            Assert.AreSame(_testCase.TestMethod.Module.Assembly, _testCase.Origin.Assembly);
        }

        [Test]
        public void TestThatOriginContainsAssemblyFileName()
        {
            Assert.That(_testCase.Origin.AssemblyFileName, Contains.Substring("TestNess.Target.dll").IgnoreCase);
        }

        [Test]
        public void TestThatTestCaseWithUnspecifiedOriginFindsOriginAssemblyInTestMethod()
        {
            var tc = new TestCase(_testCase.TestMethod);
            Assert.AreSame(tc.TestMethod.Module.Assembly, _testCase.Origin.Assembly);
        }
    }
}
