using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphBuilder;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace TestNess.Lib.Cil
{
    public static class InstructionGraph
    {
        public static Graph<Instruction> CreateFrom(MethodDefinition method)
        {
            return CreateInstructionGraph(method);
        }

        public static IEnumerable<IList<Instruction>> FindInstructionPaths(this Graph<Instruction> igraph)
        {
            var returns = igraph.Walk().Where(i => i.OpCode == OpCodes.Ret);
            var paths = new List<IList<Instruction>>();
            foreach (var ret in returns)
            {
                paths.AddRange(igraph.FindPaths(igraph.Root, ret));
            }
            return paths;
        }

        public static bool ContainsLoop(this IList<Instruction> path)
        {
            return path.Count != path.Distinct().Count();
        }

        private static Graph<Instruction> CreateInstructionGraph(MethodDefinition method)
        {
            var builder = new GraphBuilder<Instruction>(NextInstructions);
            return builder.Build(method.Body.Instructions[0]);
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
                        yield return (Instruction)v.Operand;
                    break;

                case FlowControl.Branch:
                    yield return (Instruction)v.Operand;
                    break;

                default:
                    throw new NotImplementedException(fc.ToString());
            }
        }
    }
}
