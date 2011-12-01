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
    public static class CecilExtensions
    {
        /// <summary>
        /// Extension method that finds all methods that are called from the extended method, and 
        /// for each method returns the call instruction and the method reference.
        /// </summary>
        /// <param name="definition">The method to extend.</param>
        /// <returns>The called methods.</returns>
        public static IEnumerable<CalledMethod> CalledMethods(this MethodDefinition definition)
        {
            if (!definition.HasBody)
                return new CalledMethod[0];
            return from instruction in definition.Body.Instructions
                   where instruction.OpCode.FlowControl == FlowControl.Call
                   select
                       new CalledMethod { Instruction = instruction, Method = instruction.Operand as MethodReference };
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

        public struct CalledMethod
        {
            public Instruction Instruction;
            public MethodReference Method;
        }
    }
}
