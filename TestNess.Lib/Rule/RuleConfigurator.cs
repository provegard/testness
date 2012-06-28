// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TestNess.Lib.Rule
{
    /// <summary>
    /// An object of this class can read a rule configuration string that consists of zero or more lines
    /// on the form "Rule.Setting=Value". Blank lines and lines that start with "#" are ignored. Multiple
    /// lines can refer to the same rule, but must differ in setting names.
    /// <para>A rule name corresponds to the class name of a rule. A setting name corresponds to a public,
    /// writable property in the rule class.</para>
    /// <para>The configuration can subsequently be applied to rules in a <see cref="Rules"/> repository.
    /// If at this point a rule cannot be found, a setting cannot be found or a setting cannot be written
    /// to, an exception is thrown.
    /// </para>
    /// </summary>
    public class RuleConfigurator
    {
        private readonly IDictionary<string, IList<RuleSetting>> _rulesSettings = new Dictionary<string, IList<RuleSetting>>();

        /// <summary>
        /// Reads the configuration found in the given string. This method is cumulative, so calling it
        /// multiple times will add to the total configuration.
        /// </summary>
        /// <param name="config">A configuration string.</param>
        /// <exception cref="MalformedRuleConfiguration">Throw if the configuration string cannot be
        /// parsed.</exception>
        public void ReadConfiguration(string config)
        {
            var individualSettings = Regex.Split(config, "\r?\n");
            foreach (var individualSetting in individualSettings)
            {
                if (individualSetting.StartsWith("#"))
                    continue; // comment line
                var trimmed = individualSetting.Trim();
                if ("".Equals(trimmed))
                    continue; // empty line
                var parts = Regex.Split(trimmed, "\\.|=");
                if (parts.Length != 3)
                    throw new MalformedRuleConfiguration(string.Format("Setting is malformed, must be on the form Rule.Setting=Value: " + trimmed));

                var rule = parts[0];
                var setting = parts[1].TrimEnd();
                var value = parts[2].Trim();

                var rs = new RuleSetting {Name = setting, Value = value};
                IList<RuleSetting> settingsList;
                if (!_rulesSettings.TryGetValue(rule, out settingsList))
                {
                    settingsList = new List<RuleSetting>();
                    _rulesSettings.Add(rule, settingsList);
                }
                if (settingsList.Where(s => s.Name.Equals(setting)).Count() > 0)
                    throw new MalformedRuleConfiguration("Duplicate setting: " + trimmed);
                settingsList.Add(rs);
            }
        }

        /// <summary>
        /// The number of rules that will be configured. This includes all rules, even though some may not
        /// be found when the configuration is applied.
        /// </summary>
        public int NumberOfRulesToConfigure { get { return _rulesSettings.Count; } }

        /// <summary>
        /// Applies the configuration to the rules found in the given repository.
        /// </summary>
        /// <param name="rules">A repository of rules.</param>
        /// <exception cref="NoSuchRuleException">Thrown if a rule in the configuration cannot be found.
        /// </exception>
        /// <exception cref="MalformedRuleConfiguration">Thrown if a public, writeable setting cannot be 
        /// found on a rule, or if the value of the setting has the wrong type.</exception>
        public void ApplyConfiguration(Rules rules)
        {
            foreach (var entry in _rulesSettings)
            {
                var rule = rules.RuleByName(entry.Key);
                foreach (var setting in entry.Value)
                {
                    var prop = FindProperty(rule, setting.Name);
                    if (prop == null)
                        throw new MalformedRuleConfiguration("Missing or non-writeable setting: " + setting.Name);
                    try
                    {
                        var typedValue = Convert.ChangeType(setting.Value, prop.PropertyType);
                        prop.SetValue(rule, typedValue, BindingFlags.Default, null, null, null);
                    }
                    catch (FormatException)
                    {
                        throw new MalformedRuleConfiguration(string.Format("Value for setting {0} does not match the setting type: {1}", setting.Name, setting.Value));
                    }
                }
            }
        }

        private PropertyInfo FindProperty(object rule, string name)
        {
            var prop = rule.GetType().GetProperty(name);
            return prop != null && prop.GetSetMethod() != null ? prop : null;
        }

        private struct RuleSetting
        {
            internal string Name;
            internal string Value;
        }
    }

    public class MalformedRuleConfiguration : Exception
    {
        public MalformedRuleConfiguration(string message) : base(message)
        {
        }
    }
}
