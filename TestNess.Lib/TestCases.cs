// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
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
        private readonly string _fileName;

        /// <summary>
        /// Creates a new test case repository that fetches test cases from the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains test cases (in the form of test methods).</param>
        /// <param name="fileName">The name of the file from which the assembly was loaded.</param>
        private TestCases(AssemblyDefinition assembly, string fileName)
        {
            _assembly = assembly;
            _fileName = fileName;
            _framework = TestFrameworks.Instance;
            _enumerable = CreateEnumerable();
        }

        private IEnumerable<TestCase> CreateEnumerable()
        {
            return
                _assembly.MainModule.Types.SelectMany(type => type.Methods).Where(_framework.IsTestMethod).Select(
                    m => new TestCase(m, new TestCaseOrigin(_assembly, _fileName)));
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

            return new TestCases(assemblyDef, fileName);
        }

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
