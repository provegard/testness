using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace TestNess.Lib
{
    public class MethodCall
    {
        public readonly Instruction Instruction;
        public readonly MethodReference MethodReference;

        public MethodCall(Instruction i, MethodReference m)
        {
            Instruction = i;
            MethodReference = m;
        }

        public MethodDefinition MethodDefinition
        {
            get
            {
                if (!MethodReference.IsDefinition)
                    throw new InvalidOperationException("Method is not a definition.");
                return MethodReference as MethodDefinition;
            }
        }

        public bool HasMethodDefinition
        {
            get { return MethodReference.IsDefinition; }
        }

        protected bool Equals(MethodCall other)
        {
            return Equals(Instruction, other.Instruction);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MethodCall) obj);
        }

        public override int GetHashCode()
        {
            return (Instruction != null ? Instruction.GetHashCode() : 0);
        }
    }
}