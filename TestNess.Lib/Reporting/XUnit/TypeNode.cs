using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace TestNess.Lib.Reporting.XUnit
{
    public class TypeNode : XUnitNode
    {
        public TypeDefinition Type { get; set; }

        public override bool Equals(object obj)
        {
            var tn = obj as TypeNode;
            return tn != null && Equals(Type, tn.Type);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override void Accept(XUnitTreeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
