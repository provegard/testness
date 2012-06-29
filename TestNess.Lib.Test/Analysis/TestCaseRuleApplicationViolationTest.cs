// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Analysis
{
    [TestFixture]
    public class TestCaseRuleApplicationViolationTest
    {
        private TestCaseRuleApplication _app;

        [TestFixtureSetUp]
        public void GivenAnApplicationOfARuleToAnInappropriateTestCase()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorTest>(t => t.TestAddTwoAsserts());
            var rule = new LimitAssertsPerTestCaseRule();
            var scorer = Substitute.For<IViolationScorer>();
            scorer.CalculateScore(Arg.Any<Violation>()).Returns(1);

            _app = new TestCaseRuleApplication(tc, rule, scorer);
        }

        [Test]
        public void ThenThereIsAViolations()
        {
            Assert.AreEqual(1, _app.Violations.Count());
        }

        [Test]
        public void ThenTheScoreIsOne()
        {
            Assert.AreEqual(1, _app.Score);
        }
    }
}
