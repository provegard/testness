// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

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
