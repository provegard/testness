// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
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
            var signature = call.Method.ToString();
            // strip the return type
            signature = signature.Substring(signature.IndexOf(' ') + 1);
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
    }
}
