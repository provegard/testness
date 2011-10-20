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

using NUnit.Framework;
using TestNess.Target;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class MSTestTestFrameworkTest
    {
        [TestCase]
        public void TestThatMethodIsDetectedAsTestMethod()
        {
            var framework = new MSTestTestFramework();
            var method = typeof (IntegerCalculatorTest).FindMethod("TestAddBasic()");
            Assert.IsTrue(framework.IsTestMethod(method));
        }

        [TestCase]
        public void TestThatMethodIsDetectedAsNonTestMethod()
        {
            var framework = new MSTestTestFramework();
            var method = typeof(IntegerCalculator).FindMethod("Add(System.Int32,System.Int32)");
            Assert.IsFalse(framework.IsTestMethod(method));
        }

        [TestCase]
        public void TestThatExpectedExceptionIsIdentified()
        {
            var framework = new MSTestTestFramework();
            var method = typeof(IntegerCalculatorTest).FindMethod("TestDivideWithException()");
            Assert.IsTrue(framework.HasExpectedException(method));
        }

        [TestCase]
        public void TestThatNonExpectedExceptionIsIdentified()
        {
            var framework = new MSTestTestFramework();
            var method = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            Assert.IsFalse(framework.HasExpectedException(method));
        }

        [TestCase]
        public void TestThatAssertionThrowingMethodIsDetected()
        {
            var framework = new MSTestTestFramework();
            var method =
                typeof (Microsoft.VisualStudio.TestTools.UnitTesting.Assert).FindMethod(
                    "HandleFail(System.String,System.String,System.Object[])");
            Assert.IsTrue(framework.DoesContainAssertion(method));
        }

        [TestCase]
        public void TestThatNonAssertionThrowingMethodIsDetected()
        {
            var framework = new MSTestTestFramework();
            var method = typeof(IntegerCalculatorTest).FindMethod("TestAddBasic()");
            Assert.IsFalse(framework.DoesContainAssertion(method));
        }
    }
}
