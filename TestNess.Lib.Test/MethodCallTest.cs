using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class MethodCallTest
    {
        private void Bar()
        {
        }
        private void Foo()
        {
            // Same assembly, will be MethodDefinition
            Bar();
        }

        private void Baz()
        {
            // Other assembly, will be MethodReference (not resolved)
            Environment.Exit(0);
        }

        [Test]
        public void ShouldExposeInstruction()
        {
            var methodDef = GetType().FindMethod("Foo()");
            var call = methodDef.CalledMethods().First();
            Assert.NotNull(call.Instruction);
        }

        [Test]
        public void ShouldExposeMethod()
        {
            var methodDef = GetType().FindMethod("Foo()");
            var call = methodDef.CalledMethods().First();
            Assert.NotNull(call.MethodReference);
        }

        [Test]
        public void ShouldIndicateWhenItHasAMethodDefinition()
        {
            var methodDef = GetType().FindMethod("Foo()");
            var call = methodDef.CalledMethods().First();
            Assert.True(call.HasMethodDefinition);
        }

        [Test]
        public void ShouldReturnTheMethodDefinition()
        {
            var methodDef = GetType().FindMethod("Foo()");
            var call = methodDef.CalledMethods().First();
            Assert.NotNull(call.MethodDefinition);
        }

        [Test]
        public void ShouldIndicateWhenItDoesntHaveAMethodDefinition()
        {
            var methodDef = GetType().FindMethod("Baz()");
            var call = methodDef.CalledMethods().First();
            Assert.False(call.HasMethodDefinition);
        }

        [Test]
        public void ShouldThrowWhenMethodDefinitionIsRequestedButHasReference()
        {
            var methodDef = GetType().FindMethod("Baz()");
            var call = methodDef.CalledMethods().First();
            object x;
            Assert.Catch<InvalidOperationException>(() => x = call.MethodDefinition);
        }
    }
}
