// Copyright (C) 2011-2013 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace TestNess.Lib.TestFramework
{
    public class MSTestTestFramework : ITestFramework
    {
        private const string TestMethodAttributeName =
            "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute";
        private const string ExpectedExceptionAttributeName =
            "Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedExceptionAttribute";
        private const string AssertExceptionTypeName =
            "Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException";
        private const string TestContextTypeName = 
            "Microsoft.VisualStudio.TestTools.UnitTesting.TestContext";
        private const string TestSetupAttributeName =
            "Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute";
        private const string IgnoreAttributeName =
            "Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute";

        public bool IsSetupMethod(MethodDefinition method)
        {
            return method.CustomAttributes.Where(IsInitializeAttribute).Any();
        }

        public bool IsTestMethod(MethodDefinition method)
        {
            return method.CustomAttributes.Where(IsTestAttribute).Any();
        }

        public bool IsIgnoredTest(MethodDefinition method)
        {
            return method.CustomAttributes.Where(IsIgnoreAttribute).Any();
        }

        private static bool IsIgnoreAttribute(CustomAttribute attr)
        {
            return IgnoreAttributeName.Equals(attr.AttributeType.FullName);
        }

        private static bool IsTestAttribute(CustomAttribute attr)
        {
            return TestMethodAttributeName.Equals(attr.AttributeType.FullName);
        }

        private static bool IsInitializeAttribute(CustomAttribute attr)
        {
            return TestSetupAttributeName.Equals(attr.AttributeType.FullName);
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
            // The call to the test class' own TestContext property must be allowed
            if ("get_TestContext".Equals(method.Name) && TestContextTypeName.Equals(method.ReturnType.FullName))
                return true;
            // The call to the DataRow property on TestContext must be allowed
            if (TestContextTypeName.Equals(method.DeclaringType.FullName) && "get_DataRow".Equals(method.Name))
                return true;
            // Finally, all DataRow instance interaction is allowed
            if ("System.Data.DataRow".Equals(method.DeclaringType.FullName))
                return true;

            // All other methods are considered as not related to data access
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
                    if ("System.Type".Equals(method.Parameters[1].ParameterType.FullName))
                        return paramIdx == 0 ? ParameterPurpose.Actual : ParameterPurpose.Expected;
                    if ("StringAssert".Equals(method.DeclaringType.Name))
                        return paramIdx == 0 ? ParameterPurpose.Actual : ParameterPurpose.Expected;
                    if (method.Name.Contains("Subset") || method.Name.Contains("Contain"))
                        return ParameterPurpose.ExpectedOrActual;
                    break;
            }
            return ParameterPurpose.Unknown;
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
    }
}
