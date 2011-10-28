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
            _testCase = typeof(IntegerCalculatorTest).FindTestCase("TestAddBasic()");
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
            var testCase2 = new TestCase(_testCase.TestMethod);
            Assert.AreEqual(_testCase, testCase2);
        }

        [TestCase]
        public void TestThatTestCasesWithSameTestMethodHaveSameHashCode()
        {
            var testCase2 = new TestCase(_testCase.TestMethod);
            Assert.AreEqual(_testCase.GetHashCode(), testCase2.GetHashCode());
        }

        [TestCase]
        public void TestThatTestCasesWithDifferentTestMethodsAreNotEqual()
        {
            var testCase2 = typeof(IntegerCalculatorTest).FindTestCase("TestSubtractBasic()");
            Assert.AreNotEqual(_testCase, testCase2);
        }

        [TestCase]
        public void TestThatTestCasesWithDifferentTestMethodsHaveDifferentHashCode()
        {
            var testCase2 = typeof(IntegerCalculatorTest).FindTestCase("TestSubtractBasic()");
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
            var result = "";
            graph.Walk(reference => result += reference.Name + "\n");
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
            var testCase = typeof(IntegerCalculatorTest).FindTestCase("TestAddTwoAsserts()");
            var assertMethods = testCase.GetCalledAssertingMethods();
            Assert.AreEqual(2, assertMethods.Count);
        }

        [TestCase]
        public void TestThatTestCaseCallGraphContainsResolvedMethodsWherePossible()
        {
            var graph = _testCase.CallGraph;
            var result = "";
            graph.Walk(reference => result += string.Format("{0} ({1})\n", reference.Name, reference.IsDefinition));
            StringAssert.StartsWith("TestAddBasic (True)\nAdd (True)\nAreEqual (True)\n", result);
        }
    }
}
