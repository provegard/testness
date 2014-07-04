// Copyright (C) 2011-2013 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Collections.Generic;
using Mono.Cecil;
using NUnit.Framework;
using TestNess.Lib.TestFramework;
using TestNess.Target;

namespace TestNess.Lib.Test.TestFramework
{
    [TestFixture]
    public class NUnitTestFrameworkTest
    {
        private NUnitTestFramework _framework;

        [SetUp]
        public void GivenFramework()
        {
            _framework = new NUnitTestFramework();
        }

        [Test]
        public void TestThatBeforeAllMethodIsDetectedAsSetupMethod()
        {
            var method = typeof(IntegerCalculatorBddStyleTestNUnit).FindMethod("BeforeAll()");
            Assert.IsTrue(_framework.IsSetupMethod(method));
        }

        [Test]
        public void TestThatBeforeEachMethodIsDetectedAsSetupMethod()
        {
            var method = typeof(IntegerCalculatorBddStyleTestNUnit).FindMethod("BeforeEach()");
            Assert.IsTrue(_framework.IsSetupMethod(method));
        }

        [Test]
        public void TestThatTestMethodIsntDetectedAsSetupMethod()
        {
            var method = typeof(IntegerCalculatorBddStyleTestNUnit).FindMethod("ItShouldBeAbleToAdd()");
            Assert.IsFalse(_framework.IsSetupMethod(method));
        }

        [Test]
        public void TestThatMethodIsDetectedAsTestMethod()
        {
            var method = typeof (IntegerCalculatorTestNUnit).FindMethod("TestAddBasic()");
            Assert.IsTrue(_framework.IsTestMethod(method));
        }

        [Test]
        public void TestThatDataDrivenMethodIsDetectedAsTestMethod()
        {
            var method = typeof(IntegerCalculatorTestNUnit).FindMethod("TestDataDriven(System.Int32,System.Int32,System.Int32)");
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
            var method = typeof(IntegerCalculatorTestNUnit).FindMethod("TestDivideWithException()");
            Assert.IsTrue(_framework.HasExpectedException(method));
        }

        [Test]
        public void TestThatNonExpectedExceptionIsIdentified()
        {
            var method = typeof(IntegerCalculatorTestNUnit).FindMethod("TestAddBasic()");
            Assert.IsFalse(_framework.HasExpectedException(method));
        }

        [Test]
        public void TestThatAssertionThrowingMethodIsDetected()
        {
            // That(object actual, IResolveConstraint expression, string message, params object[] args)
            var method =
                typeof (Assert).FindMethod(
                    "That(System.Object,NUnit.Framework.Constraints.IResolveConstraint,System.String,System.Object[])");
            Assert.IsTrue(_framework.DoesContainAssertion(method));
        }

        [Test]
        public void TestThatNonAssertionThrowingMethodIsDetected()
        {
            var method = typeof(IntegerCalculatorTestNUnit).FindMethod("TestAddBasic()");
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
        [TestCase("IsInstanceOfType(System.Type,System.Object)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual })]
        [TestCase("IsTrue(System.Boolean)", new[] { ParameterPurpose.Actual })]
        public void TestThatParameterIndexIsIdentifiedCorrectlyForAssert(string methodName, ParameterPurpose[] types)
        {
            var method = typeof(Assert).FindMethod(methodName);
            VerifyExpectedTypes(method, types);
        }

        [TestCase("Contains(System.String,System.String)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual })]
        [TestCase("StartsWith(System.String,System.String)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual })]
        [TestCase("IsMatch(System.String,System.String)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual })]
        [TestCase("IsMatch(System.String,System.String,System.String)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual, ParameterPurpose.MetaData })]
        public void TestThatParameterIndexIsIdentifiedCorrectlyForStringAssert(string methodName, ParameterPurpose[] types)
        {
            var method = typeof(StringAssert).FindMethod(methodName);
            VerifyExpectedTypes(method, types);
        }

        [TestCase("Contains(System.Collections.IEnumerable,System.Object)", new[] { ParameterPurpose.ExpectedOrActual, ParameterPurpose.ExpectedOrActual })]
        [TestCase("AllItemsAreInstancesOfType(System.Collections.IEnumerable,System.Type)", new[] { ParameterPurpose.Actual, ParameterPurpose.Expected })]
        [TestCase("AllItemsAreUnique(System.Collections.IEnumerable)", new[] { ParameterPurpose.Actual })]
        [TestCase("AreEqual(System.Collections.IEnumerable,System.Collections.IEnumerable)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual })]
        [TestCase("AreEquivalent(System.Collections.IEnumerable,System.Collections.IEnumerable)", new[] { ParameterPurpose.Expected, ParameterPurpose.Actual })]
        [TestCase("IsSubsetOf(System.Collections.IEnumerable,System.Collections.IEnumerable)", new[] { ParameterPurpose.ExpectedOrActual, ParameterPurpose.ExpectedOrActual })]
        public void TestThatParameterIndexIsIdentifiedCorrectlyForCollectionAssert(string methodName, ParameterPurpose[] types)
        {
            var method = typeof(CollectionAssert).FindMethod(methodName);
            VerifyExpectedTypes(method, types);
        }

        private void VerifyExpectedTypes(MethodReference method, IEnumerable<ParameterPurpose> types)
        {
            var actualTypes = _framework.GetParameterPurposes(method);
            CollectionAssert.AreEqual(types, actualTypes);
        }
    }
}
