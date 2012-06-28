// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Linq;
using NUnit.Framework;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class RulesTest
    {
        [TestCase, ExpectedException(typeof(ArgumentException))]
        public void TestAtLeastOneAssemblyIsRequired()
        {
            new Rules(); // should throw
        }

        [TestCase]
        public void TestFindRulesInSuppliedAssembly()
        {
            var rules = new Rules(typeof(IRule).Assembly);

            var foundRules = rules; //.AllRules;

            // Rules are not singleton, so find by type...
            var soughtRule = foundRules.Where(r => r.GetType() == typeof(LimitAssertsPerTestCaseRule));
            Assert.IsNotNull(soughtRule.FirstOrDefault());
        }

        [TestCase]
        public void TestFindRuleByName()
        {
            var rules = new Rules(typeof(IRule).Assembly);

            var soughtRule = rules.RuleByName("LimitAssertsPerTestCaseRule");

            Assert.IsNotNull(soughtRule);
        }

        [TestCase]
        public void TestFindRuleByNameWithoutRuleSuffix()
        {
            var rules = new Rules(typeof(IRule).Assembly);

            var soughtRule = rules.RuleByName("LimitAssertsPerTestCase");

            Assert.IsNotNull(soughtRule);
        }

        [TestCase, ExpectedException(typeof(NoSuchRuleException))]
        public void TestFindingMissingRuleThrows()
        {
            var rules = new Rules(typeof(IRule).Assembly);

            rules.RuleByName("Dummy"); // should throw
        }

    }
}
