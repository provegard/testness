using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestNess.Lib.Reporting.XUnit
{
    public abstract class XUnitNode
    {
        public abstract void Accept(XUnitTreeVisitor visitor);
    }
}
