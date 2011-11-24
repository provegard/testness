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
using Mono.Cecil;

namespace TestNess.Lib
{
    /// <summary>
    /// Defines the interface that TestNess uses to discover and inspect test cases implemented
    /// using a particular test framework.
    /// </summary>
    public interface ITestFramework
    {
        bool IsTestMethod(MethodDefinition method);

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
