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
