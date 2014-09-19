/*
 * InternalTestFailureException.cs.
 *
 * Contact:
 *   Moonlight List (moonlight-list@lists.ximian.com)
 *
 * Copyright 2008 Novell, Inc. (http://www.novell.com)
 *
 * See the LICENSE file included with the distribution for details.
 * 
 */

using System;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
	public class InternalTestFailureException : UnitTestAssertException	{
		public InternalTestFailureException ()
		{
		}

		public InternalTestFailureException (string message) : base (message)
		{
		}

		public InternalTestFailureException (string message, Exception inner) : base (message, inner)
		{
		}
	}
}