using TestNess.Lib.Rule;

namespace TestNess.Lib.Analysis
{
    public interface IViolationScorer
    {
        int CalculateScore(Violation v);
    }
}
