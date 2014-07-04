// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Text;
using NUnit.Framework;

namespace TestNess.Lib.Test
{
    [TestFixture]
    public class ArgumentsTest
    {
        [Test]
        public void TestWithoutAssemblyName()
        {
            var args = Arguments.Parse(new string[0]);
            Assert.IsFalse(args.HasAssemblyFileName);
        }

        [Test]
        public void TestPresenceOfAssemblyNameCanBeChecked()
        {
            var args = Arguments.Parse(new[] { "anassembly.dll" });
            Assert.IsTrue(args.HasAssemblyFileName);
        }

        [Test]
        public void TestAssemblyNameIsRead()
        {
            var args = Arguments.Parse(new[] { "anassembly.dll" });
            Assert.AreEqual("anassembly.dll", args.AssemblyFileName);
        }

        [Test]
        public void TestThatOptionIsNotMistakenForAssemblyName()
        {
            var args = Arguments.Parse(new[] { "/c:testness.cfg", "anassembly.dll" });
            Assert.AreEqual("anassembly.dll", args.AssemblyFileName);
        }

        [Test]
        public void TestThatConfigFileIsReadFromOption()
        {
            var args = Arguments.Parse(new[] { "/c:testness.cfg", "anassembly.dll" });
            Assert.AreEqual("testness.cfg", args.ConfigurationFileName);
        }

        [Test]
        public void TestThatConfigFileIsReadFromLongOption()
        {
            var args = Arguments.Parse(new[] { "/config:testness.cfg", "anassembly.dll" });
            Assert.AreEqual("testness.cfg", args.ConfigurationFileName);
        }

        [Test]
        public void TestThatPreseceOfConfigFileCanBeChecked()
        {
            var args = Arguments.Parse(new[] { "/c:testness.cfg", "anassembly.dll" });
            Assert.IsTrue(args.HasConfigurationFileName);
        }

        [Test]
        public void TestThatUnrecognizedOptionThrows()
        {
            Assert.Throws<ArgumentException>(() => Arguments.Parse(new[] { "/x" }));
        }

        [Test]
        public void TestThatDefaultReporterIsPlain()
        {
            var args = Arguments.Parse(new string[0]);
            Assert.AreEqual(ReporterType.Plain, args.ReporterType);
        }

        [Test]
        public void TestThatPlainReporterCanBeSelectedWithArgument()
        {
            var args = Arguments.Parse(new[] { "/plain" });
            Assert.AreEqual(ReporterType.Plain, args.ReporterType);
        }

        [Test]
        public void TestThatReportPathCanBeSelectedWithArgument()
        {
            var args = Arguments.Parse(new[] { "/output:foo.txt" });
            Assert.AreEqual("foo.txt", args.ReportFilePath);
        }

        [Test]
        public void TestThatXunitXmlReporterCanBeSelectedWithArgument()
        {
            var args = Arguments.Parse(new[] { "/xxml" });
            Assert.AreEqual(ReporterType.XunitXml, args.ReporterType);
        }

        [Test]
        public void TestThatXunitHtmlReporterCanBeSelectedWithArgument()
        {
            var args = Arguments.Parse(new[] { "/xhtml" });
            Assert.AreEqual(ReporterType.XunitHtml, args.ReporterType);
        }

        [Test]
        public void TestThatEncodingCanBeSpecified()
        {
            var args = Arguments.Parse(new[] { "/e:utf-8" });
            Assert.AreEqual(Encoding.UTF8, args.OutputEncoding);
        }

        [Test]
        public void TestThatEncodingCanBeSpecifiedUsingLongOption()
        {
            var args = Arguments.Parse(new[] { "/encoding:utf-8" });
            Assert.AreEqual(Encoding.UTF8, args.OutputEncoding);
        }

        [Test]
        public void TestThatEncodingCanBeSpecifiedUsingMediumOption()
        {
            var args = Arguments.Parse(new[] { "/enc:utf-8" });
            Assert.AreEqual(Encoding.UTF8, args.OutputEncoding);
        }

        [Test]
        public void TestThatThereIsNoEncodingByDefault()
        {
            var args = Arguments.Parse(new string[0]);
            Assert.IsFalse(args.HasOutputEncoding);
        }

        [Test]
        public void TestThatThereIsAnEncodingWhenSpecified()
        {
            var args = Arguments.Parse(new[] { "/e:utf-8" });
            Assert.IsTrue(args.HasOutputEncoding);
        }

        [Test]
        public void TestThatUnrecognizedEncodingThrows()
        {
            Assert.Catch<ArgumentException>(() => Arguments.Parse(new[] { "/e:adfasdf" }));
        }

    }
}
