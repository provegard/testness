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
using System.IO;
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
        private readonly IDictionary<string, TestCase> _testCases = new Dictionary<string, TestCase>();

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
                _testCases.Add(TestCase.GetTestCaseName(method), new TestCase(method));
            }
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
        /// <returns>A test case instance.</returns>
        /// <exception cref="NotATestMethodException">If the method name does not refer to a recognized test method,
        /// or if there is no method at all by that name.</exception>
        public TestCase GetTestCaseByName(string testMethodName)
        {
            TestCase testCase;
            if (!_testCases.TryGetValue(testMethodName, out testCase))
            {
                throw new NotATestMethodException(testMethodName);
            }
            return testCase;
        }

        /// <summary>
        /// Fetches all test cases found in the current assembly. If there are no test cases in the assembly, an
        /// empty collection is returned.
        /// </summary>
        /// <returns>A collection of test cases.</returns>
        public ICollection<TestCase> GetAllTestCases()
        {
            return _testCases.Values;
        }

        /// <summary>
        /// Loads a test case repository from file. More precisely, loads an assembly from file and creates a 
        /// test case repository for the assembly.
        /// </summary>
        /// <param name="fileName">The path to the assembly file.</param>
        /// <returns>A test case repository that fetches test cases from the loaded assembly.</returns>
        public static TestCaseRepository LoadFromFile(string fileName)
        {
            var assemblyDef = AssemblyDefinition.ReadAssembly(fileName);
            return new TestCaseRepository(assemblyDef);
        }
    }
}
