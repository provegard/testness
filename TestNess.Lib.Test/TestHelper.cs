// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphBuilder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using TestNess.Lib.Cil;
using TestNess.Lib.TestFramework;
using TestNess.Target;

namespace TestNess.Lib.Test
{
    /// <summary>
    /// Helper class with convenience methods used by TestNess unit tests.
    /// </summary>
    public static class TestHelper
    {
        private static readonly IDictionary<Assembly, TestCases> Repositories = new Dictionary<Assembly, TestCases>();

        /// <summary>
        /// Returns the assembly that unit tests use for testing the evaluation capabilities of
        /// TestNess.
        /// </summary>
        /// <returns>The assembly that contains the target test cases.</returns>
        public static Assembly GetTargetAssembly()
        {
            return typeof(IntegerCalculatorTest).Assembly;
        }

        public static TestCase FindTestCase(this Type type, string methodSignature)
        {
            var assembly = type.Assembly;
            TestCases repository;
            if (!Repositories.TryGetValue(assembly, out repository))
            {
                repository = TestCases.FromAssembly(assembly);
                Repositories.Add(assembly, repository);
            }
            var fullName = type.FullName + "::" + methodSignature;
            return repository.GetTestCaseByName(fullName);
        }

        public static TestCase FindTestCase<T>(Expression<Action<T>> expr)
        {
            var call = expr.Body as MethodCallExpression;
            if (call == null)
                throw new InvalidOperationException("A method call is necessary.");
            var signature = Signature.Of(call.Method).WithoutReturnType();
            return typeof(T).FindTestCase(signature);
        }

        /// <summary>
        /// Extension method that returns a Cecil <see cref="MethodDefinition" /> instances for a method
        /// in a type with the given method signature (excluding return type).
        /// </summary>
        /// <param name="type">The type that contains the method.</param>
        /// <param name="methodSignature">The signature of the method, excluding return type.</param>
        /// <returns>A <see cref="MethodDefinition" /> instance or <c>null</c>.</returns>
        public static MethodDefinition FindMethod(this Type type, string methodSignature)
        {
            var uri = new Uri(type.Assembly.CodeBase);
            var assemblyDef = AssemblyDefinition.ReadAssembly(uri.LocalPath);
            var nameSuffix = type.FullName + "::" + methodSignature;
            var method = (from module in assemblyDef.Modules
                          from t in module.Types
                          from m in t.Methods
                          where m.FullName.EndsWith(nameSuffix)
                          select m).FirstOrDefault();
            return method;
        }

        /// <summary>
        /// Converts a generic collection of type T to a non-generic collection. This is useful since Visual Studio's
        /// unit testing framework doesn't have an assertion class that works with generic collections.
        /// </summary>
        /// <typeparam name="T">The type of the generic collection.</typeparam>
        /// <param name="collection">The generic collection.</param>
        /// <returns>A non-generic collection that contains the items in the generic collection.</returns>
        public static ICollection AsNonGeneric<T>(this ICollection<T> collection)
        {
            return new List<T>(collection);
        }

        public static TestCase GetTestCaseByName(this TestCases repo, string name)
        {
            var result = repo.FirstOrDefault(tc => name.Equals(tc.Name));
            if (result == null)
                throw new NotATestMethodException(name);
            return result;
        }

        public static ICollection<TestCase> GetAllTestCases(this TestCases repo)
        {
            return repo.ToList();
        }

        private static MethodValueTracker.Value FindValue(MethodValueTracker tracker,
            Func<Graph<MethodValueTracker.Value>, MethodValueTracker.Value> func)
        {
            return tracker.ValueGraphs.Select(func).FirstOrDefault(val => val != null);
        }

        private static Func<MethodValueTracker.Value, bool> IsFieldStoreOf(FieldDefinition field)
        {
            return v =>
            {
                if (v.Consumer != null && v.Consumer.OpCode == OpCodes.Stsfld)
                {
                    var stField = v.Consumer.Operand as FieldDefinition;
                    if (stField == field)
                        return true;
                }
                return false;
            };
        }

