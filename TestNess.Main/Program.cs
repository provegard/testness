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
using System.Reflection;
using System.IO;
using TestNess.Lib;
using TestNess.Lib.Rule;

namespace TestNess.Main
{
    class Program
    {
        static int Main(string[] args)
        {
            TestCaseRepository repo;
            PrintHeader();
            var arguments = Arguments.Parse(args);
            if (!arguments.HasAssemblyFileName)
            {
                PrintUsage();
                return 1;
            }
            if (!LoadRepository(arguments.AssemblyFileName, out repo))
            {
                return 2;
            }

            AnalyzeTestCases(repo);
            return 0;
        }

        private static bool LoadRepository(string fileName, out TestCaseRepository repository)
        {
            try
            {
                repository = TestCaseRepository.LoadFromFile(fileName);
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine("Failed to load the assembly file: {0}", e.Message);
                repository = null;
            }
            return repository != null;
        }

        private static void AnalyzeTestCases(TestCaseRepository repo)
        {
            var analyzer = new Analyzer(repo);
            analyzer.AddRule(new OneAssertPerTestCaseRule());
            analyzer.AddRule(new NonConditionalTestCaseRule());
            analyzer.AddRule(new NoTryCatchInTestCaseRule());
            analyzer.Analyze();
            
            Console.WriteLine("Violations:");
            foreach (var violation in analyzer.Violations)
            {
                Console.WriteLine("  Test case {0} violates rule \"{1}\"", violation.TestCase, violation.Rule);
            }

            Console.WriteLine();

            Console.WriteLine("Total score = {0}", analyzer.Score);
        }

        private static void PrintUsage()
        {
            var exeName = new FileInfo(Environment.GetCommandLineArgs()[0]).Name;
            Console.WriteLine("Usage: {0} <assembly file>", exeName);
            Console.WriteLine();
        }

        private static void PrintHeader()
        {
            var assembly = Assembly.GetCallingAssembly();
            Console.WriteLine("{0} v{1} - {2}", GetProductName(assembly), assembly.GetName().Version, GetCopyright(assembly));
            Console.WriteLine();
        }

        private static string GetCopyright(Assembly assembly)
        {
            return ((AssemblyCopyrightAttribute) Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute))).Copyright;
        }

        private static string GetProductName(Assembly assembly)
        {
            return ((AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute))).Product;
        }
    }
}
