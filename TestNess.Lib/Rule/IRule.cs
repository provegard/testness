// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;

namespace TestNess.Lib.Rule
{
    public interface IRule
    {
        IEnumerable<Violation> Apply(TestCase testCase);
    }
}
