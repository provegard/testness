// Copyright (C) 2011-2013 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace TestNess.Lib.TestFramework
{
    public class NUnitTestFramework : ITestFramework
    {
        private const string TestMethodAttributeName =
            "NUnit.Framework.TestAttribute";
        private const string TestCaseMethodAttributeName =
            "NUnit.Framework.TestCaseAttribute";
        private const string ExpectedExceptionAttributeName =
            "NUnit.Framework.ExpectedExceptionAttribute";
        private const string AssertExceptionTypeName =
            "NUnit.Framework.AssertionException";
        private const string SetUpAttributeName =
            "NUnit.Framework.SetUpAttribute";
        private const string TestFixtureSetUpAttributeName =
            "NUnit.Framework.TestFixtureSetUpAttribute";
        private const string IgnoreAttributeName =
            "NUnit.Framework.IgnoreAttribute";

        public bool IsSetupMethod(MethodDefinition method)
        {
            return method.CustomAttributes.Where(IsSetupAttribute).Any();
        }

        public bool IsTestMethod(MethodDefinition method)
        {
            return method.CustomAttributes.Any(a => IsTestAttribute(a) || IsTestCaseAttribute(a));
        }

        private static bool IsSetupAttribute(CustomAttribute attr)
        {
            return SetUpAttributeName.Equals(attr.AttributeType.FullName)
                || TestFixtureSetUpAttributeName.Equals(attr.AttributeType.FullName);
        }

        private static bool IsTestCaseAttribute(CustomAttribute attr)
        {
            return TestCaseMethodAttributeName.Equals(attr.AttributeType.FullName);
        }

        private static bool IsTestAttribute(CustomAttribute attr)
        {
            return TestMethodAttributeName.Equals(attr.AttributeType.FullName);
        }

        public bool HasExpectedException(MethodDefinition method)
        {
            return method.CustomAttributes.Where(IsExpectedExceptionAttribute).Any();
        }

        private static bool IsExpectedExceptionAttribute(CustomAttribute attr)
        {
            return ExpectedExceptionAttributeName.Equals(attr.AttributeType.FullName);
        }

        public bool DoesContainAssertion(MethodDefinition method)
        {
            return CountAssertions(method) > 0;
        }

        public IList<ParameterPurpose> GetParameterPurposes(MethodReference method)
        {
            var reducedMethod = method.ReduceToShortestOverload();
            var list = new List<ParameterPurpose>();
            for (var i = 0; i < method.Parameters.Count; i++)
            {
                if (i < reducedMethod.Parameters.Count)
                    list.Add(DeduceParameterPurpose(reducedMethod, i));
                else
                    list.Add(ParameterPurpose.MetaData);
            }
            return list;
        }

        public bool IsDataAccessorMethod(MethodReference method)
        {
            // Data access (to data in data-driven tests) in NUnit goes via the method parameters.
            return false;
        }

        private static ParameterPurpose DeduceParameterPurpose(MethodReference method, int paramIdx)
        {
            var pc = method.Parameters.Count;
            switch (pc)
            {
                case 1:
                    return ParameterPurpose.Actual;

                case 2:
                    if (method.Name.StartsWith("Are") || "Equals".Equals(method.Name))
                        return paramIdx == 0 ? ParameterPurpose.Expected : ParameterPurpose.Actual;
                    var typeParamIndex = GetTypeParameterIndex(method.Parameters);
                    if (typeParamIndex >= 0)
                        return paramIdx == typeParamIndex ? ParameterPurpose.Expected : ParameterPurpose.Actual;
                    if ("StringAssert".Equals(method.DeclaringType.Name))
                        return paramIdx == 0 ? ParameterPurpose.Expected : ParameterPurpose.Actual;
                    if (method.Name.Contains("Subset") || method.Name.Contains("Contain"))
                        return ParameterPurpose.ExpectedOrActual;
                    break;
            }
            return ParameterPurpose.Unknown;
        }

        private static int GetTypeParameterIndex(IEnumerable<ParameterDefinition> parameters)
        {
            return
                parameters.Select((p, i) => new {p, i}).Where(x => "System.Type".Equals(x.p.ParameterType.FullName)).
                    Select(x => x.i + 1).FirstOrDefault() - 1;
        }
        
        private static int CountAssertions(MethodDefinition method)
        {
            return method.Body.Instructions.Where(IsAssertion).Count();
        }

        private static bool IsAssertion(Instruction inst)
        {
            var isAssert = false;
            if (inst.OpCode == OpCodes.Throw)
            {
                var prev = inst.Previous;
                if (prev.OpCode == OpCodes.Newobj)
                {
                    var reference = (MethodReference)prev.Operand;
                    isAssert = AssertExceptionTypeName.Equals(reference.DeclaringType.FullName);
                }
            }
            return isAssert;
        }

        public bool IsIgnoredTest(MethodDefinition method)
        {
            return method.CustomAttributes.Where(IsIgnoreAttribute).Any();
        }

        private static bool IsIgnoreAttribute(CustomAttribute attr)
        {
            return IgnoreAttributeName.Equals(attr.AttributeType.FullName);
        }
    }
}
