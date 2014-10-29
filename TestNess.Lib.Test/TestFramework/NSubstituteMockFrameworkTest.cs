// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using NSubstitute;
using NUnit.Framework;
using TestNess.Lib.TestFramework;

namespace TestNess.Lib.Test.TestFramework
{
    [TestFixture]
    public class NSubstituteMockFrameworkTest
    {
        private NSubstituteMockFramework _framework;

        [SetUp]
        public void GivenFramework()
        {
            _framework = new NSubstituteMockFramework();
        }

        [TestCase("Received(T)", true)]
        [TestCase("Received(T,System.Int32)", true)]
        [TestCase("ReceivedWithAnyArgs(T)", true)]
        [TestCase("ReceivedWithAnyArgs(T,System.Int32)", true)]
        [TestCase("DidNotReceive(T)", true)]
        [TestCase("DidNotReceiveWithAnyArgs(T)", true)]
        [TestCase("Returns(T,T,T[])", false)]
        public void TestThatNSubstituteExpectationMethodCountsAsAssert(string methodName, bool isAssert)
        {
            var method = typeof (SubstituteExtensions).FindMethod(methodName);
            Assert.That(_framework.DoesContainAssertion(method), Is.EqualTo(isAssert));
        }

        public static void Received()
        {
        }

        [Test]
        public void TestThatWhatLooksLikeExpectationMethodDoesntCountAsAssert()
        {
            var method = typeof(NSubstituteMockFrameworkTest).FindMethod("Received()");
            Assert.That(_framework.DoesContainAssertion(method), Is.False);
        }
    }
}
