using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestNess.Lib.Analysis;

namespace TestNess.Lib.Reporting
{
    public interface IReporter
    {
        void GenerateReport(AnalysisResults results);
    }
}
