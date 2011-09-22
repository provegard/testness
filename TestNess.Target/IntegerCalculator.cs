namespace TestNess.Target
{
    /// <summary>
    /// Simple calculator that can perform the four basic arithmetic operations add, subtract, multiply and divide.
    /// The calculator only operates on integers.
    /// <para>This class is the target for the test cases that TestNess evaluates.</para>
    /// </summary>
    public class IntegerCalculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Subtract(int a, int b)
        {
            return a - b;
        }

        public int Divide(int a, int b)
        {
            return a / b;
        }

        public int Multiply(int a, int b)
        {
            return a * b;
        }
    }
}
