// Copyright (C) 2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2014.
using System;
using System.Linq;
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Lib.TestFramework;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class AvoidAssertBooleanRuleTest
    {
        private static readonly Action _violatingAction = () =>
        {
            var x = 5;
            var y = 6;
            Assert.False(x == y);
        };

        [Test]
        public void ItShouldNotComplainAboutAssertingASingleBooleanValue()
        {
            Action a = () =>
            {
                var actual = Environment.Is64BitOperatingSystem;
                Assert.True(actual);
            };
            var rule = new AvoidAssertBooleanRule();
            var violations = rule.Apply(a.AsTestCase(new NUnitTestFramework()));
            Assert.AreEqual(0, violations.Count());
        }

        [Test]
        public void ItShouldComplainAboutAssertingABooleanValueResultingFromAnOperatorComparison()
        {
            var rule = new AvoidAssertBooleanRule();
            var violations = rule.Apply(_violatingAction.AsTestCase(new NUnitTestFramework()));
            Assert.AreEqual(1, violations.Count());
        }

        [Test]
        public void ItShouldProvideAMessage()
        {
            var rule = new AvoidAssertBooleanRule();
            var violations = rule.Apply(_violatingAction.AsTestCase(new NUnitTestFramework()));
            var message = violations.First().Message;
            Assert.AreEqual("Assert.False performs a boolean test on a composite boolean value", message);
        }

        [Test]
        public void ItShouldPointToTheLocationOfTheAssertion()
        {
            var rule = new AvoidAssertBooleanRule();
            var violations = rule.Apply(_violatingAction.AsTestCase(new NUnitTestFramework()));
            var location = violations.First().Location;
            Assert.AreEqual(19, location.StartLine);
        }

        [Test]
        public void ItShouldHaveAToStringDescription()
        {
            var rule = new AvoidAssertBooleanRule();
            Assert.AreEqual("a test case should not do a boolean assertion of a composite boolean value", rule.ToString());
        }

        [Test]
        public void ItShouldComplainAboutAssertingABooleanValueResultingFromAEqualsComparison()
        {
            Action a = () =>
            {
                var x = 5;
                var y = 6;
                Assert.False(x.Equals(y));
            };
            var rule = new AvoidAssertBooleanRule();
            var violations = rule.Apply(a.AsTestCase(new NUnitTestFramework()));
            Assert.AreEqual(1, violations.Count());
        }
    }
}
