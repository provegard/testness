using TestNess.Lib.Rule;

namespace TestNess.Lib.Analysis
{
    public interface IViolationScorer
    {
        decimal CalculateScore(Violation v);
    }
}
