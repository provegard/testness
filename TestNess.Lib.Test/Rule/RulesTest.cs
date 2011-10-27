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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var foundRules = rules.AllRules;

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
