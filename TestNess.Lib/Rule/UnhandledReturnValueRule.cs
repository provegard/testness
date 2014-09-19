﻿// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TestNess.Lib.TestFramework;

namespace TestNess.Lib.Rule
{
    public class UnhandledReturnValueRule : IRule
    {
        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            // It seems as if unhandled return values are popped off the stack
            // immediately via an explicit "pop" instruction.
            
            // We exclude asserting methods to avoid getting a violation for Assert.Throws (or Assert.Catch),
            // which returns the exception.
            var calledMethods = testCase.TestMethod.CalledMethods();
            var asserting = testCase.GetCalledAssertingMethods();

            var callingNonVoidInstructions = calledMethods.Where(IsNotAnAssertingMethod(asserting)).Select(cm => cm.Instruction);
            
            var unhandled = callingNonVoidInstructions.Where(ins => ins.Next.OpCode == OpCodes.Pop).ToList();
            if (unhandled.Count == 1 && testCase.Framework.HasExpectedException(testCase.TestMethod))
                yield break; // last unhandled value is ok!
            foreach (var instr in unhandled)
            {
                yield return new Violation(this, testCase, instr, CreateViolationMessage(instr));
            }
        }

        private Func<CecilExtensions.CalledMethod, bool> IsNotAnAssertingMethod(ICollection<MethodDefinition> asserting)
        {
            return calledMethod =>
            {
                var methodDef = calledMethod.Method.Resolve();
                return methodDef == null || !asserting.Contains(methodDef);
            };
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
