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

using System.Collections.Generic;
using NUnit.Framework;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class RuleConfiguratorTest
    {
        private RuleConfigurator _configurator;
        private Rules _rules;

        [SetUp]
        public void WithConfiguratorAndRules()
        {
            _configurator = new RuleConfigurator();
            _rules = new Rules(typeof(IRule).Assembly);
        }

        [TestCase]
        public void TestThatConfiguratorAcceptsEmptyConfiguration()
        {
            _configurator.ReadConfiguration("");
            Assert.AreEqual(0, _configurator.NumberOfRulesToConfigure);
        }

        [TestCase]
        public void TestThatConfiguratorAcceptsSingleKeyValuePair()
        {
            _configurator.ReadConfiguration("Rule.Setting=5");
            Assert.AreEqual(1, _configurator.NumberOfRulesToConfigure);
        }

        [TestCase]
        public void TestThatConfiguratorAcceptsMultipleKeyValuePair()
        {
            _configurator.ReadConfiguration("Rule.Setting=5\nRule2.Setting=1");
            Assert.AreEqual(2, _configurator.NumberOfRulesToConfigure);
        }

        [TestCase]
        public void TestThatConfiguratorAcceptsMultipleKeyValuePairWithCrLf()
        {
            _configurator.ReadConfiguration("Rule.Setting=5\r\nRule2.Setting=1");
            Assert.AreEqual(2, _configurator.NumberOfRulesToConfigure);
        }

        [TestCase]
        public void TestThatConfiguratorIgnoresBlankLinesInConfig()
        {
            _configurator.ReadConfiguration("Rule.Setting=5\n  \nRule2.Setting=1");
            Assert.AreEqual(2, _configurator.NumberOfRulesToConfigure);
        }

        [TestCase]
        public void TestThatConfiguratorIgnoresCommentLinesInConfig()
        {
            _configurator.ReadConfiguration("Rule.Setting=5\n# a comment\nRule2.Setting=1");
            Assert.AreEqual(2, _configurator.NumberOfRulesToConfigure);
        }

        [TestCase]
        public void TestThatConfiguratorCountsRulesNotSettings()
        {
            _configurator.ReadConfiguration("Rule.Setting1=5\n\nRule.Setting2=1");
            Assert.AreEqual(1, _configurator.NumberOfRulesToConfigure);
        }

        [TestCase, ExpectedException(typeof(MalformedRuleConfiguration))]
        public void TestThatConfiguratorThrowsOnMissingSettingValue()
        {
            _configurator.ReadConfiguration("Rule.Setting1");
        }

        [TestCase, ExpectedException(typeof(MalformedRuleConfiguration))]
        public void TestThatConfiguratorThrowsOnDuplicateSetting()
        {
            _configurator.ReadConfiguration("Rule.Setting1=1\nRule.Setting1=2");
        }

        [TestCase, ExpectedException(typeof(MalformedRuleConfiguration))]
        public void TestThatConfiguratorThrowsOnMissingSetting()
        {
            _configurator.ReadConfiguration("Rule=5");
        }

        [TestCase]
        public void TestThatConfigurationCanBeAppliedToRules()
        {
            _configurator.ReadConfiguration("LimitAssertsPerTestCase.MaxNumberOfAsserts=2");
            _configurator.ApplyConfiguration(_rules);

            var rule = _rules.RuleByName("LimitAssertsPerTestCase");
            Assert.AreEqual(2, ((LimitAssertsPerTestCaseRule) rule).MaxNumberOfAsserts);
        }

        [TestCase]
        public void TestThatConfigurationWithWhitespaceCanBeAppliedToRules()
        {
            _configurator.ReadConfiguration("LimitAssertsPerTestCase.MaxNumberOfAsserts = 2 ");
            _configurator.ApplyConfiguration(_rules);

            var rule = _rules.RuleByName("LimitAssertsPerTestCase");
            Assert.AreEqual(2, ((LimitAssertsPerTestCaseRule)rule).MaxNumberOfAsserts);
        }

        [TestCase, ExpectedException(typeof(NoSuchRuleException))]
        public void TestThatApplyingConfigThrowsOnUnrecognizedRule()
        {
            _configurator.ReadConfiguration("NotARule.MaxNumberOfAsserts=2");
            _configurator.ApplyConfiguration(_rules); // should throw
        }
        
        [TestCase, ExpectedException(typeof(MalformedRuleConfiguration))]
        public void TestThatApplyingConfigThrowsOnUnrecognizedSetting()
        {
            _configurator.ReadConfiguration("LimitAssertsPerTestCase.FooBar=2");
            _configurator.ApplyConfiguration(_rules); // should throw
        }

        [TestCase, ExpectedException(typeof(MalformedRuleConfiguration))]
        public void TestThatApplyingConfigThrowsOnInvalidValue()
        {
            _configurator.ReadConfiguration("LimitAssertsPerTestCase.MaxNumberOfAsserts=astring");
            _configurator.ApplyConfiguration(_rules); // should throw
        }

        [TestCase, ExpectedException(typeof(MalformedRuleConfiguration))]
        public void TestThatApplyingConfigThrowsOnNonWriteableSetting()
        {
            _configurator.ReadConfiguration("ATestRule.ASetting=value");
            var rules = new Rules(typeof (ATestRule).Assembly);

            _configurator.ApplyConfiguration(rules); // should throw
        }
    }

    public class ATestRule : IRule
    {
        public string ASetting { get; private set; }

        public IEnumerable<Violation> Apply(TestCase testCase)
        {
            yield break;
        }
    }
}
