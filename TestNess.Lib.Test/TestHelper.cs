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
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using TestNess.Target;

namespace TestNess.Lib.Test
{
    /// <summary>
    /// Helper class with convenience methods used by TestNess unit tests.
    /// </summary>
    public static class TestHelper
    {
        private static readonly IDictionary<string, AssemblyDefinition> Assemblies = new Dictionary<string, AssemblyDefinition>();
        private static readonly IDictionary<string, TestCaseRepository> Repositories = new Dictionary<string, TestCaseRepository>(); 

        /// <summary>
        /// Returns the assembly which TestNess unit tests use for testing the evaluation capabilities of
        /// TestNess.
        /// </summary>
        /// <returns>An assembly as a Cecil <see cref="AssemblyDefinition"/> instance.</returns>
        public static AssemblyDefinition GetTargetAssembly()
        {
            return typeof (IntegerCalculatorTest).GetAssemblyDefinition();
        }

        /// <summary>
        /// Extension method that returns a Cecil <see cref="AssemblyDefinition"/> instance for the 
        /// assembly that contains the given type.
        /// </summary>
        /// <returns>The type's assembly as a Cecil <see cref="AssemblyDefinition"/> instance.</returns>
        public static AssemblyDefinition GetAssemblyDefinition(this Type type)
        {
            var assembly = type.Assembly;
            AssemblyDefinition assemblyDef;
            if (!Assemblies.TryGetValue(assembly.Location, out assemblyDef))
            {
                assemblyDef = AssemblyDefinition.ReadAssembly(assembly.Location);
                Assemblies.Add(assembly.Location, assemblyDef);
            }
            return assemblyDef;
        }

        public static TestCase FindTestCase(this Type type, string methodSignature)
        {
            var assemblyDef = type.GetAssemblyDefinition();
            TestCaseRepository repository;
            if (!Repositories.TryGetValue(assemblyDef.FullName, out repository))
            {
                repository = new TestCaseRepository(assemblyDef);
                Repositories.Add(assemblyDef.FullName, repository);
            }
            var fullName = type.FullName + "::" + methodSignature;
            return repository.GetTestCaseByName(fullName);
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
    }
}
