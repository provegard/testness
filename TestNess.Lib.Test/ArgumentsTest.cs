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
            var args = Arguments.Parse(new[] { "-c", "testness.cfg", "anassembly.dll" });
            Assert.AreEqual("anassembly.dll", args.AssemblyFileName);
        }

        [TestCase]
        public void TestThatConfigFileIsReadFromOption()
        {
            var args = Arguments.Parse(new[] { "-c", "testness.cfg", "anassembly.dll" });
            Assert.AreEqual("testness.cfg", args.ConfigurationFileName);
        }

        [TestCase]
        public void TestThatPreseceOfConfigFileCanBeChecked()
        {
            var args = Arguments.Parse(new[] { "-c", "testness.cfg", "anassembly.dll" });
            Assert.IsTrue(args.HasConfigurationFileName);
        }

        [TestCase, ExpectedException(typeof(ArgumentException))]
        public void TestThatUnrecognizedOptionThrows()
        {
            Arguments.Parse(new[] { "-x" }); // should throw
        }

        [TestCase, ExpectedException(typeof(ArgumentException))]
        public void TestThatMissingOptionValueThrows()
        {
            Arguments.Parse(new[] { "-c" }); // should throw
        }
    }
}
