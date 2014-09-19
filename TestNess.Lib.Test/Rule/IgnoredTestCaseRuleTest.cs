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

        [Test]
        public void TestThatIgnoredWithoutReasonHasHighSeverity()
        {
            var violation = FindViolations("TestIgnoredAdd()").First();
            Assert.AreEqual(1.5m, violation.SeverityFactor);
        }

        [Test]
        public void TestThatIgnoredWithoutReasonHasSpecificMessage()
        {
            var violation = FindViolations("TestIgnoredAdd()").First();
            StringAssert.Contains("without reason", violation.Message);
        }

        private Violation GetIgnoreViolationWithReason()
        {
            // MSTest doesn't support ignore reason, so we have to target NUnit here.
            var tc = typeof(IntegerCalculatorTestNUnit).FindTestCase("TestIgnoredAddWithReason()");
            var rule = new IgnoredTestCaseRule();
            return rule.Apply(tc).First();
        }

        [Test]
        public void TestThatIgnoredWithReasonHasSpecificMessage()
        {
            var violation = GetIgnoreViolationWithReason();
            StringAssert.Contains("with reason", violation.Message);
        }

        [Test]
        public void TestThatIgnoredWithReasonHasNormalSeverity()
        {
            var violation = GetIgnoreViolationWithReason();
            Assert.AreEqual(1m, violation.SeverityFactor);
        }
    }
}
