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

using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace TestNess.Lib
{
    /// <summary>
    /// Class that represents a repository of unit test cases. The repository treats an assembly (represented by a
    /// Cecil <see cref="AssemblyDefinition"/> instance) as a database, and fetches "virtual" test cases based on 
    /// test methods identified in the assembly. The ID/name of a test case is the full name (excluding return type) 
    /// of the test method that contains the test case.
    /// </summary>
    public class TestCaseRepository
    {
        private readonly AssemblyDefinition _assembly;
        private readonly IDictionary<string, MethodDefinition> _testMethods = new Dictionary<string, MethodDefinition>(); 

        /// <summary>
        /// Creates a new test case repository that fetches test cases from the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains test cases (in the form of test methods).</param>
        public TestCaseRepository(AssemblyDefinition assembly)
        {
            _assembly = assembly;
            BuildTestMethodDictionary();
        }

        private void BuildTestMethodDictionary()
        {
            var methods = from module in _assembly.Modules
                          from type in module.Types
                          from method in type.Methods
                          where IsTestMethod(method)
                          select method;

            foreach (var method in methods)
            {
                _testMethods.Add(GetMethodNameWithoutReturnType(method), method);
            }
        }

        private string GetMethodNameWithoutReturnType(MethodDefinition method)
        {
            // FullName includes the return type, which is not interesting from
            // an identification perspective, so lets strip it!
            var name = method.FullName;
            name = name.Substring(name.IndexOf(' ') + 1);
            return name;
        }

        private bool IsTestMethod(MethodDefinition method)
        {
            return HasTestAttribute(method);
        }

        private static bool HasTestAttribute(MethodDefinition method)
        {
            return method.CustomAttributes.Where(IsTestAttribute).Count() > 0;
        }

        private static bool IsTestAttribute(CustomAttribute attr)
        {
            // TODO: hard-coded for now, but will be refactored later
            return "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute".Equals(attr.AttributeType.FullName);
        }

        /// <summary>
        /// Fetches a test case by its name. The name (ID) of a test case is the full name of the test method that
        /// contains the test case, minus the return type.
        /// <example>
        /// var testCase = repository.GetTestCaseByName("Some.Assembly::TestFoo()");
        /// </example>
        /// </summary>
        /// <param name="testMethodName">The full name (excluding return type) of the test method that contains
        /// the test case.</param>
        /// <returns>A test case object.</returns>
        /// <exception cref="NotATestMethodException">If the method name does not refer to a recognized test method,
        /// or if there is no method at all by that name.</exception>
        public object GetTestCaseByName(string testMethodName)
        {
            MethodDefinition method;
            if (!_testMethods.TryGetValue(testMethodName, out method))
            {
                throw new NotATestMethodException(testMethodName);
            }
            return method; //TODO: return a TestCase here!
        }
    }
}
