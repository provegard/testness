using System;

namespace TestNess.Lib.Analysis.Node
{
    public class NamespaceNode : AnalysisNode
    {
        public string Name { get; private set; }

        public NamespaceNode(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var nn = obj as NamespaceNode;
            return nn != null && Equals(Name, nn.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override void Accept(AnalysisTreeVisitor v)
        {
            v.Visit(this);
        }
    }
}