        private static MethodDefinition FindFunction(TypeDefinition type, MethodValueTracker tracker, MethodValueTracker.Value value)
        {
            var queue = new Queue<MethodValueTracker.Value>(new[] { value });
            MethodDefinition funcDef = null;
            while (queue.Count > 0)
            {
                var val = queue.Dequeue();

                if (val.Producer.OpCode == OpCodes.Ldftn)
                {
                    // Something like:
                    // ldftn      instance bool TestNess.Lib.Test.TestCaseTest/'<>c__DisplayClasse'::'<TestFoo>b__d'(int32)
                    funcDef = val.Producer.Operand as MethodDefinition;
                    break;
                }
                if (val.Producer.OpCode == OpCodes.Ldsfld)
                {
                    var field = val.Producer.Operand as FieldDefinition;

                    // Find:
                    // stsfld     class [mscorlib]System.Func`2<int32,bool> TestNess.Lib.Test.TestCaseTest::'CS$<>9__CachedAnonymousMethodDelegate11'

                    Func<Graph<MethodValueTracker.Value>, MethodValueTracker.Value> valueFinder = 
                        graph => graph.Walk().FirstOrDefault(IsFieldStoreOf(field));

                    // First try the method itself
                    var xval = FindValue(tracker, valueFinder);
                    if (xval == null)
                    {
                        // Next try the static constructor of the type
                        var staticConstructor = type.GetStaticConstructor();
                        if (staticConstructor != null)
                        {
                            var tracker2 = new MethodValueTracker(staticConstructor);
                            xval = FindValue(tracker2, valueFinder);
                        }
                    }
                    if (xval == null)
                    {
                        throw new InvalidOperationException("Failed to find storing instruction.");
                    }

                    funcDef = FindFunction(type, tracker, xval);
                    if (funcDef != null)
                        break;
                }

                foreach (var parent in val.Parents)
                    queue.Enqueue(parent);
            }
            return funcDef;
        }

        private static MethodDefinition FindMethodOfDelegate(Delegate d, int stackFrameIndexDiff)
        {
            var st = new StackTrace();
            var frame = st.GetFrame(1 + stackFrameIndexDiff);

            var callingMethod = frame.GetMethod();
            var callingType = callingMethod.DeclaringType;

            var path = new Uri(callingType.Assembly.CodeBase).LocalPath;
            var assemblyDef = AssemblyLoader.Load(path);
            var mm = assemblyDef.MainModule;

            var typeOfCaller = mm.Types.First(t => t.FullName == callingType.FullName);
            var meth = typeOfCaller.Methods.First(m => m.MetadataToken.ToInt32() == callingMethod.MetadataToken).Resolve();
            var myMetadataToken = st.GetFrame(0 + stackFrameIndexDiff).GetMethod().MetadataToken;
            var callingInstruction =
                meth.Body.Instructions.Where(ins => ins.OpCode == OpCodes.Call).FirstOrDefault(ins => (ins.Operand as MethodReference).MetadataToken.ToInt32() == myMetadataToken);
            var tracker = new MethodValueTracker(meth);

            var value = FindValue(tracker, graph =>
            {
                var consumedValues =
                    tracker.GetConsumedValues(graph, callingInstruction).ToList();
                return consumedValues.Count > 0 ? consumedValues[0] : null;
            });
            var funcDef = FindFunction(typeOfCaller, tracker, value);

            if (funcDef == null)
                throw new InvalidOperationException("Failed to find method for " + d);
            return funcDef;
        }

        public static TestCase AsTestCase(this Delegate d, ITestFramework framework)
        {
            return new TestCase(FindMethodOfDelegate(d, 1), framework.AsRelated());
        }

        public static MethodDefinition AsMethodDef(this Delegate d)
        {
            return FindMethodOfDelegate(d, 1);
        }
    }
}
