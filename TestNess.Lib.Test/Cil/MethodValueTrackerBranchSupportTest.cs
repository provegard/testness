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

using System;
using System.Linq;
using Mono.Cecil.Cil;
using NUnit.Framework;
using TestNess.Lib.Cil;

namespace TestNess.Lib.Test.Cil
{
    [TestFixture]
    public class MethodValueTrackerBranchSupportTest
    {
        [TestCase]
        public void TestThatTrackerExposesTwoValueGraphsForMethodWithTwoPaths()
        {
            var method = GetType().FindMethod("MethodWithBranch()");
            var tracker = new MethodValueTracker(method);
            Assert.AreEqual(2, tracker.ValueGraphs.Count);
        }
        
        [TestCase]
        public void TestThatTrackerExposesValueGraphsForAllCasesOfSwitchStatement()
        {
            var method = GetType().FindMethod("MethodWithSwitch()");
            var tracker = new MethodValueTracker(method);
            Assert.AreEqual(3, tracker.ValueGraphs.Count);
        }

        [TestCase]
        public void TestThatValueConsumptionForInstructionMatchesPath()
        {
            var method = GetType().FindMethod("MethodWithBranch()");
            var tracker = new MethodValueTracker(method);
            var graph = tracker.ValueGraphs[1]; // second path
            // Find the last instruction that stores to y (first local)
            var instr = method.Body.Instructions.Where(i => i.OpCode == OpCodes.Stloc_0).Last();
            var producers = tracker.GetConsumedValues(graph, instr).Select(v => v.Producer.OpCode).ToList();
            // Expect the producer of the conssumed value to load from u (third local)
            CollectionAssert.AreEqual(new[] { OpCodes.Ldloc_2 }, producers);
        }

        [TestCase]
        public void TestThatValueConsumptionForInstructionOutsidePathIsEmpty()
        {
            var method = GetType().FindMethod("MethodWithBranch()");
            var tracker = new MethodValueTracker(method);
            var graph = tracker.ValueGraphs[0]; // first path
            // Find the last instruction that stores to y, part of second path
            var instr = method.Body.Instructions.Where(i => i.OpCode == OpCodes.Stloc_0).Last();

            var consumed = tracker.GetConsumedValues(graph, instr);
            Assert.AreEqual(0, consumed.Count());
        }

        #region Method subjects used in the test cases above

        public int MethodWithBranch()
        {
            int y;
            var x = new Random().Next(2); // 0 or 1
            var u = new Random().Next(2); // 0 or 1
            if (x > 0)
                y = x;
            else
                y = u;
            return y;
        }

        public int MethodWithSwitch()
        {
            int y;
            var x = new Random().Next(3); // 0 to 2
            switch (x)
            {
                case 0:
                    y = 0;
                    break;
                case 1:
                    y = 1;
                    break;
                default:
                    y = 2;
                    break;
            }
            return y;
        }

        #endregion
    }
}
