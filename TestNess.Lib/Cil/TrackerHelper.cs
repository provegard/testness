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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace TestNess.Lib.Cil
{
    static class TrackerHelper
    {
        internal static int GetPushCount(this Instruction instruction)
        {
            int count;
            switch (instruction.OpCode.StackBehaviourPush)
            {
                case StackBehaviour.Push1_push1:
                    count = 2;
                    break;
                case StackBehaviour.Varpush:
                    count = CalculatePushCount(instruction);
                    break;
                case StackBehaviour.Push0:
                    count = 0;
                    break;
                default:
                    count = 1;
                    break;
            }
            return count;
        }

        private static int CalculatePushCount(Instruction instruction)
        {
            // First simple approach: 1 if non-void, 0 otherwise
            var method = instruction.Operand as MethodReference;
            return IsVoidMethod(method) ? 0 : 1;
        }

        internal static bool IsVoidMethod(MethodReference method)
        {
            return "System.Void".Equals(method.ReturnType.FullName);
        }

        internal static bool IsStoreLocal(this Instruction instruction, out int index)
        {
            var ret = true;
            switch (instruction.OpCode.Code)
            {
                case Code.Stloc:
                case Code.Stloc_S:
                    index = Convert.ToInt32(instruction.Operand);
                    break;
                case Code.Stloc_0:
                    index = 0;
                    break;
                case Code.Stloc_1:
                    index = 1;
                    break;
                case Code.Stloc_2:
                    index = 2;
                    break;
                case Code.Stloc_3:
                    index = 3;
                    break;
                default:
                    index = -1;
                    ret = false;
                    break;
            }
            return ret;
        }

        internal static bool IsLoadLocal(this Instruction instruction, out int index)
        {
            var ret = true;
            switch (instruction.OpCode.Code)
            {
                case Code.Ldloca:
                case Code.Ldloca_S:
                    var vdef = instruction.Operand as VariableDefinition;
                    index = vdef.Index;
                    break;
                case Code.Ldloc:
                case Code.Ldloc_S:
                    index = Convert.ToInt32(instruction.Operand);
                    break;
                case Code.Ldloc_0:
                    index = 0;
                    break;
                case Code.Ldloc_1:
                    index = 1;
                    break;
                case Code.Ldloc_2:
                    index = 2;
                    break;
                case Code.Ldloc_3:
                    index = 3;
                    break;
                default:
                    index = -1;
                    ret = false;
                    break;
            }
            return ret;
        }

        internal static int GetPopCount(this Instruction instruction, MethodDefinition method)
        {
            int count;
            switch (instruction.OpCode.StackBehaviourPop)
            {
                case StackBehaviour.Popref_popi_popref:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popi_popi_popi:
                    count = 3;
                    break;
                case StackBehaviour.Popref_popi:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Pop1_pop1:
                    count = 2;
                    break;
                case StackBehaviour.Varpop:
                    count = CalculatePopCount(instruction, method);
                    break;
                case StackBehaviour.Pop0:
                    count = 0;
                    break;
                default:
                    count = 1;
                    break;
            }
            return count;
        }

        private static int CalculatePopCount(Instruction instruction, MethodDefinition method)
        {
            int count;
            if (instruction.OpCode.FlowControl == FlowControl.Call)
            {
                // First simple approach: number of parameters, + 1 if virtual call
                var calledMethod = instruction.Operand as MethodReference;
                count = calledMethod.Parameters.Count + (calledMethod.HasThis ? 1 : 0);
            }
            else if (instruction.OpCode == OpCodes.Ret)
            {
                count = IsVoidMethod(method) ? 0 : 1;
            }
            else
            {
                throw new ArgumentException("Unhandled instruction: " + instruction.OpCode);
            }
            return count;
        }

        internal static bool IsRef(this ParameterDefinition pdef)
        {
            return pdef.ParameterType.IsByReference;
        }
    }
}
