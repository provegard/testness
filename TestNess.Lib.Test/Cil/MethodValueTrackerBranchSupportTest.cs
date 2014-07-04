// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
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
        [Test]
        public void TestThatTrackerExposesTwoValueGraphsForMethodWithTwoPaths()
        {
            var method = GetType().FindMethod("MethodWithBranch()");
            var tracker = new MethodValueTracker(method);
            Assert.AreEqual(2, tracker.ValueGraphs.Count);
        }
        
        [Test]
        public void TestThatTrackerExposesValueGraphsForAllCasesOfSwitchStatement()
        {
            var method = GetType().FindMethod("MethodWithSwitch()");
            var tracker = new MethodValueTracker(method);
            Assert.AreEqual(3, tracker.ValueGraphs.Count);
        }

        [Test]
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

        [Test]
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
