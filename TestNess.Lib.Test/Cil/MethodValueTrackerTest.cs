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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;
using TestNess.Lib.Cil;
using GraphBuilder;

namespace TestNess.Lib.Test.Cil
{
    [TestFixture]
    public class MethodValueTrackerTest
    {
        [TestCase]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThrowOnNullMethod()
        {
            new MethodValueTracker(null);
        }

        [TestCase]
        public void TestThatConsumedValueCountCorrespondsToActualConsumption()
        {
            var method = GetType().FindMethod("DirectConstantUseByStaticMethod()");
            var tracker = new MethodValueTracker(method);
            var consumedValues = tracker.GetConsumedValues(tracker.ValueGraphs[0], LastCall(method));
            Assert.AreEqual(1, consumedValues.Count());
        }

        [TestCase]
        public void TestThatConsumedConstantIsFound()
        {
            var method = GetType().FindMethod("DirectConstantUseByStaticMethod()");
            var tracker = new MethodValueTracker(method);
            var consumedValue = tracker.GetConsumedValues(tracker.ValueGraphs[0], LastCall(method)).First();

            Assert.AreEqual(OpCodes.Ldc_I4_S, consumedValue.Producer.OpCode);
        }

        [TestCase]
        public void TestThatConstantConsumedViaBoxedValueIsFound()
        {
            var method = GetType().FindMethod("BoxedConstantUseByStaticMethod()");
            var tracker = new MethodValueTracker(method);
            var consumedValue = tracker.GetConsumedValues(tracker.ValueGraphs[0], LastCall(method)).First();
            var roots = tracker.FindSourceValues(consumedValue);

            Assert.AreEqual(OpCodes.Ldc_I4_S, roots.First().Producer.OpCode);
        }

        [TestCase]
        public void TestThatConsumedValuesExcludesThisObjectForVirtualCall()
        {
            var method = GetType().FindMethod("DirectConstantUseByVirtualMethod()");
            var tracker = new MethodValueTracker(method);
            var consumedValues = tracker.GetConsumedValues(tracker.ValueGraphs[0], LastCall(method));
            var opCodes = consumedValues.Select(v => v.Producer.OpCode);

            // Ldarg_0 should not be in the list!
            CollectionAssert.AreEqual(new[] { OpCodes.Ldc_I4_S }.ToList(), opCodes.ToList());
        }

        [TestCase]
        public void TestThatConsumedValuesIncludesFirstArgOfStaticCallInStatic()
        {
            var method = GetType().FindMethod("StaticMethodWithStaticCall(System.Int32)");
            var tracker = new MethodValueTracker(method);
            var consumedValues = tracker.GetConsumedValues(tracker.ValueGraphs[0], LastCall(method));
            var opCodes = consumedValues.Select(v => v.Producer.OpCode);

            // Ldarg_0 should be in the list!
            CollectionAssert.AreEqual(new[] { OpCodes.Ldarg_0 }.ToList(), opCodes.ToList());
        }

        [TestCase]
        public void TestThatSourceValuesAreFoundThroughSequenceOfCalculations()
        {
            var method = GetType().FindMethod("CalculatedValueUseByStaticMethod(System.Int32,System.Int32)");
            var tracker = new MethodValueTracker(method);
            var opCodes = FindAllSourceValueOpCodes(tracker, LastCall(method));

            CollectionAssert.AreEquivalent(new[] { OpCodes.Ldarg_1, OpCodes.Ldarg_2, OpCodes.Ldc_I4_1 }.ToList(), opCodes.ToList());
        }

        [TestCase]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThatQueryInstructionCannotBeNull()
        {
            var method = GetType().FindMethod("DirectConstantUseByStaticMethod()");
            var tracker = new MethodValueTracker(method);
            tracker.GetConsumedValues(tracker.ValueGraphs[0], null);
        }

        [TestCase]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThatGraphCannotBeNullWhenQueryingConsumedValues()
        {
            var method = GetType().FindMethod("DirectConstantUseByStaticMethod()");
            var tracker = new MethodValueTracker(method);
            tracker.GetConsumedValues(null, LastCall(method));
        }

        [TestCase]
        public void TestThatConsumedValuesAreEmptyWhenQueryInstructionIsNotPartOfMethod()
        {
            var method = GetType().FindMethod("DirectConstantUseByStaticMethod()");
            var method2 = GetType().FindMethod("DirectConstantUseByVirtualMethod()");
            var tracker = new MethodValueTracker(method);
            var values = tracker.GetConsumedValues(tracker.ValueGraphs[0], LastCall(method2));
            Assert.AreEqual(0, values.Count());
        }

        [TestCase]
        public void TestThatSourceValueIsTrackedThroughRefManipulation()
        {
            var method = GetType().FindMethod("UseOfValueManipulatedAsReference()");
            var tracker = new MethodValueTracker(method);
            var opCodes = FindAllSourceValueOpCodes(tracker, LastCall(method));

            CollectionAssert.AreEquivalent(new[] { OpCodes.Ldc_I4_4, OpCodes.Ldc_I4_5 }.ToList(), opCodes.ToList());
        }

