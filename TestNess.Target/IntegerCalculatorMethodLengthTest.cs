// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
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
