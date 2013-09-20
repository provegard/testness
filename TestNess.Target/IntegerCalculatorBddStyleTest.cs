using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNess.Target
{
    [TestClass]
    public class IntegerCalculatorBddStyleTest
    {
        private IntegerCalculator _calculator;

        [TestInitialize]
        public void Setup()
        {
            _calculator = new IntegerCalculator();
        }
        
        [TestMethod]
        public void ItShouldBeAbleToAdd()
        {
            Assert.AreEqual(3, _calculator.Add(1, 2));
        }
    }
}
