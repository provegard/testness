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

namespace TestNess.Lib.Rule
{
    public class UnhandledReturnValueRule : IRule
    {
        private readonly ITestFramework _framework = TestFrameworks.Instance;

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            // It seems as if unhandled return values are popped off the stack
            // immediately via an explicit "pop" instruction.
            var callingNonVoidInstructions = testCase.TestMethod.CalledMethods().Select(cm => cm.Instruction);
            var unhandled = callingNonVoidInstructions.Where(ins => ins.Next.OpCode == OpCodes.Pop).ToList();
            if (unhandled.Count == 1 && _framework.HasExpectedException(testCase.TestMethod))
                yield break; // last unhandled value is ok!
            foreach (var instr in unhandled)
            {
                yield return new Violation(this, testCase, instr, CreateViolationMessage(instr));
            }
        }

        private static string CreateViolationMessage(Instruction instr)
        {
            var method = (MethodReference) instr.Operand;
            return string.Format("return value of method {0} is not used", method.NameWithParameters());
        }

        public override string ToString()
        {
            return "a test case should deal with all method return values";
        }
    }
}
