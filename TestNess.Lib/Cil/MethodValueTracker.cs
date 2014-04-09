// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
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
        /// List of graphs of values produced and consumed in the method this tracker was created for. 
        /// The root of each graph is a fake value which only serves the purpose of being root. Each graph
        /// corresponds to a single path through the method; thus if the method does not contain branching,
        /// there is only one graph.
        /// </summary>
        public IList<Graph<Value>> ValueGraphs { get; private set; }

        public MethodValueTracker(MethodDefinition method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            _method = method;
            var paths = FindInstructionPaths();
            ValueGraphs = new ReadOnlyCollection<Graph<Value>>(CreateValueGraph(paths));
        }

        private IEnumerable<IList<Instruction>> FindInstructionPaths()
        {
            var igraph = CreateInstructionGraph();
            var returns = igraph.Walk().Where(i => i.OpCode == OpCodes.Ret);
            var paths = new List<IList<Instruction>>();
            foreach (var ret in returns)
            {
                paths.AddRange(igraph.FindPaths(igraph.Root, ret));
            }
            return paths;
        }

        private Graph<Instruction> CreateInstructionGraph()
        {
            var builder = new GraphBuilder<Instruction>(NextInstructions);
            return builder.Build(_method.Body.Instructions[0]);
        }

        private static IEnumerable<Instruction> NextInstructions(Instruction v)
        {
            var fc = v.OpCode.FlowControl;
            switch (fc)
            {
                case FlowControl.Next:
                case FlowControl.Call: // stay within the method
                    yield return v.Next;
                    break;

                case FlowControl.Return:
                    yield break;

                case FlowControl.Cond_Branch:
                    yield return v.Next;
                    if (v.Operand is Instruction[])
                    {
                        // switch statement
                        foreach (var i in (Instruction[])v.Operand)
                            yield return i;
                    }
                    else
                        yield return (Instruction) v.Operand;
                    break;

                case FlowControl.Branch:
                    yield return (Instruction) v.Operand;
                    break;

                default:
                    throw new NotImplementedException(fc.ToString());
            }
        }

        private IList<Graph<Value>> CreateValueGraph(IEnumerable<IList<Instruction>> instructionPaths)
        {
            return instructionPaths.Select(CreateValueGraph).ToList();
        }

        private Graph<Value> CreateValueGraph(IEnumerable<Instruction> instructions)
        {
            var stack = new Stack<Value>();
            var values = new List<Value>();
            var locals = new Value[_method.Body.Variables.Count];

            foreach (var instruction in instructions)
            {
                Apply(instruction, stack, values, locals);
            }
            Debug.Assert(stack.Count == 0, "Stack contains unpopped values.");

            return CreateGraph(values);
        }

        private void Apply(Instruction instruction, Stack<Value> stack, ICollection<Value> values, IList<Value> locals)
        {
            // TODO: this method is a mess and needs to be refactored!!

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
        /// Returns the values that are consumed by the given instruction when it is part of the
        /// given value graph. The "this object" value needed for a virtual call 
        /// <strong>is not</strong> included. If the instruction does not participate in the given
        /// value graph, an empty enumerable of values is returned.
        /// </summary>
        /// <param name="valueGraph">One of the value graphs produced by this tracker.</param>
        /// <param name="query">An instruction that consumes values.</param>
        /// <returns>An enumerable of values consumed by the instruction.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any of the arguments is <c>null</c>.
        /// </exception>
        public IEnumerable<Value> GetConsumedValues(Graph<Value> valueGraph, Instruction query)
        {
            if (valueGraph == null)
                throw new ArgumentNullException("valueGraph");
            if (query == null)
                throw new ArgumentNullException("query");
            return valueGraph.Walk().Where(v => v.Consumer == query).Where(Not_IsThisObjectValue);
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
        /// <exception cref="ArgumentException">Thrown if the value does not belong to any of the value
        /// graphs produced by this tracker.</exception>
        public IEnumerable<Value> FindSourceValues(Value value)
        {
            var graph = ValueGraphs.Where(vg => vg.Contains(value)).FirstOrDefault();
            //if (graph == null)
            //    throw new ArgumentException("Unknown value.");
            return graph.Walk(value).Where(v => v.Parents.Count == 0);
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
}
