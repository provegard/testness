using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GraphBuilder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;
using TestNess.Lib.Cil;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class Lab
    {
        [Test, Ignore] // Has to be a test case for TestHelper.FindTestCase to pick it up...
        public int LabFun()
        {
            var x = new Random().Next(10);
            if (x < 5)
                return x + 1;
            return x - 1;
        }

        [TestCase(0), Ignore] // Has to be a test case for TestHelper.FindTestCase to pick it up...
        public int Fib(int n)
        {
            if (n <= 1) return n; // F0 = 0, F1 = 1
            int a = 0, b = 1;
            while (--n > 0)
            {
                var tmp = a + b;
                a = b;
                b = tmp;
            }
            return b;
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 3)]
        [TestCase(5, 5)]
        [TestCase(6, 8)]
        public void FibTest(int n, int exp)
        {
            var actual = Fib(n);
            Assert.AreEqual(exp, actual);
        }

        private string ValueToString(MethodValueTracker.Value val)
        {
            if (val.Producer == null)
                return "fake root";
            return string.Format("P:{0} / C:{1}", val.Producer, val.Consumer);
        }

        private void DumpDot(TestCase tc, string baseName)
        {
            var prefix = baseName + "-";
            var tracker = new MethodValueTracker(tc.TestMethod);
            var gen = new GraphVizDotGenerator<Instruction>(tracker.InstructionGraph, ins => "O" + ins.Offset, ins => ins.ToString());
            var dot = gen.CreateDot("body");
            File.WriteAllText(@"c:\temp\dot\" + prefix + "body.dot", dot);

            var vgIdx = 0;
            foreach (var vg in tracker.ValueGraphs)
            {
                var values = new List<MethodValueTracker.Value>();
                Func<MethodValueTracker.Value, string> valueId = v =>
                {
                    var idx = values.IndexOf(v);
                    if (idx >= 0)
                        return "V" + idx;
                    values.Add(v);
                    return "V" + (values.Count - 1);
                };
                var vgGen = new GraphVizDotGenerator<MethodValueTracker.Value>(vg, valueId, ValueToString);
                var vgDot = vgGen.CreateDot("value-" + vgIdx);
                File.WriteAllText(@"c:\temp\dot\" + prefix + "val-" + vgIdx + ".dot", vgDot);
                vgIdx++;
            }
        }

        [Test]
        public void DumpLabFunAsDot()
        {
            var tc = TestHelper.FindTestCase<Lab>(x => x.LabFun());
            DumpDot(tc, "labfun");
        }

        [Test]
        public void DumpInstanceLambdaAsDot()
        {
            var y = 5;
            Func<int, bool> x = a =>
            {
                if (y + a > 5)
                    return true;
                return false;
            };
            var tc = x.AsTestCase();
            DumpDot(tc, "instancelambda");
        }

        [Test]
        public void DumpStaticLambdaAsDot()
        {
            Func<int, bool> x = a =>
            {
                if (a > 5)
                    return true;
                return false;
            };
            var tc = x.AsTestCase();
            DumpDot(tc, "staticlambda");
        }

        [Test]
        public void DumpActionLambdaAsDot()
        {
            Action x = () =>
            {
                Console.WriteLine("fun fun");
            };
            var tc = x.AsTestCase();
            DumpDot(tc, "actionlambda");
        }

        [Test]
        public void TryCatch()
        {
            var x = 5;
            try
            {
                x = x / (x - x);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
