using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNess.Target
{
    [TestClass]
    public class IntegerCalculatorBddStyleTest
    {
        private IntegerCalculator _calculator;
        private int _result;

        [TestInitialize]
        public void Setup()
        {
            _calculator = new IntegerCalculator();
            _result = 3;
        }
        
        [TestMethod]
        public void ItShouldBeAbleToAdd()
        {
            Assert.AreEqual(_result, _calculator.Add(1, 2));
        }
    }
}
