using TestNess.Lib.Rule;

namespace TestNess.Lib.Analysis
{
    public class ViolationScorer : IViolationScorer
    {
        public decimal CalculateScore(Violation v)
        {
            return 1.0m;
        }
    }
}
