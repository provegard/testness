using System;
using System.Linq;
using Mono.Cecil.Cil;
using NUnit.Framework;
using TestNess.Lib.TestFramework;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class TestHelperTest
    {
        private static Action sa = () => Console.WriteLine("sfoo");

        private Func<Instruction, bool> IsLoadStringWith(string s)
        {
            return ins => ins.OpCode == OpCodes.Ldstr && ((string) ins.Operand) == s;
        }

        private string InstructionToString(Instruction i)
        {
            return i.ToString().Split(new[] {": "}, 2, StringSplitOptions.None)[1];
        }

        [Test]
        public void ShouldProduceTestCaseFromStaticAction()
        {
            // IL_0000:  ldstr      "foo"
            Action a = () => Console.WriteLine("foo");
            var tc = a.AsTestCase(new NUnitTestFramework());

            var instructionStrings = tc.TestMethod.Body.Instructions.Select(InstructionToString);
            CollectionAssert.Contains(instructionStrings, "ldstr \"foo\"");
        }

        [Test]
        public void ShouldProduceTestCaseFromInstanceAction()
        {
            // IL_0000:  ldstr      "foo"
            var x = 5;
            Action a = () => Console.WriteLine("bar" + x);
            var tc = a.AsTestCase(new NUnitTestFramework());

            var instructionStrings = tc.TestMethod.Body.Instructions.Select(InstructionToString);
            CollectionAssert.Contains(instructionStrings, "ldstr \"bar\"");
        }

        [Test]
        public void ShouldProduceTestCaseFromStaticMemberAction()
        {
            var tc = sa.AsTestCase(new NUnitTestFramework());

            var instructionStrings = tc.TestMethod.Body.Instructions.Select(InstructionToString);
            CollectionAssert.Contains(instructionStrings, "ldstr \"sfoo\"");
        }
    }
}
