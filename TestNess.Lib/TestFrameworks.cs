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

using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace TestNess.Lib
{
    /// <summary>
    /// This class merges a number of <see cref="ITestFramework" /> implementations while implementing said
    /// interface itself. Thus, it acts as a facade for a number of test frameworks.
    /// </summary>
    public class TestFrameworks : ITestFramework
    {
        private readonly IList<ITestFramework> _frameworks = new List<ITestFramework>();

        public TestFrameworks()
        {
            _frameworks.Add(new MSTestTestFramework());
        }

        public bool IsTestMethod(MethodDefinition method)
        {
            return _frameworks.Any(f => f.IsTestMethod(method));
        }

        public bool HasExpectedException(MethodDefinition method)
        {
            return _frameworks.Any(f => f.HasExpectedException(method));
        }

        public bool DoesContainAssertion(MethodDefinition method)
        {
            return _frameworks.Any(f => f.DoesContainAssertion(method));
        }
    }
}
