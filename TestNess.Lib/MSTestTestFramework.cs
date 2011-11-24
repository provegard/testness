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
using Mono.Cecil.Cil;

namespace TestNess.Lib
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

        public bool IsTestMethod(MethodDefinition method)
        {
            return method.CustomAttributes.Where(IsTestAttribute).Count() > 0;
        }

        private static bool IsTestAttribute(CustomAttribute attr)
        {
            return TestMethodAttributeName.Equals(attr.AttributeType.FullName);
        }

        public bool HasExpectedException(MethodDefinition method)
        {
            return method.CustomAttributes.Where(IsExpectedExceptionAttribute).Count() > 0;
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
            var reducedMethod = ReduceToShortestOverload(method);
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

        private static MethodReference ReduceToShortestOverload(MethodReference method)
        {
            var type = method.DeclaringType.Resolve();
            var shortest = type.Methods.Where(m => m.Name.Equals(method.Name)).OrderBy(m => m.Parameters.Count).First();
            return shortest;
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
