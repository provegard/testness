// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;
using Mono.Cecil;

namespace TestNess.Lib.TestFramework
{
    /// <summary>
    /// Defines the interface that TestNess uses to discover and inspect test cases implemented
    /// using a particular test framework.
    /// </summary>
    public interface ITestFramework
    {
        /// <summary>
        /// Determines if the method is a setup method, i.e. one that returns before all tests
        /// or before each test.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <returns>Whether or not the method is a setup method.</returns>
        bool IsSetupMethod(MethodDefinition method);

        bool IsTestMethod(MethodDefinition method);

        bool IsIgnoredTest(MethodDefinition method);

        bool HasExpectedException(MethodDefinition method);

        bool DoesContainAssertion(MethodDefinition method);

        /// <summary>
        /// Returns a list of parameter purposes corresponding to the parameters of the given method.
        /// If the method is not recognized, the return value is <c>null</c> (as opposed to the empty
        /// list, which is returned for a recognized method with no parameters).
        /// </summary>
        /// <param name="method">Supposedly an asserting method from this framework.</param>
        /// <returns>A list of parameter purposes.</returns>
        IList<ParameterPurpose> GetParameterPurposes(MethodReference method);

        /// <summary>
        /// Determines if the given method is a method that accesses data for a data-driven test.
        /// MSTest uses <c>TestContext.DataRow</c> rather than method parameters for publishing
        /// data from a data source. For this reason, we need a way to check if a method call really
        /// is a way to access data in a data-driven test.
        /// </summary>
        /// <param name="method">The method being called.</param>
        /// <returns>A boolean value indicating if the method is an accessor method for data in a
        /// data-driven test.</returns>
        bool IsDataAccessorMethod(MethodReference method);
    }

    public enum ParameterPurpose
    {
        /// <summary>
        /// The parameter represents the expected value.
        /// </summary>
        Expected,

        /// <summary>
        /// The parameter represents the actual value.
        /// </summary>
        Actual,

        /// <summary>
        /// The parameter can be either the expected or the actual value.
        /// </summary>
        ExpectedOrActual,

        /// <summary>
        /// We don't know what this parameter represents!
        /// </summary>
        Unknown,

        /// <summary>
        /// The parameter is a message, or a message parameter, or something else that has nothing
        /// to do with the outcome of the assertion.
        /// </summary>
        MetaData
    }
}
