// Copyright (C) 2011-2013 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Linq;
using NUnit.Framework;
using TestNess.Lib.Analysis;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class TestSelfAwareness
    {
        [Test, Explicit]
        public void MyOwnScoreShouldBe0()
        {
            var assemblyFile = new Uri(GetType().Assembly.CodeBase).LocalPath;
            var repo = TestCases.LoadFromFile(assemblyFile);
            var rules = new Rules(typeof(IRule).Assembly);
            var results = AnalysisResults.Create(repo, rules, new ViolationScorer());
            var score = results.Applications.Sum(a => a.Score);
            PrintViolationsToConsole(results);
            Assert.AreEqual(0m, score);
        }

        private void PrintViolationsToConsole(AnalysisResults results)
        {
            foreach (var v in results.Applications.SelectMany(a => a.Violations))
            {
                Console.WriteLine(v);
            }
        }
    }
}
