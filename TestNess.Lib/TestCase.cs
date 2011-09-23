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

using Mono.Cecil;

namespace TestNess.Lib
{
    /// <summary>
    /// Class that represents a single unit test case to be evaluated by TestNess. A instance of this class 
    /// encapsulates a Cecil <see cref="MethodDefinition"/> instance, which is the method that contains (or
    /// defines, if you will) the test case.
    /// <para>
    /// This is a DDD aggregate root, which means that clients should not store data fetched from an 
    /// instance of this class, other than temporarily.
    /// </para>
    /// </summary>
    public class TestCase
    {
        private MethodDefinition _method;

        /// <summary>
        /// Creates an instance of this class based on a method. The method is assumed to be a test method.
        /// </summary>
        /// <param name="method">The method that contains/defines the test case.</param>
        public TestCase(MethodDefinition method)
        {
            _method = method;
        }
    }
}