        [TestCase]
        public void TestThatSourceValueIsTrackedThroughtOutValue()
        {
            var method = GetType().FindMethod("UseOfOutValue()");
            var tracker = new MethodValueTracker(method);
            var opCodes = FindAllSourceValueOpCodes(tracker, LastCall(method));

            CollectionAssert.AreEquivalent(new[] { OpCodes.Ldc_I4_5, OpCodes.Ldc_I4_6 }.ToList(), opCodes.ToList());
        }

        [TestCase]
        public void TestThatSourceValueIsTrackedThroughtOutValueDespitePreviousUseOfOutVariable()
        {
            var method = GetType().FindMethod("UseOfOutValueTwice()");
            var tracker = new MethodValueTracker(method);
            var opCodes = FindAllSourceValueOpCodes(tracker, LastCall(method));

            CollectionAssert.AreEquivalent(new[] { OpCodes.Ldc_I4_1, OpCodes.Ldc_I4_2 }.ToList(), opCodes.ToList());
        }

        [TestCase]
        public void TestThatSourceValueIsTrackedThroughReturnValue()
        {
            var method = GetType().FindMethod("UseOfReturnValue()");
            var tracker = new MethodValueTracker(method);
            var opCodes = FindAllSourceValueOpCodes(tracker, LastCall(method));

            CollectionAssert.AreEquivalent(new[] { OpCodes.Ldc_I4_5, OpCodes.Ldc_I4_6 }.ToList(), opCodes.ToList());
        }

        [TestCase]
        public void TestThatObjectConstructionIsHandled()
        {
            var method = GetType().FindMethod("ObjectConstruction()");
            var tracker = new MethodValueTracker(method);
            var opCodes = FindAllSourceValueOpCodes(tracker, LastCall(method));

            CollectionAssert.AreEquivalent(new[] { OpCodes.Newobj }.ToList(), opCodes.ToList());
        }
        
        private Instruction LastCall(MethodDefinition method)
        {
            return method.Body.Instructions.Where(i => i.OpCode.FlowControl == FlowControl.Call).Last();
        }

        private IEnumerable<OpCode> FindAllSourceValueOpCodes(MethodValueTracker tracker, Instruction instruction)
        {
            var consumedValues = tracker.GetConsumedValues(tracker.ValueGraphs[0], instruction);
            var sourceValues = consumedValues.SelectMany(tracker.FindSourceValues);
            return sourceValues.Select(v => v.Producer.OpCode).Distinct();
        }

        #region Method subjects used in the test cases above

        public static void StaticMethodWithStaticCall(int x)
        {
            StaticMethodWithStaticCall(x);
        }

        public void CalculatedValueUseByStaticMethod(int x, int y)
        {
            var a = x * y;
            var b = x - y;
            var c = a * b;
            StaticObjectConsumer(c - 1);
        }

        public void BoxedConstantUseByStaticMethod()
        {
            // ldc.i4.s 10
            // box
            StaticObjectConsumer(10);
        }

        public void DirectConstantUseByStaticMethod()
        {
            // ldc.i4.s 10
            StaticConsumer(10);
        }

        public void DirectConstantUseByVirtualMethod()
        {
            // ldarg.0
            // ldc.i4.s 10
            VirtualConsumer(10);
        }

        public void UseOfValueManipulatedAsReference()
        {
            var i = 4;
            RefMultiply(ref i, 5);
            StaticConsumer(i);
        }

        public void UseOfOutValue()
        {
            int result;
            OutMultiply(5, 6, out result);
            StaticConsumer(result);
        }

        public void UseOfOutValueTwice()
        {
            int result;
            OutMultiply(5, 6, out result);
            OutMultiply(1, 2, out result);
            StaticConsumer(result);
        }

        public void ObjectConstruction()
        {
            var obj = new MethodValueTrackerTest();
            StaticObjectConsumer(obj);
        }

        public void UseOfReturnValue()
        {
            var result = Multiply(5, 6);
            StaticConsumer(result);
        }

        public static void StaticObjectConsumer(object o)
        {
        }

        public static void StaticConsumer(int i)
        {
        }

        public virtual void VirtualConsumer(int i)
        {
        }

        public static int Multiply(int x, int y)
        {
            return x * y;
        }

        public static void RefMultiply(ref int x, int mul)
        {
            x *= mul;
        }

        public static void OutMultiply(int x, int y, out int result)
        {
            result = x * y;
        }
        #endregion

        #region Helpers for debugging test cases
        internal static string ValueGraphAsString(Graph<MethodValueTracker.Value> graph)
        {
            var builder = new StringBuilder();
            PrintNode(builder, graph.Root, 0);
            return builder.ToString();
        }

        private static void PrintNode(StringBuilder builder, MethodValueTracker.Value node, int indent)
        {
            builder.Append("".PadRight(indent * 2));
            PrintValue(builder, node);
            foreach (var parent in node.Parents)
            {
                PrintNode(builder, parent, indent + 1);
            }
        }

        private static void PrintValue(StringBuilder builder, MethodValueTracker.Value node)
        {
            builder.AppendFormat("Value produced by {0} and consumed by {1} [#parents={2}].\n",
                InstructionToString(node.Producer), InstructionToString(node.Consumer),
                node.Parents.Count);
        }

        private static string InstructionToString(Instruction i)
        {
            if (i == null)
                return "none";
            return string.Format("{0} {1}", i.OpCode, i.Operand);
        }
        #endregion
    }
}
