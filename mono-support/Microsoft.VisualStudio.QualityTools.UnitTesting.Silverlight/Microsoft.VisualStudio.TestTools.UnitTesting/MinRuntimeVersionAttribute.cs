/*
 * TestMethodAttribute.cs.
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
	[AttributeUsage (AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=false)]
	public class MinRuntimeVersionAttribute : Attribute
	{
		public MinRuntimeVersionAttribute (int version) {
			RuntimeVersion = version;
		}

		public int RuntimeVersion { get; private set; }
	}
}
