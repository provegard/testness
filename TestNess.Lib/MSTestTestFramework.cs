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
                    isAssert = "Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException".Equals(
                        reference.DeclaringType.FullName);
                }
            }
            return isAssert;
        }
    }
}
