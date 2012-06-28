// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
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
            var tc = TestHelper.FindTestCase<IntegerCalculatorLocationTest>(t => t.TestAdd());
            _violation = new Violation(new ViolationTest.SomeRule(), tc, 
                tc.TestMethod.Body.Instructions.First(i => i.OpCode != OpCodes.Nop && i.SequencePoint != null));
        }

        [TestCase]
        public void ThenTheDocumentShouldBeExposed()
        {
            StringAssert.EndsWith("\\IntegerCalculatorLocationTest.cs", _violation.DocumentUrl);
        }

        [TestCase]
        public void ThenTheStartLineLocationShouldBeExposed()
        {
            Assert.AreEqual(15, _violation.Location.StartLine);
        }

        [TestCase]
        public void ThenTheEndLineLocationShouldBeExposed()
        {
            Assert.AreEqual(16, _violation.Location.EndLine);
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
            const string expectedEnd = "\\IntegerCalculatorLocationTest.cs(15,13): violation of \"some rule\"";
            StringAssert.EndsWith(expectedEnd, _violation.ToString());
        }
    }
}
