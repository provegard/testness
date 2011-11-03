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
using System.Collections.ObjectModel;
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
        private ICollection<MethodDefinition> _assertingMethods;
        private readonly ITestFramework _framework = new TestFrameworks();

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
            return definition.CalledMethods().Select(cm => cm.Method).Where(r => !r.Name.Equals(".ctor")).Select(TryResolve);
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
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(null, obj) || !(obj is TestCase)) return false;
            var testCase = (TestCase)obj;
            return ReferenceEquals(testCase.TestMethod, TestMethod);
        }

        public override int GetHashCode()
        {
            return TestMethod.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("TestCase [{0}]", Name);
        }

        /// <summary>
        /// Returns a collection of methods that are called by the test method that contains this test case,
        /// and that eventually contain an assertion. Thus "asserting methods". Note that if the same 
        /// assering method is called multiple times, it will be included multiple times in the collection.
        /// </summary>
        /// <returns>A collection of methods.</returns>
        public ICollection<MethodDefinition> GetCalledAssertingMethods()
        {
            if (_assertingMethods != null)
                return _assertingMethods;
            var paths = CallGraph.Walk().SelectMany(AddPathsToRoot);
            // It's safe to cast to MethodDefinition, since if the method wasn't resolved, we
            // hadn't been able to determine that it was an asserting method.
            var definitions = paths.Select(path => path[path.Count - 2] as MethodDefinition);
            _assertingMethods = new ReadOnlyCollection<MethodDefinition>(definitions.ToList());
            return _assertingMethods;
        }
        
        private IEnumerable<IList<MethodReference>> AddPathsToRoot(MethodReference reference)//, ICollection<IList<MethodReference>> listOfPaths)
        {
            if (!DoesContainAssertion(reference))
                return new IList<MethodReference>[0];
            var graph = CallGraph;
            // Create a graph with edges in the other direction so we can
            // backtrack from this method to the test method (root).
            var builder = new GraphBuilder<MethodReference>(graph.TailsFor);
            var backGraph = builder.Build(reference);
            return backGraph.FindPaths(reference, graph.Root);
        }

        private bool DoesContainAssertion(MethodReference method)
        {
            if (!method.IsDefinition || !((MethodDefinition)method).HasBody)
                return false;
            return _framework.DoesContainAssertion((MethodDefinition)method);
        }
    }
}
