// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using NUnit.Framework;
using TestNess.Lib.Rule;
using TestNess.Target;

namespace TestNess.Lib.Test.Rule
{
    [TestFixture]
    public class ViolationMethodLocationTest
    {
        private Violation _violation;

        [TestFixtureSetUp]
        public void GivenAViolationCreatedFromAMethod()
        {
            var tc = TestHelper.FindTestCase<IntegerCalculatorLocationTest>(t => t.TestAdd());
            _violation = new Violation(new ViolationTest.SomeRule(), tc);
        }

        [TestCase]
        public void ThenTheDocumentShouldBeExposed()
        {
            StringAssert.EndsWith("\\IntegerCalculatorLocationTest.cs", _violation.DocumentUrl);
        }

        [TestCase]
        public void ThenTheLocationShouldBeUnknown()
        {
            Assert.IsNull(_violation.Location);
        }

        [TestCase]
        public void ThenToStringShouldContainDocumentAndMethod()
        {
            const string expectedEnd = "\\IntegerCalculatorLocationTest.cs(TestAdd()): violation of \"some rule\"";
            StringAssert.EndsWith(expectedEnd, _violation.ToString());
        }
    }
}
