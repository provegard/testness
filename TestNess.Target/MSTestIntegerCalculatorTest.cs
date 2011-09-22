using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNess.Target
{
    /// <summary>
    /// Specialization of <c>IntegerCalculatorTestBase</c> for the MSTest unit test framework. The MSTest
    /// framework is included in Visual Studio.
    /// </summary>
    public class MsTestIntegerCalculatorTest : IntegerCalculatorTestBase
    {
        [TestMethod]
        public override void TestAddBasic()
        {
            base.TestAddBasic();
        }

        protected override void DoAssertEqual(int expected, int actual)
        {
            Assert.AreEqual(expected, actual);
        }
    }
}
