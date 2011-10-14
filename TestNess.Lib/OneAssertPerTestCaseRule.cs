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
using Mono.Cecil.Cil;
using GraphBuilder;

namespace TestNess.Lib
{
    public class OneAssertPerTestCaseRule : IRule
    {
        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            IList<IList<MethodReference>> paths = new List<IList<MethodReference>>();
            testCase.CallGraph.Walk(reference => AddPathsToRoot(testCase, reference, paths));
            var calledMethodsForAsserting = paths.Select(path => path[path.Count - 2]);
            if (calledMethodsForAsserting.Count() == 1)
            {
                yield break;
            }
            yield return new Violation(this, testCase);
        }

        private static void AddPathsToRoot(TestCase testCase, MethodReference reference, ICollection<IList<MethodReference>> listOfPaths)
        {
            if (CountAssertions(reference) <= 0)
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

        private static int CountAssertions(MethodReference method)
        {
            MethodDefinition methodDef;
            var count = 0;
            if (method.IsDefinition && (methodDef = (MethodDefinition) method).HasBody)
            {
                var assertions = methodDef.Body.Instructions.Where(IsAssertion);
                count = assertions.Count();
            }
            return count;
        }

        private static bool IsAssertion(Instruction inst)
        {
            var isAssert = false;
            if (inst.OpCode == OpCodes.Throw)
            {
                var prev = inst.Previous;
                if (prev.OpCode == OpCodes.Newobj)
                {
                    var reference = (MethodReference) prev.Operand;
                    //TODO: hard-coded to MsTest for now!
                    isAssert = "Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException".Equals(
                        reference.DeclaringType.FullName);
                }
            }
            return isAssert;
        }

        public override string ToString()
        {
            return "a test case should have a single assert";
        }
    }
}
