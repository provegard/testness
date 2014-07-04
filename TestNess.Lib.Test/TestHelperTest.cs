using System;
using System.Linq;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class TestHelperTest
    {
        private Func<Instruction, bool> IsLoadStringWith(string s)
        {
            return ins => ins.OpCode == OpCodes.Ldstr && ((string) ins.Operand) == s;
        }

        [Test]
        public void ShouldProduceTestCaseFromStaticAction()
        {
            // IL_0000:  ldstr      "foo"
            Action a = () => Console.WriteLine("foo");
            var tc = a.AsTestCase();

            Assert.True(tc.TestMethod.Body.Instructions.Any(IsLoadStringWith("foo")));
        }

        [Test]
        public void ShouldProduceTestCaseFromInstanceAction()
        {
            // IL_0000:  ldstr      "foo"
            var x = 5;
            Action a = () => Console.WriteLine("foo" + x);
            var tc = a.AsTestCase();

            Assert.True(tc.TestMethod.Body.Instructions.Any(IsLoadStringWith("foo")));
        }
    }
}
