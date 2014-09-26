// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Test.Rule
{
    public abstract class AbstractRuleTest<TRule, TTestClass> where TRule : IRule, new()
    {
        protected IEnumerable<Violation> FindViolations(string method)
        {
            var tc = typeof(TTestClass).FindTestCase(method);
            var rule = new TRule();
            return rule.Apply(tc);
        }
    }
}
