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
using GraphBuilder;

namespace TestNess.Lib.Rule
{
    public class OneAssertPerTestCaseRule : IRule
    {
        private readonly ITestFramework _framework = new TestFrameworks();

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            IList<IList<MethodReference>> paths = new List<IList<MethodReference>>();
            testCase.CallGraph.Walk(reference => AddPathsToRoot(testCase, reference, paths));
            var assertMethodCount = paths.Select(path => path[path.Count - 2]).Count();
            if (assertMethodCount == 1)
                yield break; // no violation
            if (assertMethodCount == 0 && _framework.HasExpectedException(testCase.TestMethod))
                yield break; // no violation
            yield return new Violation(this, testCase);
        }

        private void AddPathsToRoot(TestCase testCase, MethodReference reference, ICollection<IList<MethodReference>> listOfPaths)
        {
            if (!DoesContainAssertion(reference))
                return;
            var graph = testCase.CallGraph;
            // Create a graph with edges in the other direction so we can
            // backtrack from this method to the test method (root).
            var builder = new GraphBuilder<MethodReference>(graph.TailsFor);
            var backGraph = builder.Build(reference);
            var paths = backGraph.FindPaths(reference, graph.Root);
            foreach (var path in paths)
            {
                listOfPaths.Add(path);
            }
        }

        private bool DoesContainAssertion(MethodReference method)
        {
            if (!method.IsDefinition || !((MethodDefinition)method).HasBody)
                return false;
            return _framework.DoesContainAssertion((MethodDefinition) method);
        }

        public override string ToString()
        {
            return "a test case should have a single assert";
        }
    }
}
