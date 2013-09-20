// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Linq;
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class LocalExpectationRuleBddStyleTest : AbstractRuleTest<LocalExpectationRule, IntegerCalculatorBddStyleTest>
    {
        [Test]
        public void TestThatAValueProducedInASetupMethodIsConsideredLocallyProduced()
        {
            var violations = FindViolations("ItShouldBeAbleToAdd()");
            Assert.AreEqual(0, violations.Count());
        }
    }
}
