using System.Linq;
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class IgnoredTestCaseRuleTest : AbstractRuleTest<IgnoredTestCaseRule, IntegerCalculatorTest>
    {
        [TestCase("TestAddBasic()", 0)]
        [TestCase("TestIgnoredAdd()", 1)]
        public void TestViolationCountForDifferentMethods(string method, int expectedViolationCount)
        {
            var violations = FindViolations(method);
            Assert.AreEqual(expectedViolationCount, violations.Count());
        }

        [Test]
        public void TestThatToStringDescribesRule()
        {
            var rule = new IgnoredTestCaseRule();
            Assert.AreEqual("a test case should not be ignored", rule.ToString());
        }
    }
}
