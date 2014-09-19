// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;
using TestNess.Lib.TestFramework;
using TestNess.Target;

namespace TestNess.Lib.Test.TestFramework
{
    [TestFixture]
    public class MSTestTestFrameworkTest
    {
        private MSTestTestFramework _framework;
        private Assembly _msTestAssembly;

        [TestFixtureSetUp]
        public void FindMSTestAssembly()
        {
            // Find the assembly but without having a static reference to it, because that
            // causes problems in Mono.
            var attrs = typeof(IntegerCalculatorTest).GetCustomAttributes(false);
            if (attrs.Length == 0)
                throw new Exception("Huh, failed to get attributes for IntegerCalculatorTest.");
            _msTestAssembly = attrs[0].GetType().Assembly;
        }

        [SetUp]
        public void GivenFramework()
        {
            _framework = new MSTestTestFramework();
        }

        [Test]
        public void TestThatIgnoredTestMethodIsRecognized()
        {
            string dummy;
            var method = typeof(IntegerCalculatorTest).FindMethod("TestIgnoredAdd()");
            Assert.IsTrue(_framework.IsIgnoredTest(method, out dummy));
        }

        [Test]
        public void TestThatInitializeMethodIsDetectedAsSetupMethod()
        {
            var method = typeof(IntegerCalculatorBddStyleTest).FindMethod("Setup()");
            Assert.IsTrue(_framework.IsSetupMethod(method));
        }

        [Test]
        public void TestThatTestMethodIsntDetectedAsSetupMethod()
        {
            var method = typeof(IntegerCalculatorBddStyleTest).FindMethod("ItShouldBeAbleToAdd()");
            Assert.IsFalse(_framework.IsSetupMethod(method));
        }

        [Test]
        public void TestThatMethodIsDetectedAsTestMethod()
        {
            var method = typeof (IntegerCalculatorTest).FindMethod("TestAddBasic()");
            Assert.IsTrue(_framework.IsTestMethod(method));
        }

        [Test]
        public void TestThatMethodIsDetectedAsNonTestMethod()
        {
            var method = typeof(IntegerCalculator).FindMethod("Add(System.Int32,System.Int32)");
            Assert.IsFalse(_framework.IsTestMethod(method));
        }

        [Test]
        public void TestThatExpectedExceptionIsIdentified()
        {
            var method = typeof(IntegerCalculatorTest).FindMethod("TestDivideWithException()");
            Assert.IsTrue(_framework.HasExpectedException(method));
        }

        [Test]
        public void TestThatNonExpectedExceptionIsIdentified()
        {
            var method = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            Assert.IsFalse(_framework.HasExpectedException(method));
        }

        [Test, Ignore("This test case won't work both under Windows and Mono as it is written given that our Mono version of MSTest doesn't have the specified method.")]
        public void TestThatAssertionThrowingMethodIsDetected()
        {
            //var method =
            //    typeof (Microsoft.VisualStudio.TestTools.UnitTesting.Assert).FindMethod(
            //        "HandleFailure(System.String,System.String)");
            //Assert.IsTrue(_framework.DoesContainAssertion(method));
        }

        [Test]
        public void TestThatNonAssertionThrowingMethodIsDetected()
        {
            var method = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            Assert.IsFalse(_framework.DoesContainAssertion(method));
        }

        [TestCase("Equals(System.Object,System.Object)", new[] {ParameterPurpose.Expected, ParameterPurpose.Actual})]
        [TestCase("AreEqual(System.Object,System.Object)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual })]
        [TestCase("AreEqual(System.Double,System.Double,System.Double,System.String)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual, ParameterPurpose.MetaData, ParameterPurpose.MetaData })]
        [TestCase("AreSame(System.Object,System.Object)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual })]
        [TestCase("Fail()", new ParameterPurpose[] {})]
        [TestCase("Fail(System.String)", new[] { ParameterPurpose.MetaData })]
        [TestCase("Inconclusive()", new ParameterPurpose[] {})]
        [TestCase("Inconclusive(System.String)", new[] { ParameterPurpose.MetaData })]
        [TestCase("IsInstanceOfType(System.Object,System.Type)", new[] { ParameterPurpose.Actual, ParameterPurpose.Expected })]
        [TestCase("IsTrue(System.Boolean)", new[] { ParameterPurpose.Actual })]
        public void TestThatParameterIndexIsIdentifiedCorrectlyForAssert(string methodName, ParameterPurpose[] types)
        {
            var method = FindTypeInMSTestAssembly("Microsoft.VisualStudio.TestTools.UnitTesting.Assert").FindMethod(methodName);
            VerifyExpectedTypes(method, types);
        }

        [TestCase("Contains(System.String,System.String)", new[] { ParameterPurpose.Actual, ParameterPurpose.Expected })]
        [TestCase("StartsWith(System.String,System.String)", new[] { ParameterPurpose.Actual, ParameterPurpose.Expected })]
        [TestCase("Matches(System.String,System.Text.RegularExpressions.Regex)", new[] { ParameterPurpose.Actual, ParameterPurpose.Expected })]
        public void TestThatParameterIndexIsIdentifiedCorrectlyForStringAssert(string methodName, ParameterPurpose[] types)
        {
            var method = FindTypeInMSTestAssembly("Microsoft.VisualStudio.TestTools.UnitTesting.StringAssert").FindMethod(methodName);
            VerifyExpectedTypes(method, types);
        }

        [TestCase("Contains(System.Collections.ICollection,System.Object)", new[] { ParameterPurpose.ExpectedOrActual, ParameterPurpose.ExpectedOrActual })]
        [TestCase("AllItemsAreInstancesOfType(System.Collections.ICollection,System.Type)", new[] { ParameterPurpose.Actual, ParameterPurpose.Expected })]
        [TestCase("AllItemsAreUnique(System.Collections.ICollection)", new[] { ParameterPurpose.Actual })]
        [TestCase("AreEqual(System.Collections.ICollection,System.Collections.ICollection)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual })]
        [TestCase("AreEquivalent(System.Collections.ICollection,System.Collections.ICollection)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual })]
        [TestCase("IsSubsetOf(System.Collections.ICollection,System.Collections.ICollection)", new[] { ParameterPurpose.ExpectedOrActual, ParameterPurpose.ExpectedOrActual })]
        public void TestThatParameterIndexIsIdentifiedCorrectlyForCollectionAssert(string methodName, ParameterPurpose[] types)
        {
            var method = FindTypeInMSTestAssembly("Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert").FindMethod(methodName);
            VerifyExpectedTypes(method, types);
        }

        private Type FindTypeInMSTestAssembly(string typeName)
        {
            return _msTestAssembly.GetType(typeName, true);
        }

        private void VerifyExpectedTypes(MethodReference method, IEnumerable<ParameterPurpose> types)
        {
            var actualTypes = _framework.GetParameterPurposes(method);
            CollectionAssert.AreEqual(types, actualTypes);
        }
    }
}
