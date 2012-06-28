using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestNess.Lib.Analysis;

namespace TestNess.Lib.Reporting.XUnit
{
    public class TestCaseNode : XUnitNode
    {
        public TestCaseRuleApplication Application { get; set; }

        public override void Accept(XUnitTreeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
