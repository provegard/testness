﻿// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GraphBuilder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TestNess.Lib.Cil;
using TestNess.Lib.Feature;
using TestNess.Lib.TestFramework;

namespace TestNess.Lib
{
    /// <summary>
    /// Class that represents a single unit test case to be evaluated by TestNess. A instance of this class 
    /// encapsulates a Cecil <see cref="MethodDefinition"/> instance, which is the method that contains (or
    /// defines, if you will) the test case.
    /// </summary>
    public class TestCase
    {
        private ICollection<MethodCall> _assertingMethods;
        private TestCaseOrigin _origin;
        private Features _features;

        /// <summary>
        /// The test method that contains this test case.
        /// </summary>
        public MethodDefinition TestMethod { get; private set; }

        /// <summary>
        /// Exposes the call graph for the test method that contains this test case. The root of the
        /// call graph is the test method.
        /// </summary>
        public Graph<MethodCall> CallGraph { get; private set; }

        /// <summary>
        /// The name of this test case. The name is the name of the test method without the return type.
        /// </summary>
        public string Name
        {
            get { return GetTestCaseName(TestMethod); }
        }

        /// <summary>
        /// The test framework that this test case belongs to.
        /// </summary>
        public ITestFramework Framework { get; private set; }

        private readonly IEnumerable<IMockFramework> _mockFrameworks;

        public Features Features
        {
            get { return _features ?? (_features = new Features(this)); }
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
        /// <param name="frameworks">Test and mock frameworks for this test case.</param>
        public TestCase(MethodDefinition method, RelatedFrameworks frameworks, TestCaseOrigin testCaseOrigin = null)
        {
            TestMethod = method;
            _origin = testCaseOrigin;
            CallGraph = new GraphBuilder<MethodCall>(CalledMethodsFinder).Build(new MethodCall(null, method));
            Framework = frameworks != null ? frameworks.TestFramework : null;
            _mockFrameworks = frameworks != null ? frameworks.MockFrameworks : new IMockFramework[0];
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

        private static IEnumerable<MethodCall> CalledMethodsFinder(MethodCall call)
        {
            if (!call.HasMethodDefinition)
                return new MethodCall[0]; // no body to parse anyway
            var definition = call.MethodDefinition;
            return definition.CalledMethods().Where(mc => !mc.MethodReference.Name.Equals(".ctor")).Select(TryResolve);
        }

        private static MethodCall TryResolve(MethodCall mc)
        {
            if (mc.HasMethodDefinition)
                return mc;
            MethodReference result;
            try
            {
                result = mc.MethodReference.Resolve();
            }
            catch (AssemblyResolutionException)
            {
                // resolution is best-effort, keep the reference
                result = mc.MethodReference;
            }
            return new MethodCall(mc.Instruction, result);
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
        public ICollection<MethodCall> GetCalledAssertingMethods()
        {
            if (_assertingMethods != null)
                return _assertingMethods;
            var paths = CallGraph.Walk().SelectMany(AddPathsToRoot);
            // It's safe to cast to MethodDefinition, since if the method wasn't resolved, we
            // hadn't been able to determine that it was an asserting method.
            var calls = paths.Select(path => path[path.Count - 2]);
            _assertingMethods = new ReadOnlyCollection<MethodCall>(calls.ToList());
            return _assertingMethods;
        }

        private IEnumerable<IList<MethodCall>> AddPathsToRoot(MethodCall call)
        {
            if (!DoesContainAssertion(call))
                return new IList<MethodCall>[0];
            var graph = CallGraph;
            // Create a graph with edges in the other direction so we can
            // backtrack from this method to the test method (root).
            var builder = new GraphBuilder<MethodCall>(graph.TailsFor);
            var backGraph = builder.Build(call);
            return backGraph.FindPaths(call, graph.Root);
        }

        private bool DoesContainAssertion(MethodCall methodCall)
        {
            if (!methodCall.HasMethodDefinition || !methodCall.MethodDefinition.HasBody)
                return false;
            var frameworks = new[] {Framework}.Concat(_mockFrameworks);
            return frameworks.Any(fw => fw.DoesContainAssertion(methodCall.MethodDefinition));
        }
    }
}
