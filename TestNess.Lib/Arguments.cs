// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;

namespace TestNess.Lib
{
    /// <summary>
    /// Parser command-line arguments for the TestNess console application, and exposes the arguments in a
    /// structured form.
    /// </summary>
    public sealed class Arguments
    {
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

        private Arguments()
        {
        }

        public static Arguments Parse(string[] args)
        {
            var arguments = new Arguments();
            var index = 0;

            // Options first
            while (index < args.Length && args[index].StartsWith("-"))
            {
                var option = args[index].Substring(1);
                switch (option)
                {
                    case "c":
                        arguments.ConfigurationFileName = GetOptionValue("c", args, ++index);
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

        private static string GetOptionValue(string opt, string[] args, int index)
        {
            var value = index < args.Length ? args[index] : null;
            if (value == null)
                throw new ArgumentException(string.Format("Missing value for option '{0}'.", opt));
            return value;
        }
    }
}
