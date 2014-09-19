// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace TestNess.Lib.TestFramework
{
    public class NSubstituteTestFramework : ITestFramework
    {
        private static readonly string[] ExpectationMethodNames =
        {
            "Received", "ReceivedWithAnyArgs", "DidNotReceive",
            "DidNotReceiveWithAnyArgs"
        };

        public bool IsSetupMethod(MethodDefinition method)
        {
            return false;
        }

        public bool IsTestMethod(MethodDefinition method)
        {
            return false;
        }

        public bool IsIgnoredTest(MethodDefinition method, out string reason)
        {
            reason = null;
            return false;
        }

        public bool HasExpectedException(MethodDefinition method)
        {
            return false;
        }

        public bool DoesContainAssertion(MethodDefinition method)
        {
            if (method.DeclaringType.FullName != "NSubstitute.SubstituteExtensions")
                return false;
            return Array.IndexOf(ExpectationMethodNames, method.Name) >= 0;
        }

        public IList<ParameterPurpose> GetParameterPurposes(MethodReference method)
        {
            return null;
        }

        public bool IsDataAccessorMethod(MethodReference method)
        {
            return false;
        }
    }
}
