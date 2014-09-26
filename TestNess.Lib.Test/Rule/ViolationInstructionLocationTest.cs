﻿// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using System.IO;
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

            foreach (var ins in tc.TestMethod.Body.Instructions)
            {
                Console.Write(ins);
                if (ins.SequencePoint != null)
                {
                    Console.Write(" SP:{0}-{1} {2}-{3}", ins.SequencePoint.StartLine, ins.SequencePoint.EndLine, ins.SequencePoint.StartColumn, ins.SequencePoint.EndColumn);
                }
                Console.WriteLine();
            }

            _violation = new Violation(new ViolationTest.SomeRule(), tc, 
                tc.TestMethod.Body.Instructions.First(i => i.OpCode != OpCodes.Nop && i.SequencePoint != null));
        }

        [Test]
        public void ThenTheDocumentShouldBeExposed()
        {
            StringAssert.EndsWith(Path.DirectorySeparatorChar + "IntegerCalculatorLocationTest.cs", _violation.DocumentUrl);
        }

        [Test]
        public void ThenTheStartLineLocationShouldBeExposed()
        {
            Assert.AreEqual(15, _violation.Location.StartLine);
        }

        [Test]
        public void ThenTheEndLineLocationShouldBeExposed()
        {
            Assert.AreEqual(16, _violation.Location.EndLine);
        }

        [Test]
        public void ThenTheStartColumnLocationShouldBeExposed()
        {
            Assert.AreEqual(13, _violation.Location.StartColumn);
        }

        [Test]
        public void ThenTheEndColumnLocationShouldBeExposed()
        {
            Assert.AreEqual(41, _violation.Location.EndColumn);
        }

        [Test]
        public void ThenToStringShouldContainDocumentAndLocation()
        {
            var expectedEnd = Path.DirectorySeparatorChar + "IntegerCalculatorLocationTest.cs(15,13): violation of \"some rule\"";
            StringAssert.EndsWith(expectedEnd, _violation.ToString());
        }
    }
}
