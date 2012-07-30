// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Text;

namespace TestNess.Lib
{
    /// <summary>
    /// Parser command-line arguments for the TestNess console application, and exposes the arguments in a
    /// structured form.
    /// </summary>
    public sealed class Arguments
    {
        /// <summary>
        /// Returns the output encoding to use for generated files/text data.
        /// </summary>
        public Encoding OutputEncoding;

        /// <summary>
        /// Determines if there is a specified output encoding or not.
        /// </summary>
        public bool HasOutputEncoding
        {
            get { return OutputEncoding != null;  }
        }

        /// <summary>
        /// Returns the name of the assembly file.
        /// </summary>
        public string AssemblyFileName { get; private set; }

        /// <summary>
        /// Returns the name of the configuration file, if any.
        /// </summary>
        public string ConfigurationFileName { get; private set; }

        /// <summary>
        /// Returns <c>true</c> if an assembly file name has been specified, otherwise <c>false</c>.
        /// </summary>
        public bool HasAssemblyFileName
        {
            get { return AssemblyFileName != null; }
        }

        /// <summary>
        /// Returns <c>true</c> if an configuration file name has been specified, otherwise <c>false</c>.
        /// </summary>
        public bool HasConfigurationFileName
        {
            get { return ConfigurationFileName != null; }
        }

        public ReporterType ReporterType { get; private set; }

        private Arguments()
        {
        }

        public static Arguments Parse(string[] args)
        {
            var arguments = new Arguments();
            var index = 0;

            // Options first
            while (index < args.Length && args[index].StartsWith("/"))
            {
                var colonIdx = args[index].IndexOf(':');
                string option, arg;
                if (colonIdx < 0)
                {
                    option = args[index].Substring(1);
                    arg = null;
                }
                else
                {
                    option = args[index].Substring(1, colonIdx - 1);
                    arg = args[index].Substring(colonIdx + 1);
                }
                switch (option)
                {
                    case "c":
                    case "config":
                        arguments.ConfigurationFileName = GetOptionValue(option, arg);
                        break;

                    case "e":
                    case "enc":
                    case "encoding":
                        arguments.OutputEncoding = Encoding.GetEncoding(GetOptionValue(option, arg));
                        break;

                    case "plain":
                        arguments.ReporterType = ReporterType.Plain;
                        break;

                    case "xxml":
                        arguments.ReporterType = ReporterType.XunitXml;
                        break;

                    case "xhtml":
                        arguments.ReporterType = ReporterType.XunitHtml;
                        break;

                    default:
                        throw new ArgumentException(string.Format("Encountered unrecognized option '{0}'.", option));
                }
                index++;
            }

            // Then non-options
            if (index < args.Length)
            {
                arguments.AssemblyFileName = args[index];
            }

            return arguments;
        }

        private static string GetOptionValue(string opt, string arg)
        {
            if (arg == null)
                throw new ArgumentException(string.Format("Missing value for option '{0}'.", opt));
            return arg;
        }

        public static string GenerateUsageOverview()
        {
            var sb = new StringBuilder("[/c[onfig]:<configuration file>] [reporting option] <assembly file>");
            sb.AppendLine().AppendLine();
            sb.AppendLine("  The following reporting options are supported:").AppendLine();
            sb.AppendLine("    /plain   Prints a plain-text report.");
            sb.AppendLine("    /xxml    Prints an XUnit XML report.");
            sb.AppendLine("    /xhtml   Prints an XUnit HTML report.");
            return sb.ToString();
        }
    }

    public enum ReporterType
    {
        Plain,
        XunitXml,
        XunitHtml
    }
}
