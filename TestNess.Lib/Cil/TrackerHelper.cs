﻿// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
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

        internal static bool IsConstructor(this MethodReference method)
        {
            return ".ctor" == method.Name;
        }

        internal static bool IsStoreLocal(this Instruction instruction, out int index)
        {
            var ret = true;
            switch (instruction.OpCode.Code)
            {
                case Code.Stloc:
                case Code.Stloc_S:
                    index = ((VariableDefinition) instruction.Operand).Index;
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
                case Code.Ldloc:
                case Code.Ldloc_S:
                    index = ((VariableDefinition) instruction.Operand).Index;
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
                case StackBehaviour.PopAll:
                    // Signal to the caller that the evaluation stack is emptied.
                    count = Int32.MaxValue;
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
                count = calledMethod.Parameters.Count + (AssumesObjectOnStack(instruction, calledMethod) ? 1 : 0);
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

        private static bool AssumesObjectOnStack(Instruction instruction, MethodReference method)
        {
            // Newobj creates the 'this' instance rather than consuming it.
            if (instruction.OpCode == OpCodes.Newobj)
                return false;
            return method.HasThis;
        }

        internal static bool IsRef(this ParameterDefinition pdef)
        {
            return pdef.ParameterType.IsByReference;
        }
    }
}
