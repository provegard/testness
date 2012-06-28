// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;

namespace TestNess.Lib
{
    /// <summary>
    /// Class that represents an exception thrown when a method is not a recognized test method.
    /// </summary>
    public class NotATestMethodException : Exception
    {
        public NotATestMethodException(string testMethodName)
            : base("Not a test method: " + testMethodName)
        {
        }
    }
}
