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

using System.Collections.Generic;
using Mono.Cecil;
using NUnit.Framework;
using TestNess.Target;
using PP = TestNess.Lib.ParameterPurpose;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class MSTestTestFrameworkTest
    {
        private MSTestTestFramework _framework;

        [SetUp]
        public void GivenFramework()
        {
            _framework = new MSTestTestFramework();
        }

        [TestCase]
        public void TestThatMethodIsDetectedAsTestMethod()
        {
            var method = typeof (IntegerCalculatorTest).FindMethod("TestAddBasic()");
            Assert.IsTrue(_framework.IsTestMethod(method));
        }

        [TestCase]
        public void TestThatMethodIsDetectedAsNonTestMethod()
        {
            var method = typeof(IntegerCalculator).FindMethod("Add(System.Int32,System.Int32)");
            Assert.IsFalse(_framework.IsTestMethod(method));
        }

        [TestCase]
        public void TestThatExpectedExceptionIsIdentified()
        {
            var method = typeof(IntegerCalculatorTest).FindMethod("TestDivideWithException()");
            Assert.IsTrue(_framework.HasExpectedException(method));
        }

        [TestCase]
        public void TestThatNonExpectedExceptionIsIdentified()
        {
            var method = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            Assert.IsFalse(_framework.HasExpectedException(method));
        }

        [TestCase]
        public void TestThatAssertionThrowingMethodIsDetected()
        {
            var method =
                typeof (Microsoft.VisualStudio.TestTools.UnitTesting.Assert).FindMethod(
                    "HandleFail(System.String,System.String,System.Object[])");
            Assert.IsTrue(_framework.DoesContainAssertion(method));
        }

        [TestCase]
        public void TestThatNonAssertionThrowingMethodIsDetected()
        {
            var method = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            Assert.IsFalse(_framework.DoesContainAssertion(method));
        }

        [TestCase("Equals(System.Object,System.Object)", new[] {PP.Expected, PP.Actual})]
        [TestCase("AreEqual(System.Object,System.Object)", new[] { PP.Expected, PP.Actual })]
        [TestCase("AreEqual(System.Double,System.Double,System.Double,System.String)", new[] { PP.Expected, PP.Actual, PP.MetaData, PP.MetaData })]
        [TestCase("AreSame(System.Object,System.Object)", new[] { PP.Expected, PP.Actual })]
        [TestCase("Fail()", new PP[] {})]
        [TestCase("Fail(System.String)", new[] { PP.MetaData })]
        [TestCase("Inconclusive()", new PP[] {})]
        [TestCase("Inconclusive(System.String)", new[] { PP.MetaData })]
        [TestCase("IsInstanceOfType(System.Object,System.Type)", new[] { PP.Actual, PP.Expected })]
        [TestCase("IsTrue(System.Boolean)", new[] { PP.Actual })]
        public void TestThatParameterIndexIsIdentifiedCorrectlyForAssert(string methodName, PP[] types)
        {
            var method = typeof(Microsoft.VisualStudio.TestTools.UnitTesting.Assert).FindMethod(methodName);
            VerifyExpectedTypes(method, types);
        }

        [TestCase("Contains(System.String,System.String)", new[] { PP.Actual, PP.Expected })]
        [TestCase("StartsWith(System.String,System.String)", new[] { PP.Actual, PP.Expected })]
        [TestCase("Matches(System.String,System.Text.RegularExpressions.Regex)", new[] { PP.Actual, PP.Expected })]
        public void TestThatParameterIndexIsIdentifiedCorrectlyForStringAssert(string methodName, PP[] types)
        {
            var method = typeof(Microsoft.VisualStudio.TestTools.UnitTesting.StringAssert).FindMethod(methodName);
            VerifyExpectedTypes(method, types);
        }

        [TestCase("Contains(System.Collections.ICollection,System.Object)", new[] { PP.ExpectedOrActual, PP.ExpectedOrActual })]
        [TestCase("AllItemsAreInstancesOfType(System.Collections.ICollection,System.Type)", new[] { PP.Actual, PP.Expected })]
        [TestCase("AllItemsAreUnique(System.Collections.ICollection)", new[] { PP.Actual })]
        [TestCase("AreEqual(System.Collections.ICollection,System.Collections.ICollection)", new[] { PP.Expected, PP.Actual })]
        [TestCase("AreEquivalent(System.Collections.ICollection,System.Collections.ICollection)", new[] { PP.Expected, PP.Actual })]
        [TestCase("IsSubsetOf(System.Collections.ICollection,System.Collections.ICollection)", new[] { PP.ExpectedOrActual, PP.ExpectedOrActual })]
        public void TestThatParameterIndexIsIdentifiedCorrectlyForCollectionAssert(string methodName, PP[] types)
        {
            var method = typeof(Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert).FindMethod(methodName);
            VerifyExpectedTypes(method, types);
        }

        private void VerifyExpectedTypes(MethodReference method, IEnumerable<ParameterPurpose> types)
        {
            var actualTypes = _framework.GetParameterPurposes(method);
            CollectionAssert.AreEqual(types, actualTypes);
        }
    }
}
