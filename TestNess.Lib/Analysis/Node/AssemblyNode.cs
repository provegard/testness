using Mono.Cecil;

namespace TestNess.Lib.Analysis.Node
{
    public class AssemblyNode : AnalysisNode
    {
        public AssemblyDefinition Assembly { get; private set; }

        public AssemblyNode(AssemblyDefinition assembly)
        {
            Assembly = assembly;
        }

        public override void Accept(AnalysisTreeVisitor v)
        {
            v.Visit(this);
        }
    }
}
