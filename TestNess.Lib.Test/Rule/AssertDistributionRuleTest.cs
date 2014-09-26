// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class AssertDistributionRuleTest : AbstractRuleTest<AssertDistributionRule, IntegerCalculatorDistributionTest>
    {
        [TestCase("TestAddWithSingleAssert()", 0)]
        [TestCase("TestAddWithSpreadAsserts()", 1)]
        [TestCase("TestAddWithAssertsInTheMiddle()", 1)]
        [TestCase("TestAddWithWhitespaceSeparatedAssertsTowardsTheEnd()", 0)]
        public void TestViolationCountForDifferentMethods(string method, int expectedViolationCount)
        {
            IList<Instruction> instructions;
            var violations = FindViolations(method, out instructions);
            Assert.AreEqual(expectedViolationCount, violations.Count(), string.Join(Environment.NewLine, instructions.Select(DescIns)));
        }

        private object DescIns(Instruction i)
        {
            var sp = i.SequencePoint;
            return i + (sp != null ? string.Format(" SP:{0}-{1}", sp.StartLine, sp.EndLine) : "");
        }

        [Test]
        public void TestThatToStringDescribesRule()
        {
            var rule = new AssertDistributionRule();
            Assert.AreEqual("all asserts in a test case should be placed together last in the test method", rule.ToString());
        }
    }
}
