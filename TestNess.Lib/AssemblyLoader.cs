// Copyright (C) 2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2014.
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;

namespace TestNess.Lib
{
    public static class AssemblyLoader
    {
        public static AssemblyDefinition Load(string fileName)
        {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(new FileInfo(fileName).DirectoryName);
            var parameters = new ReaderParameters
            {
                SymbolReaderProvider = new PdbReaderProvider(),
                ReadingMode = ReadingMode.Immediate,
                AssemblyResolver = resolver
            };
            AssemblyDefinition assemblyDef;
            try
            {
                assemblyDef = AssemblyDefinition.ReadAssembly(fileName, parameters);
            }
            catch (FileNotFoundException)
            {
                // Perhaps we have an MDB file (Mono), or there is no symbol file to load.
                // Try MDB first!
                parameters.SymbolReaderProvider = new MdbReaderProvider();
                try
                {
                    assemblyDef = AssemblyDefinition.ReadAssembly(fileName, parameters);
                }
                catch (FileNotFoundException)
                {
                    parameters.SymbolReaderProvider = null;
                    assemblyDef = AssemblyDefinition.ReadAssembly(fileName, parameters);
                }
            }
            return assemblyDef;
        }
    }
}
