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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;

namespace TestNess.Lib.Test
{
    [TestClass]
    public class CecilExtensionsTest
    {
        private static TypeDefinition _typeDefinition;

        [ClassInitialize]
        public static void ClassSetup(TestContext ctx)
        {
            var assemblyDef = typeof(TesterClass).GetAssemblyDefinition();
            _typeDefinition = (from m in assemblyDef.Modules
                               from t in m.Types
                               where t.Name.Equals(typeof(TesterClass).Name)
                               select t).First();
        }

        private static MethodDefinition GetReferringMethod(string name)
        {
            return (from m in _typeDefinition.Methods
                    where m.Name.Equals(name)
                    select m).First();
        }

        [TestMethod]
        public void TestThatCalledMethodCanBeExtracted()
        {
            var calledMethods = GetReferringMethod("ReferringMethod").CalledMethods();
            CollectionAssert.Contains(calledMethods.Select(m => m.Name).ToList(), "AMethod");
        }

        [TestMethod]
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
