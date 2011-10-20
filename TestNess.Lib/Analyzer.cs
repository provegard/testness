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

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestNess.Lib
{
    public class Analyzer
    {
        private readonly TestCaseRepository _repo;
        private readonly CompoundRule _rule = new CompoundRule();
        private readonly List<Violation> _violations = new List<Violation>();

        public Analyzer(TestCaseRepository repo)
        {
            _repo = repo;
            Score = -1; // no initial score
        }

        public ICollection<IRule> Rules
        {
            get { return new ReadOnlyCollection<IRule>(new List<IRule>(_rule.Rules));  }
        }

        public int Score { get; private set; }

        public ICollection<Violation> Violations
        {
            get { return new ReadOnlyCollection<Violation>(_violations); }
        }

        public void AddRule(IRule rule)
        {
            _rule.Rules.Add(rule);
        }

        public void Analyze()
        {
            _violations.Clear();
            foreach (var tc in _repo.GetAllTestCases())
            {
                _violations.AddRange(_rule.Apply(tc));
            }
            UpdateScore();
        }

        private void UpdateScore()
        {
            Score = _violations.Count;
        }
    }
}