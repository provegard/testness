// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace TestNess.Lib.TestFramework
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

        public bool IsSetupMethod(MethodDefinition method)
        {
            return _frameworks.Any(f => f.IsSetupMethod(method));
        }

        public bool IsTestMethod(MethodDefinition method)
        {
            return _frameworks.Any(f => f.IsTestMethod(method));
        }

        public bool IsIgnoredTest(MethodDefinition method, out string reason)
        {
            reason = null;
            foreach (var fw in _frameworks)
            {
                if (fw.IsIgnoredTest(method, out reason))
                    return true;
            }
            return false;
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

        public bool IsDataAccessorMethod(MethodReference method)
        {
            return _frameworks.Any(f => f.IsDataAccessorMethod(method));
        }
    }
}
