// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TestNess.Lib.Rule
{
    /// <summary>
    /// Repository of rules. A repository is initialized with a number of assemblies, from which it extracts
    /// and creates all rules it can find. Only public rules that implement the <see cref="IRule"/> interface
    /// are considered.
    /// </summary>
    public class Rules : IEnumerable<IRule>
    {
        private readonly ICollection<IRule> _allRules;
 
        /// <summary>
        /// Creates a new rules repository.
        /// </summary>
        /// <param name="assemblies">One or more assemblies in which to look for rules.</param>
        public Rules(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                throw new ArgumentException("At least one assembly is required!");
            _allRules = new ReadOnlyCollection<IRule>(DiscoverRules(assemblies));
        }

        private IList<IRule> DiscoverRules(IEnumerable<Assembly> assemblies)
        {
            var list = new List<IRule>();
            foreach (var assembly in assemblies)
            {
                list.AddRange(from t in assembly.GetTypes() where IsRuleType(t) select NewRule(t));
            }
            return list;
        }

        private bool IsRuleType(Type t)
        {
            return t.IsPublic && typeof(IRule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract;
        }

        private IRule NewRule(Type type)
        {
            return (IRule) Activator.CreateInstance(type);
        }

        /// <summary>
        /// Returns a rule with the given name. The name of a rule is its class name, but the "Rule" suffix
        /// (if the class name has one) can be omitted.
        /// </summary>
        /// <param name="name">The name of the rule (with or without the "Rule" suffix).</param>
        /// <returns>A rule instance.</returns>
        /// <exception cref="NoSuchRuleException">Throw if no rule by the given name can be found.
        /// </exception>
        public IRule RuleByName(string name)
        {
            var pattern = "^" + Regex.Escape(name) + "(Rule)?$";
            var regex = new Regex(pattern);
            var cands = _allRules.Where(r => regex.IsMatch(r.GetType().Name));
            var ret = cands.FirstOrDefault();
            if (ret == null)
                throw new NoSuchRuleException(name);
            return ret;
        }

        public IEnumerator<IRule> GetEnumerator()
        {
            return _allRules.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class NoSuchRuleException : Exception
    {
        public NoSuchRuleException(string name) : base("Found no rule with the name " + name)
        {
        }
    }
}
