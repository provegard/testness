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
using Mono.Cecil.Cil;
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class ViolationInstructionLocationTest
    {
        private Violation _violation;

        [TestFixtureSetUp]
        public void GivenAViolationCreatedFromAnInstruction()
        {
            var tc = typeof(IntegerCalculatorLocationTest).FindTestCase("TestAdd()");
            _violation = new Violation(new ViolationTest.SomeRule(), tc, 
                tc.TestMethod.Body.Instructions.Where(i => i.OpCode != OpCodes.Nop && i.SequencePoint != null).First());
        }

        [TestCase]
        public void ThenTheDocumentShouldBeExposed()
        {
            StringAssert.EndsWith("\\IntegerCalculatorLocationTest.cs", _violation.DocumentUrl);
        }

        [TestCase]
        public void ThenTheStartLineLocationShouldBeExposed()
        {
            Assert.AreEqual(34, _violation.Location.StartLine);
        }

        [TestCase]
        public void ThenTheEndLineLocationShouldBeExposed()
        {
            Assert.AreEqual(35, _violation.Location.EndLine);
        }

        [TestCase]
        public void ThenTheStartColumnLocationShouldBeExposed()
        {
            Assert.AreEqual(13, _violation.Location.StartColumn);
        }

        [TestCase]
        public void ThenTheEndColumnLocationShouldBeExposed()
        {
            Assert.AreEqual(41, _violation.Location.EndColumn);
        }

        [TestCase]
        public void ThenToStringShouldContainDocumentAndLocation()
        {
            const string expectedEnd = "\\IntegerCalculatorLocationTest.cs(34,13): violation of \"some rule\"";
            StringAssert.EndsWith(expectedEnd, _violation.ToString());
        }
    }
}
