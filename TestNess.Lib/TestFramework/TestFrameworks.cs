// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestNess.Lib.TestFramework
{
    /// <summary>
    /// This class discovers test frameworks.
    /// </summary>
    public class TestFrameworks
    {
        public static TestFrameworks Instance = new TestFrameworks();
        private readonly List<ITestFramework> _frameworks = new List<ITestFramework>();

        public IEnumerable<ITestFramework> Frameworks { get { return _frameworks; } } 

        private TestFrameworks()
        {
            _frameworks.AddRange(DiscoverFrameworks(GetType().Assembly));
        }

        private IEnumerable<ITestFramework> DiscoverFrameworks(Assembly assembly)
        {
            return assembly.GetTypes().Where(IsTestFramework).Select(NewTestFramework);
        }

        private ITestFramework NewTestFramework(Type t)
        {
            return (ITestFramework) Activator.CreateInstance(t);
        }

        private bool IsTestFramework(Type t)
        {
            return t.IsPublic && typeof(ITestFramework).IsAssignableFrom(t) && !t.IsAbstract && t != GetType();
        }
    }
}
