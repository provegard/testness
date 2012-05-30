using System;
using Mono.Cecil;

namespace TestNess.Lib.Analysis.Node
{
    public class TypeNode : AnalysisNode
    {
        public TypeNode(TypeDefinition type)
        {
            Type = type;
        }

        public TypeDefinition Type { get; private set; }

        public override bool Equals(object obj)
        {
            var tn = obj as TypeNode;
            return tn != null && Equals(Type, tn.Type);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override void Accept(AnalysisTreeVisitor v)
        {
            v.Visit(this);
        }
    }
}
