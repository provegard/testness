﻿/**
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
using TestNess.Lib.Rule;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class AnalyzerTest
    {
        private Analyzer _analyzer;

        [SetUp]
        public void WithAnalyzer()
        {
            var repo = new TestCaseRepository(TestHelper.GetTargetAssembly());
            _analyzer = new Analyzer(repo, new Rules(typeof(IRule).Assembly));
        }

        [TestCase]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestThatRuleCollectionIsReadOnly()
        {
            _analyzer.Rules.Clear();
        }

        [TestCase]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestThatViolationCollectionIsReadOnly()
        {
            _analyzer.Violations.Clear();
        }
        
        [TestCase]
        public void TestThatScoreIsInitiallyNegative()
        {
            Assert.IsTrue(_analyzer.Score < 0);
        }

        [TestCase]
        public void TestThatViolationListIsInitiallyEmpty()
        {
            Assert.AreEqual(0, _analyzer.Violations.Count);
        }

        [TestCase]
        public void TestThatAnalyzingSetsScore()
        {
            _analyzer.Analyze();

            Assert.That(_analyzer.Score > 0);
        }

        [TestCase]
        public void TestThatAnalyzingFillsViolations()
        {
            _analyzer.Analyze();

            Assert.AreNotEqual(0, _analyzer.Violations.Count);
        }
    }
}
