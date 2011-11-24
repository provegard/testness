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
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mono.Cecil;
using Mono.Cecil.Cil;
using GraphBuilder;

namespace TestNess.Lib.Cil
{
    /// <summary>
    /// Tracks values produced and consumed within the boundaries of a single method. Given an instruction
    /// in the method, a tracker can tell which value or values are consumed by the method. For a consumed
    /// value, it can also tell which source values the value is derived from.
    /// </summary>
    public class MethodValueTracker
    {
        private static readonly IList<ParameterDefinition> NoParams = new List<ParameterDefinition>(); 

        private readonly MethodDefinition _method;

        /// <summary>
        /// Graph of values produced and consumed in the method this tracker was created for.
        /// The root is a fake value which only serves the purpose of being root.
        /// </summary>
        public Graph<Value> ValueGraph { get; private set; } 

        public MethodValueTracker(MethodDefinition method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            _method = method;
            ValueGraph = CreateValueGraph();
        }

        private Graph<Value> CreateValueGraph()
        {
            var stack = new Stack<Value>();
            var values = new List<Value>();
            var locals = new Value[_method.Body.Variables.Count];
            foreach (var instruction in _method.Body.Instructions)
            {
                Apply(instruction, stack, values, locals);
            }
            Debug.Assert(stack.Count == 0, "Stack contains unpopped values.");

            return CreateGraph(values);
        }

        private void Apply(Instruction instruction, Stack<Value> stack, ICollection<Value> values, IList<Value> locals)
        {
            // TODO: this method is a mess and needs to be refactored!!

            FailOnBranchInstruction(instruction);

            var pushCount = instruction.GetPushCount();
            var popCount = instruction.GetPopCount(_method);
            if (popCount == Int32.MaxValue)
                popCount = stack.Count;
            var isCall = instruction.OpCode.FlowControl == FlowControl.Call;
            var callParams = isCall ? (instruction.Operand as MethodReference).Parameters : NoParams;

            // List of all popped values
            var poppedValues = new List<Value>();

            var inputArguments = new List<Value>();
            for (var i = 0; i < popCount; i++)
            {
                // Instruction is a consumer
                var value = stack.Pop();
                poppedValues.Add(value);
                // If we popped a value for an out parameter, we're not a consumer!
                // Note that the first pop returns the last argument.
                if (isCall && i < callParams.Count && callParams[callParams.Count - i - 1].IsOut)
                {
                    inputArguments.Insert(0, null); // empty slot
                    continue;
                }
                value.Consumer = instruction;
                inputArguments.Insert(0, value); //TODO: not 'this'!?!?
            }
            int storeIndex;
            if (instruction.IsStoreLocal(out storeIndex))
            {
                Debug.Assert(popCount == 1);
                locals[storeIndex] = poppedValues[0];
            }
            for (var i = 0; i < pushCount; i++)
            {
                Value newValue;
                // Instruction is a producer
                int loadIndex;
                if (instruction.IsLoadLocal(out loadIndex))
                {
                    Debug.Assert(pushCount == 1);
                    newValue = new Value(instruction);
                    values.Add(newValue);
                    // The local value can be null if we're passing loading a reference
                    // destined for an out parameter. Note that we can get a non-null
                    // value as well, so we have to sort things out when we handle the
                    // call to the method with the out parameter.
                    var localValue = locals[loadIndex];
                    if (localValue != null)
                    {
                        newValue.AddParents(new[] { localValue });
                    }
                }
                else
                {
                    newValue = new Value(instruction);
                    values.Add(newValue);
                    newValue.AddParents(inputArguments.Where(a => a != null));
                }
                stack.Push(newValue);
            }
            if (isCall)
            {
                var argValues = new Value[callParams.Count];
                for (var i = 0; i < argValues.Length; i++)
                {
                    argValues[i] = poppedValues[argValues.Length - i - 1];
                }
                for (var i = 0; i < callParams.Count; i++)
                {
                    // First of poppedValues is last argument
                    var inputArgument = poppedValues[callParams.Count - i - 1];
                    if (!callParams[i].IsRef() && !callParams[i].IsOut) 
                        continue;

                    var newValue = new Value(instruction);
                    var storeAtIndex = (inputArgument.Producer.Operand as VariableDefinition).Index;

                    // Add all input values (including any refs) as parents!
                    for (var j = 0; j < callParams.Count; j++)
                    {
                        if (callParams[j].IsOut)
                            continue;
                        newValue.AddParents(new[] { argValues[j] });
                    }

                    // Don't push onto the stack, but save the value and store
                    // it in the locals array.
                    values.Add(newValue);
                    locals[storeAtIndex] = newValue;
                }
            }
        }

