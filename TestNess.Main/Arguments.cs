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

namespace TestNess.Main
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
        /// Returns <c>true</c> if an assembly file name has been specified, otherwise <c>false</c>.
        /// </summary>
        public bool HasAssemblyFileName
        {
            get { return AssemblyFileName != null; }
        }

        private Arguments(string assemblyFileName)
        {
            AssemblyFileName = assemblyFileName;
        }

        public static Arguments Parse(string[] args)
        {
            string assemblyFileName = null;
            if (args.Length > 0)
            {
                assemblyFileName = args[args.Length - 1];
            }
            return new Arguments(assemblyFileName);
        }
    }
}
