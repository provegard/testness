using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace TestNess.Lib.Reporting.XUnit
{
    public class AssemblyNode : XUnitNode
    {
        public AssemblyDefinition Assembly { get; set; }

        public override void Accept(XUnitTreeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
