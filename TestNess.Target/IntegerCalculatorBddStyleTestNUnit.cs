
using NUnit.Framework;

namespace TestNess.Target
{
    [TestFixture]
    public class IntegerCalculatorBddStyleTestNUnit
    {
        private IntegerCalculator _calculator;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            _calculator = null; // so meaningful!
        }

        [SetUp]
        public void BeforeEach()
        {
            _calculator = new IntegerCalculator();
        }
        
        [Test]
        public void ItShouldBeAbleToAdd()
        {
            Assert.AreEqual(3, _calculator.Add(1, 2));
        }
    }
}