        private static void FailOnBranchInstruction(Instruction instruction)
        {
            var fc = instruction.OpCode.FlowControl;
            if (fc == FlowControl.Branch || fc == FlowControl.Cond_Branch)
                throw new BranchingNotYetSupportedException();
        }

        private Graph<Value> CreateGraph(ICollection<Value> values)
        {
            var allParents = values.SelectMany(v => v.Parents).Distinct();
            var nonParents = values.Except(allParents);
            var fakeRoot = new Value(null);
            fakeRoot.AddParents(nonParents);
            var builder = new GraphBuilder<Value>(v => v.Parents);
            return builder.Build(fakeRoot);
        }

        /// <summary>
        /// Returns the values that are consumed by the given instruction (which must be part of the
        /// method this tracker was created for, obviously). The "this object" value needed for a
        /// virtual call <strong>is not</strong> included.
        /// </summary>
        /// <param name="query">An instruction that consumes values.</param>
        /// <returns>An enumerable of values consumed by the instruction.</returns>
        public IEnumerable<Value> GetConsumedValues(Instruction query)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            if (!_method.Body.Instructions.Contains(query))
                throw new ArgumentException("Unknown instruction!");
            return ValueGraph.Walk().Where(v => v.Consumer == query).Where(Not_IsThisObjectValue);
        }

        private bool Not_IsThisObjectValue(Value value)
        {
            if (!_method.HasThis)
                return true;
            return value.Producer.OpCode != OpCodes.Ldarg_0;
        }

        /// <summary>
        /// Given a value, returns all source values that the value stems from. For example, if a value
        /// is the result of the addition of two values, the two added values are returned. To find out
        /// the opcodes of the instructions that produced the values, use something like this:
        /// <para><code>
        /// var opCodes = sourceValues.Select(v => v.Producer.OpCode).Distinct();
        /// </code></para>
        /// </summary>
        /// <param name="value">A value whose source values to find.</param>
        /// <returns>An enumerable of source values.</returns>
        public IEnumerable<Value> FindSourceValues(Value value)
        {
            return ValueGraph.Walk(value).Where(v => v.Parents.Count == 0);
        }

        /// <summary>
        /// Represents a value consumed or produced (or both) in a method.
        /// </summary>
        public class Value
        {
            private readonly List<Value> _parents = new List<Value>();

            /// <summary>
            /// The instruction that produces this value.
            /// </summary>
            public Instruction Producer { get; private set; }

            /// <summary>
            /// The instruction that consumes this value.
            /// </summary>
            public Instruction Consumer { get; internal set; }

            /// <summary>
            /// A collection of values that this value is connected to via a consumer-producer
            /// relationship. More specifically, the parents are those values that were consumed in 
            /// order to produce this value.
            /// </summary>
            public ICollection<Value> Parents
            {
                get { return _parents.AsReadOnly(); }
            }

            public Value(Instruction producer)
            {
                Producer = producer;
            }

            internal void AddParents(IEnumerable<Value> parents)
            {
                _parents.AddRange(parents);
            }
        }
    }

    public class BranchingNotYetSupportedException : Exception
    {
    }
}
