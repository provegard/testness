namespace TestNess.Target
{
    /// <summary>
    /// This is the base class for test case classes. Unattributed test methods are placed in this class, while sub
    /// classes add custom attributes specific for a particular unit test framework. 
    /// </summary>
    public abstract class IntegerCalculatorTestBase
    {
        public virtual void TestAddBasic()
        {
            var calculator = new IntegerCalculator();

            var actual = calculator.Add(1, 2);

            DoAssertEqual(3, actual);
        }

        protected abstract void DoAssertEqual(int expected, int actual);
    }
}
