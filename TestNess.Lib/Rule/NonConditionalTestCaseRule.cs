// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace TestNess.Lib.Rule
{
    public class NonConditionalTestCaseRule : IRule
    {
        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            var count = BranchCount(testCase.TestMethod);
            if (count == 0)
            {
                yield break;
            }
            yield return new Violation(this, testCase);
        }

        public static int BranchCount(MethodDefinition method)
        {
            if (method.HasBody)
            {
                var bc = 0;
                Instruction prev = null;
                foreach (var ins in method.Body.Instructions)
                {
                    if (IsBranchingInstruction(ins))
                    {
                        // LINQ delegates may be cached, and the cache check leads to conditional
                        // branching. To ignore that, check if the branching is preceded by the
                        // loading of a static compiler-generated field.
                        if (prev != null && prev.OpCode == OpCodes.Ldsfld)
                        {
                            var operand = prev.Operand as FieldDefinition;
                            if (IsCompilerGenerated(operand))
                            {
                                continue;
                            }
                        }
                        bc++;
                    }
                    prev = ins;
                }
                return bc;
            }
            return 0;
        }

        private static bool IsCompilerGenerated(FieldDefinition field)
        {
            if (field == null)
                return false;
            return field.CustomAttributes.Any(a => a.AttributeType.FullName == typeof (CompilerGeneratedAttribute).FullName);
        }

        private static bool IsBranchingInstruction(Instruction inst)
        {
            return inst.OpCode.FlowControl == FlowControl.Cond_Branch; 
        }

        public override string ToString()
        {
            return "a test case should not be conditional";
        }
    }
}
