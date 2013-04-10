// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using System.Xml;
using TestNess.Lib;
using TestNess.Lib.Analysis;
using TestNess.Lib.Reporting;
using TestNess.Lib.Reporting.XUnit;
using TestNess.Lib.Rule;

namespace TestNess.Main
{
    class Program : IReportReceiver
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
        private string _reportFilePath;

        private Program(string[] args)
        {
            _arguments = Arguments.Parse(args);
        }

        internal void Run()
        {
            PrintHeader();
            if (!_arguments.HasAssemblyFileName || _arguments.ReportFilePath == null)
            {
                PrintUsage();
                throw new ExitException(1);
            }
            _reportFilePath = Path.Combine(Environment.CurrentDirectory, _arguments.ReportFilePath);
            var repo = TestCases.LoadFromFile(_arguments.AssemblyFileName);
            var rules = new Rules(typeof (IRule).Assembly);
            MaybeConfigureRules(rules);

            AnalyzeTestCases(repo, rules, new ViolationScorer());
        }

        private void MaybeConfigureRules(Rules rules)
        {
            if (!_arguments.HasConfigurationFileName) 
                return;

            var config = ReadFileContents(_arguments.ConfigurationFileName);
            var configurator = new RuleConfigurator();
            configurator.ReadConfiguration(config);
            configurator.ApplyConfiguration(rules);
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
            if (_arguments.HasOutputEncoding)
            {
                Console.OutputEncoding = _arguments.OutputEncoding;
            }
            CreateReporter(_arguments.ReporterType).GenerateReport(this, results);
        }

        private IReporter CreateReporter(ReporterType reporterType)
        {
            switch (reporterType)
            {
                case ReporterType.Plain:
                    return new PlainTextReporter();
                case ReporterType.XunitXml:
                    return new XUnitReporter();
                case ReporterType.XunitHtml:
                    return new XUnitHtmlReporter();
                case ReporterType.AngularJs:
                    return new AngularJsReporter();
            }
            throw new ArgumentException("Unknown reporter type: " + reporterType);
        }

        private static void PrintUsage()
        {
            var exeName = new FileInfo(Environment.GetCommandLineArgs()[0]).Name;
            Console.Error.WriteLine("Usage: {0} {1}", exeName, Arguments.GenerateUsageOverview());
            Console.Error.WriteLine();
        }

        private static void PrintHeader()
        {
            var assembly = Assembly.GetCallingAssembly();
            Console.Error.WriteLine("{0} v{1} - {2}", GetProductName(assembly), assembly.GetName().Version, GetCopyright(assembly));
            Console.Error.WriteLine();
        }

        private static string GetCopyright(Assembly assembly)
        {
            return ((AssemblyCopyrightAttribute) Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute))).Copyright;
        }

        private static string GetProductName(Assembly assembly)
        {
            return ((AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute))).Product;
        }

        public void GenerateReport(string contents)
        {
            WriteToFile(_reportFilePath, contents);
        }

        public void GenerateSupplementaryFile(string fileName, string contents)
        {
            var dir = new FileInfo(_reportFilePath).Directory.FullName;
            var filePath = Path.Combine(dir, fileName);
            if (!filePath.StartsWith(dir))
            {
                throw new Exception("Cannot write a file outside of the report file directory.");
            }
            WriteToFile(filePath, contents);
        }

        private void WriteToFile(string fileName, string data)
        {
            using (var fs = File.OpenWrite(fileName))
            using (var writer = new StreamWriter(fs, _arguments.HasOutputEncoding ? _arguments.OutputEncoding : Encoding.UTF8))
            {
                writer.WriteLine(data);
            }
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
