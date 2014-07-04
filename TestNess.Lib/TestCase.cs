// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GraphBuilder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TestNess.Lib.Cil;
using TestNess.Lib.TestFramework;

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
        private readonly ITestFramework _framework = TestFrameworks.Instance;
        private TestCaseOrigin _origin;

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
        /// <param name="testCaseOrigin">Contains origin information about the test case. Not
        /// mandatory.</param>
        public TestCase(MethodDefinition method, TestCaseOrigin testCaseOrigin = null)
        {
            TestMethod = method;
            _origin = testCaseOrigin;
            CallGraph = new GraphBuilder<MethodReference>(CalledMethodsFinder).Build(method);
        }

        /// <summary>
        /// Exposes the instruction graph of the test method that contains this test case.
        /// </summary>
        public Graph<Instruction> GetInstructionGraph()
        {
            return InstructionGraph.CreateFrom(TestMethod);
        }

        /// <summary>
        /// Contains information about the assembly in which the test case exists.
        /// </summary>
        public TestCaseOrigin Origin
        {
            get
            {
                if (_origin == null)
                {
                    _origin = new TestCaseOrigin(TestMethod.DeclaringType.Module.Assembly, null);
                }
                return _origin;
            }
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
        
        private IEnumerable<IList<MethodReference>> AddPathsToRoot(MethodReference reference)
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
