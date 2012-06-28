// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using NUnit.Framework;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class ArgumentsTest
    {
        [TestCase]
        public void TestWithoutAssemblyName()
        {
            var args = Arguments.Parse(new string[0]);
            Assert.IsFalse(args.HasAssemblyFileName);
        }

        [TestCase]
        public void TestPresenceOfAssemblyNameCanBeChecked()
        {
            var args = Arguments.Parse(new[] { "anassembly.dll" });
            Assert.IsTrue(args.HasAssemblyFileName);
        }

        [TestCase]
        public void TestAssemblyNameIsRead()
        {
            var args = Arguments.Parse(new[] { "anassembly.dll" });
            Assert.AreEqual("anassembly.dll", args.AssemblyFileName);
        }

        [TestCase]
        public void TestThatOptionIsNotMistakenForAssemblyName()
        {
            var args = Arguments.Parse(new[] { "/c:testness.cfg", "anassembly.dll" });
            Assert.AreEqual("anassembly.dll", args.AssemblyFileName);
        }

        [TestCase]
        public void TestThatConfigFileIsReadFromOption()
        {
            var args = Arguments.Parse(new[] { "/c:testness.cfg", "anassembly.dll" });
            Assert.AreEqual("testness.cfg", args.ConfigurationFileName);
        }

        [TestCase]
        public void TestThatConfigFileIsReadFromLongOption()
        {
            var args = Arguments.Parse(new[] { "/config:testness.cfg", "anassembly.dll" });
            Assert.AreEqual("testness.cfg", args.ConfigurationFileName);
        }

        [TestCase]
        public void TestThatPreseceOfConfigFileCanBeChecked()
        {
            var args = Arguments.Parse(new[] { "/c:testness.cfg", "anassembly.dll" });
            Assert.IsTrue(args.HasConfigurationFileName);
        }

        [TestCase]
        public void TestThatUnrecognizedOptionThrows()
        {
            Assert.Throws<ArgumentException>(() => Arguments.Parse(new[] { "/x" }));
        }

        [TestCase]
        public void TestThatDefaultReporterIsPlain()
        {
            var args = Arguments.Parse(new string[0]);
            Assert.AreEqual(ReporterType.Plain, args.ReporterType);
        }

        [TestCase]
        public void TestThatPlainReporterCanBeSelectedWithArgument()
        {
            var args = Arguments.Parse(new[] { "/plain" });
            Assert.AreEqual(ReporterType.Plain, args.ReporterType);
        }

        [TestCase]
        public void TestThatXunitXmlReporterCanBeSelectedWithArgument()
        {
            var args = Arguments.Parse(new[] { "/xxml" });
            Assert.AreEqual(ReporterType.XunitXml, args.ReporterType);
        }

        [TestCase]
        public void TestThatXunitHtmlReporterCanBeSelectedWithArgument()
        {
            var args = Arguments.Parse(new[] { "/xhtml" });
            Assert.AreEqual(ReporterType.XunitHtml, args.ReporterType);
        }
    }
}
