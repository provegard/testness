/**
 * Copyright (C) 2011 by Per Rovegård (per@rovegard.se)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestNess.Target
{
    [TestClass]
    public class IntegerCalculatorMethodLengthTest
    {
        private IntegerCalculator _calc;

        [TestInitialize]
        public void GivenACalculator()
        {
            _calc = new IntegerCalculator();
        }

        [TestMethod]
        public void ThenWeShouldBeAbleToAdd_Short()
        {
            Assert.AreEqual(3, _calc.Add(1, 2));
        }

        [TestMethod]
        public void ThenWeShouldBeAbleToAdd_Long()
        {
            // > 10 LOC, silly method...
            var one = 1;
            var two = 2;
            // some swapping
            one ^= two;
            two ^= one;
            one ^= two;
            // swap back
            one ^= two;
            two ^= one;
            one ^= two;
            // more crap
            one *= two;
            one /= two;

            Assert.AreEqual(3, _calc.Add(one, two));
        }
    }
}
