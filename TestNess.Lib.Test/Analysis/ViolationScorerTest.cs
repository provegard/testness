using NSubstitute;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Analysis
{
    [TestFixture]
    public class ViolationScorerTest
    {
        [Test]
        public void ItShouldTakeSeverityFactorIntoAccount()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddBasic());

            var violation = new Violation(Substitute.For<IRule>(), tc, severityFactor: 1.2m);
            var scorer = new ViolationScorer();

            Assert.AreEqual(1.2m, scorer.CalculateScore(violation));
        }
    }
}
