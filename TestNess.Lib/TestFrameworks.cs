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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace TestNess.Lib
{
    /// <summary>
    /// This class merges a number of <see cref="ITestFramework" /> implementations while implementing said
    /// interface itself. Thus, it acts as a facade for a number of test frameworks.
    /// </summary>
    public class TestFrameworks : ITestFramework
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static readonly TestFrameworks Instance = new TestFrameworks();

        private readonly List<ITestFramework> _frameworks = new List<ITestFramework>();

        private TestFrameworks()
        {
            _frameworks.AddRange(DiscoverRules(GetType().Assembly));
        }

        private IEnumerable<ITestFramework> DiscoverRules(Assembly assembly)
        {
            return assembly.GetTypes().Where(IsTestFramework).Select(NewTestFramework);
        }

        private ITestFramework NewTestFramework(Type t)
        {
            return (ITestFramework) Activator.CreateInstance(t);
        }

        private bool IsTestFramework(Type t)
        {
            return t.IsPublic && typeof(ITestFramework).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t != GetType();
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

        public IList<ParameterPurpose> GetParameterPurposes(MethodReference method)
        {
            IList<ParameterPurpose> list = null;
            foreach (var fw in _frameworks)
            {
                list = fw.GetParameterPurposes(method);
                if (list != null) break;
            }
            return list;
        }
    }
}
