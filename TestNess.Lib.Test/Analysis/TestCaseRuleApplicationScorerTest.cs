using NSubstitute;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Analysis
{
    [TestFixture]
    public class TestCaseRuleApplicationScorerTest
    {
        private TestCaseRuleApplication _app;

        [TestFixtureSetUp]
        public void GivenAnApplicationOfARuleToAnInappropriateTestCase()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddTwoAsserts());
            var rule = new LimitAssertsPerTestCaseRule();
            var scorer = Substitute.For<IViolationScorer>();
            scorer.CalculateScore(Arg.Any<Violation>()).Returns(0.5m);

            _app = new TestCaseRuleApplication(tc, rule, scorer);
        }

        [Test]
        public void ThenTheScoreIsCalculatedUsingTheScorer()
        {
            Assert.AreEqual(0.5m, _app.Score);
        }
    }
}
