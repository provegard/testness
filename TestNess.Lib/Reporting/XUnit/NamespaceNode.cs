using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestNess.Lib.Reporting.XUnit
{
    public class NamespaceNode : XUnitNode
    {
        public string Namespace { get; set; }

        public override bool Equals(object obj)
        {
            var nn = obj as NamespaceNode;
            return nn != null && Equals(Namespace, nn.Namespace);
        }

        public override int GetHashCode()
        {
            return Namespace.GetHashCode();
        }

        public override void Accept(XUnitTreeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public string ParentNamespace()
        {
            int idx = Namespace.LastIndexOf('.');
            if (idx < 0)
                throw new InvalidOperationException("No parent for namespace " + Namespace);
            return Namespace.Substring(0, idx);
        }
    }
}
