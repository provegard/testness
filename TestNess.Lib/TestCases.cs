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
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Pdb;

namespace TestNess.Lib
{
    /// <summary>
    /// Class that represents a repository of unit test cases. The repository treats an assembly (represented by a
    /// Cecil <see cref="AssemblyDefinition"/> instance) as a database, and fetches "virtual" test cases based on 
    /// test methods identified in the assembly. The ID/name of a test case is the full name (excluding return type) 
    /// of the test method that contains the test case.
    /// </summary>
    public class TestCases : IEnumerable<TestCase>
    {
        private readonly AssemblyDefinition _assembly;
        private readonly ITestFramework _framework;
        private readonly IEnumerable<TestCase> _enumerable;

        /// <summary>
        /// Creates a new test case repository that fetches test cases from the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains test cases (in the form of test methods).</param>
        private TestCases(AssemblyDefinition assembly)
        {
            _assembly = assembly;
            _framework = TestFrameworks.Instance;
            //BuildTestMethodDictionary();
            _enumerable = CreateEnumerable();
        }

        private IEnumerable<TestCase> CreateEnumerable()
        {
            return
                _assembly.MainModule.Types.SelectMany(type => type.Methods).Where(_framework.IsTestMethod).Select(
                    m => new TestCase(m));
        }
        
        /// <summary>
        /// Creates a test case repository from an assembly. The assembly is actually loaded from the
        /// file pointed out by the code base of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly to create a test case repository from.</param>
        /// <returns>A test case repository that fetches test cases from the given assembly.</returns>
        public static TestCases FromAssembly(Assembly assembly)
        {
            if (assembly.GetModules().Length > 1)
                throw new NotImplementedException("Multi-module assemblies not supported yet!");
            
            // Prefer CodeBase over Location. When NUnit is run without /noshadow, it copies
            // the assemblies without the symbol files. CodeBase still points to the original
            // location though.
            var uri = new Uri(assembly.CodeBase);
            return LoadFromFile(uri.LocalPath);
        }

        /// <summary>
        /// Loads a test case repository from file. More precisely, loads an assembly from file and creates a 
        /// test case repository for the assembly.
        /// </summary>
        /// <param name="fileName">The path to the assembly file.</param>
        /// <returns>A test case repository that fetches test cases from the loaded assembly.</returns>
        public static TestCases LoadFromFile(string fileName)
        {
            var parameters = new ReaderParameters
            {
                SymbolReaderProvider = new PdbReaderProvider(),
                ReadingMode = ReadingMode.Immediate,
            };
            AssemblyDefinition assemblyDef;
            try
            {
                assemblyDef = AssemblyDefinition.ReadAssembly(fileName, parameters);
            } 
            catch (FileNotFoundException)
            {
                // Might be the PDB file that is missing!
                assemblyDef = AssemblyDefinition.ReadAssembly(fileName);
            }
            if (assemblyDef.Modules.Count > 1)
                throw new NotImplementedException("Multi-module assemblies not supported yet!");

            return new TestCases(assemblyDef);
        }

        //public AssemblyDefinition Assembly
        //{
        //    get { return _assembly; }
        //}

        public IEnumerator<TestCase> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
