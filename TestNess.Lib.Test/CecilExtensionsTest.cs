// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Linq;
using NUnit.Framework;
using Mono.Cecil;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class CecilExtensionsTest
    {
        private static TypeDefinition _typeDefinition;

        [TestFixtureSetUp]
        public void ClassSetup()
        {
            var assembly = typeof(TesterClass).Assembly;
            var uri = new Uri(assembly.CodeBase);
            var assemblyDef = AssemblyDefinition.ReadAssembly(uri.LocalPath);
            _typeDefinition = (from t in assemblyDef.MainModule.Types
                               where t.Name.Equals(typeof(TesterClass).Name)
                               select t).First();
        }

        private static MethodDefinition GetReferringMethod(string name)
        {
            return (from m in _typeDefinition.Methods
                    where m.Name.Equals(name)
                    select m).First();
        }

        [Test]
        public void TestThatCalledMethodCanBeExtracted()
        {
            var calledMethods = GetReferringMethod("ReferringMethod").CalledMethods();
            CollectionAssert.Contains(calledMethods.Select(m => m.Method.Name).ToList(), "AMethod");
        }

        [Test]
        public void TestThatCalledMethodsFromAbstractClassAreEmpty()
        {
            var calledMethods = GetReferringMethod("AnAbstractMethod").CalledMethods();
            Assert.AreEqual(0, calledMethods.Count());
        }
    }

    public abstract class TesterClass
    {
        public void ReferringMethod()
        {
            AMethod();
        }

        public void AMethod()
        {
        }

        public abstract void AnAbstractMethod();
    }
}
