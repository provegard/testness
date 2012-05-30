using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Analysis
{
    public class ViolationScorer : IViolationScorer
    {
        public int CalculateScore(Violation v)
        {
            return 1;
        }
    }
}
