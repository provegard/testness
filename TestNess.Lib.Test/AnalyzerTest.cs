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
using NUnit.Framework;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class AnalyzerTest
    {
        [TestCase]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestThatRuleCollectionIsReadOnly()
        {
            var repo = new TestCaseRepository(TestHelper.GetTargetAssembly());
            var analyzer = new Analyzer(repo);

            analyzer.Rules.Clear();
        }

        [TestCase]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestThatViolationCollectionIsReadOnly()
        {
            var repo = new TestCaseRepository(TestHelper.GetTargetAssembly());
            var analyzer = new Analyzer(repo);

            analyzer.Violations.Clear();
        }


        [TestCase]
        public void TestThatRuleCanBeAddedToAnalyzer()
        {
            var repo = new TestCaseRepository(TestHelper.GetTargetAssembly());
            var analyzer = new Analyzer(repo);

            analyzer.AddRule(new OneAssertPerTestCaseRule());

            Assert.AreEqual(1, analyzer.Rules.Count);
        }

        [TestCase]
        public void TestThatScoreIsInitiallyNegative()
        {
            var analyzer = CreateAnalyzer();

            Assert.IsTrue(analyzer.Score < 0);
        }

        [TestCase]
        public void TestThatViolationListIsInitiallyEmpty()
        {
            var analyzer = CreateAnalyzer();

            Assert.AreEqual(0, analyzer.Violations.Count);
        }

        [TestCase]
        public void TestThatAnalyzingSetsScore()
        {
            var analyzer = CreateAnalyzer();
            analyzer.Analyze();

            Assert.IsTrue(analyzer.Score > 0);
        }

        [TestCase]
        public void TestThatAnalyzingFillsViolations()
        {
            var analyzer = CreateAnalyzer();
            analyzer.Analyze();

            Assert.AreNotEqual(0, analyzer.Violations.Count);
        }

        private static Analyzer CreateAnalyzer()
        {
            var repo = new TestCaseRepository(TestHelper.GetTargetAssembly());
            var analyzer = new Analyzer(repo);
            analyzer.AddRule(new OneAssertPerTestCaseRule());
            return analyzer;
        }
    }
}
