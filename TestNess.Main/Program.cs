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
using System.Text;
using System.Xml;
using TestNess.Lib;
using TestNess.Lib.Analysis;
using TestNess.Lib.Reporting;
using TestNess.Lib.Rule;

namespace TestNess.Main
{
    class Program
    {
        static int Main(string[] args)
        {
            var ret = 0;
            try
            {
                var program = new Program(args);
                program.Run();
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                ret = 1;
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine(ex.Message);
                ret = 2;                                    
            }
            catch (ExitException ex)
            {
                ret = ex.ExitCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("An error occurred: {0}", ex.Message);
                ret = 3;
            }
            return ret;
        }

        private readonly Arguments _arguments;

        private Program(string[] args)
        {
            _arguments = Arguments.Parse(args);
        }

        internal void Run()
        {
            PrintHeader();
            if (!_arguments.HasAssemblyFileName)
            {
                PrintUsage();
                throw new ExitException(1);
            }
            var repo = TestCases.LoadFromFile(_arguments.AssemblyFileName);
            var rules = new Rules(typeof (IRule).Assembly);
            if (_arguments.HasConfigurationFileName)
            {
                var config = ReadFileContents(_arguments.ConfigurationFileName);
                var configurator = new RuleConfigurator();
                configurator.ReadConfiguration(config);
                configurator.ApplyConfiguration(rules);
            }

            AnalyzeTestCases(repo, rules);
        }

        private string ReadFileContents(string file)
        {
            using (var stream = new StreamReader(file))
            {
                return stream.ReadToEnd();
            }
        }

        private void AnalyzeTestCases(TestCases repo, Rules rules)
        {
            var analyzer = new Analyzer(repo, rules);
            analyzer.Analyze();
            
            Console.WriteLine("Violations:");
            foreach (var violation in analyzer.Violations)
            {
                Console.WriteLine("  {0}", violation);
            }

            Console.WriteLine();

            var reporter = new Reporter(analyzer.AnalysisTree, new ViolationScorer());
            var elem = reporter.Generate();

            var writer = new XmlTextWriter("c:\\temp\\xunit-test-output.xml", Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument(true);
            elem.WriteTo(writer);

            //Console.WriteLine("Total score = {0}", analyzer.Score);
        }

        private static void PrintUsage()
        {
            var exeName = new FileInfo(Environment.GetCommandLineArgs()[0]).Name;
            Console.WriteLine("Usage: {0} [-c <config file>] <assembly file>", exeName);
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

    class ExitException : Exception
    {
        internal int ExitCode { get; private set; }

        internal ExitException(int code)
        {
            ExitCode = code;
        }
    }
}
