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
using System.Collections.ObjectModel;
using TestNess.Lib.Rule;

namespace TestNess.Lib
{
    public class Analyzer
    {
        private readonly TestCaseRepository _repo;
        private readonly List<Violation> _violations = new List<Violation>();
        private readonly Rules _rules;

        public Analyzer(TestCaseRepository repo, Rules rules)
        {
            _rules = rules;
            _repo = repo;
            Score = -1; // no initial score
        }

        public ICollection<IRule> Rules
        {
            get { return _rules.AllRules;  }
        }

        public int Score { get; private set; }

        public ICollection<Violation> Violations
        {
            get { return new ReadOnlyCollection<Violation>(_violations); }
        }

        public void Analyze()
        {
            var compound = CreateCompoundRule();
            _violations.Clear();
            foreach (var tc in _repo.GetAllTestCases())
            {
                _violations.AddRange(compound.Apply(tc));
            }
            UpdateScore();
        }

        private IRule CreateCompoundRule()
        {
            var compound = new CompoundRule();
            foreach (var rule in _rules.AllRules)
            {
                compound.Rules.Add(rule);
            }
            return compound;
        }

        private void UpdateScore()
        {
            Score = _violations.Count;
        }
    }
}
