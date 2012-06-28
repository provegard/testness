// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Xml;
using TestNess.Lib;
using TestNess.Lib.Analysis;
using TestNess.Lib.Reporting;
using TestNess.Lib.Reporting.XUnit;
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

            AnalyzeTestCases(repo, rules, new ViolationScorer());
        }

        private string ReadFileContents(string file)
        {
            using (var stream = new StreamReader(file))
            {
                return stream.ReadToEnd();
            }
        }

        private void AnalyzeTestCases(IEnumerable<TestCase> repo, IEnumerable<IRule> rules, IViolationScorer scorer)
        {
            var results = AnalysisResults.Create(repo, rules, scorer);
            new ConsoleReporter().GenerateReport(results);

            //TODO: Let command-line arguments control reporting!
            using (var writer = XmlWriter.Create(@"c:\temp\test1.html"))
            {
                new XUnitHtmlReporter(writer).GenerateReport(results);
            }

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
