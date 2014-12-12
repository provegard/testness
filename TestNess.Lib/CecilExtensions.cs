// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace TestNess.Lib
{
    public static class CecilExtensions
    {
        /// <summary>
        /// Extension method that finds all methods that are called from the extended method, and 
        /// for each method returns the call instruction and the method reference.
        /// </summary>
        /// <param name="definition">The method to extend.</param>
        /// <returns>The called methods.</returns>
        public static IEnumerable<MethodCall> CalledMethods(this MethodDefinition definition)
        {
            if (!definition.HasBody)
                return new MethodCall[0];
            return from instruction in definition.Body.Instructions
                   where instruction.OpCode.FlowControl == FlowControl.Call
                   select
                       new MethodCall(instruction, instruction.Operand as MethodReference);
        }

        /// <summary>
        /// Extension method that finds all overloads for the given method and returns the shortest
        /// one.
        /// </summary>
        /// <param name="method">The method for which to find the shortest overload.</param>
        /// <returns>A method reference.</returns>
        public static MethodReference ReduceToShortestOverload(this MethodReference method)
        {
            var type = method.DeclaringType.Resolve();
            var shortest = type.Methods.Where(m => m.Name.Equals(method.Name)).OrderBy(m => m.Parameters.Count).First();
            return shortest;
        }

        /// <summary>
        /// Extension method that return the name of a method, including the parameter list but
        /// excluding the return type.
        /// </summary>
        /// <param name="method">The method for which to return the name.</param>
        /// <returns>A string with the method name and the parameter list.</returns>
        public static string NameWithParameters(this MethodReference method)
        {
            var fn = method.FullName;
            var cc = fn.IndexOf("::");
            return cc >= 0 ? fn.Substring(cc + 2) : fn;
        }

        /// <summary>
        /// Extension method that finds the sequence point for an instruction. If the instruction
        /// itself does not have a sequence point, its predecessors are search backwards until a
        /// sequence point is found, or there are no more instructions to search.
        /// </summary>
        /// <param name="instruction">The instruction whose sequence point to find.</param>
        /// <returns>A sequence point, or <c>null</c> if none was found.</returns>
        public static SequencePoint FindSequencePoint(this Instruction instruction)
        {
            var i = instruction;
            var sp = i.SequencePoint;
            while (sp == null && i.Previous != null)
            {
                i = i.Previous;
                sp = i.SequencePoint;
            }
            return sp;
        }
    }
}
