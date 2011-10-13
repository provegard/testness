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
using GraphBuilder;
using Mono.Cecil;

namespace TestNess.Lib
{
    /// <summary>
    /// Class that represents a single unit test case to be evaluated by TestNess. A instance of this class 
    /// encapsulates a Cecil <see cref="MethodDefinition"/> instance, which is the method that contains (or
    /// defines, if you will) the test case.
    /// <para>
    /// This is a DDD aggregate root, which means that clients should not store data fetched from an 
    /// instance of this class, other than temporarily.
    /// </para>
    /// </summary>
    public class TestCase
    {
        /// <summary>
        /// The test method that contains this test case.
        /// </summary>
        public MethodDefinition TestMethod { get; private set; }

        /// <summary>
        /// Exposes the call graph for the test method that contains this test case. The root of the
        /// call graph is the test method.
        /// </summary>
        public Graph<MethodReference> CallGraph { get; private set; }

        /// <summary>
        /// The name of this test case. The name is the name of the test method without the return type.
        /// </summary>
        public string Name
        {
            get { return GetTestCaseName(TestMethod); }
        }

        internal static string GetTestCaseName(MethodDefinition method)
        {
            // FullName includes the return type, which is not interesting from
            // an identification perspective, so lets strip it!
            var name = method.FullName;
            name = name.Substring(name.IndexOf(' ') + 1);
            return name;
        }

        /// <summary>
        /// Creates an instance of this class based on a method. The method is assumed to be a test method.
        /// </summary>
        /// <param name="method">The method that contains/defines the test case.</param>
        public TestCase(MethodDefinition method)
        {
            TestMethod = method;
            CallGraph = new GraphBuilder<MethodReference>(CalledMethodsFinder).Build(method);
        }

        private static IEnumerable<MethodReference> CalledMethodsFinder(MethodReference reference)
        {
            if (!reference.IsDefinition)
                return new MethodReference[0]; // no body to parse anyway
            var definition = (MethodDefinition) reference;
            return definition.CalledMethods().Where(r => !r.Name.Equals(".ctor")).Select(TryResolve);
        }

        private static MethodReference TryResolve(MethodReference reference)
        {
            if (reference.IsDefinition)
                return reference;
            MethodReference result;
            try
            {
                result = reference.Resolve();
            }
            catch (AssemblyResolutionException)
            {
                // resolution is best-effort, keep the reference
                result = reference;
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == this) return true;
            if (obj == null || !(obj is TestCase)) return false;
            var testCase = (TestCase)obj;
            return testCase.TestMethod == TestMethod;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var hashCode = 1;
            hashCode += prime * TestMethod.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return string.Format("TestCase [{0}]", Name);
        }
    }
}
