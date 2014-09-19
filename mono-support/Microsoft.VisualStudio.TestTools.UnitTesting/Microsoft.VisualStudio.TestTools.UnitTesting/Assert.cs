/*
 * Assert.cs.
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
	public static class Assert
	{

		public static void IsNull (object obj)
		{
			if (obj != null)
				throw new AssertFailedException ();
		}
		public static void IsNull (object obj, string message)
		{
			if (obj != null)
				throw new AssertFailedException (message);
		}
		public static void IsNull (object obj, string message, params object [] parameters) {
			if (obj != null)
				throw new AssertFailedException (string.Format (message, parameters));
		}

		public static void IsNotNull (object obj)
		{
			if (obj == null)
				throw new AssertFailedException ();
		}

		public static void IsNotNull (object obj, string message)
		{
			if (obj == null)
				throw new AssertFailedException (message);
		}

		public static void IsNotNull (object obj, string message, params object [] parameters)
		{
			if (obj == null)
				throw new AssertFailedException (string.Format (message, parameters));
		}


		public static void IsBetween (double min, double max, double actual)
		{
			if (actual > max || actual < min)
				throw new AssertFailedException (string.Format ("Actual value '{0}' is not between '{1}' and '{2}'). ", actual, min, max));
		}

		public static void IsBetween (double min, double max, double actual, string message)
		{
			if (actual > max || actual < min)
				throw new AssertFailedException (string.Format ("Actual value '{0}' is not between '{1}' and '{2}'). " + message, actual, min, max));
		}

		public static void IsBetween (double min, double max, double actual, string message, params object [ ] parameters)
		{
			if (actual > max || actual < min)
				throw new AssertFailedException (string.Format ("Actual value '{0}' is not between '{1}' and '{2}'). {3}", actual, min, max, string.Format (message, parameters)));
		}

		public static void IsFalse (bool condition)
		{
			if (condition)
				throw new AssertFailedException ();
		}

		public static void IsFalse (bool condition, string message)
		{
			if (condition)
				throw new AssertFailedException (message);
		}

		public static void IsFalse (bool condition, string message, params object [] parameters)
		{
			if (condition)
				throw new AssertFailedException (string.Format (message, parameters));
		}

		public static void IsGreater (double expected, double actual)
		{
			if (expected >= actual) {
				string msg = String.Format ("Actual value '{0}' is not greater than expected value '{1}'.", actual, expected);
				throw new AssertFailedException (msg);
			}
		}

		public static void IsGreater (double expected, double actual, string message)
		{
			if (expected >= actual) {
				string msg = String.Format ("Actual value '{0}' is not greater than expected value '{1}'. {2}", actual, expected, message);
				throw new AssertFailedException (msg);
			}
		}

		public static void IsGreater (double expected, double actual, string message, params object [ ] parameters)
		{
			if (expected >= actual) {
				string msg = String.Format ("Actual value '{0}' is not greater than expected value '{1}'.", actual, expected, string.Format (message, parameters));
				throw new AssertFailedException (msg);
			}
		}

		public static void IsLess (double expected, double actual)
		{
			if (expected <= actual) {
				string msg = String.Format ("Actual value '{0}' is not less than expected value '{1}'.", actual, expected);
				throw new AssertFailedException (msg);
			}
		}

		public static void IsLess (double expected, double actual, string message)
		{
			if (expected <= actual) {
				string msg = String.Format ("Actual value '{0}' is not less than expected value '{1}'. {2}", actual, expected, message);
				throw new AssertFailedException (msg);
			}
		}

		public static void IsLess (int expected, int actual, string message, params object [ ] parameters)
		{
			if (expected <= actual) {
				string msg = String.Format ("Actual value '{0}' is not less than expected value '{1}'.", actual, expected, string.Format (message, parameters));
				throw new AssertFailedException (msg);
			}
		}

		public static void IsTrue (bool condition)
		{
			if (!condition)
				throw new AssertFailedException ();
		}

		public static void IsTrue (bool condition, string message)
		{
			if (!condition)
				throw new AssertFailedException (message);
		}

		public static void IsTrue (bool condition, string message, params object [] parameters)
		{
			if (!condition)
				throw new AssertFailedException (string.Format (message, parameters));
		}

		public static void AreSame (object expected, object actual)
		{
			if (expected != actual)
				throw new AssertFailedException ();
		}
		public static void AreSame (object expected, object actual, string message)
		{
			if (expected != actual)
				throw new AssertFailedException (message);
		}
		public static void AreSame (object expected, object actual, string message, params object [] parameters)
		{
			if (expected != actual)
				throw new AssertFailedException (string.Format (message, parameters));
		}

		public static void AreEqual (object expected, object actual)
		{
			AreEqual (expected, actual, null);
		}

		public static void AreEqual (object expected, object actual, string message)
		{
			if (!object.Equals (expected, actual)) {
				string msg = String.Format ("Actual value is {0} while the expected value was {1}. {2}",
							    actual == null ? "<null>" : string.Format ("'{0}'", actual),
							    expected == null ? "<null>" : string.Format ("'{0}'", expected), message);
				throw new AssertFailedException (msg);
			}
		}

		public static void AreEqual (object expected, object actual, string message, params object [] parameters)
		{
			AreEqual (expected, actual, string.Format (message, parameters));

		}

		public static void AreEqual (int expected, int actual)
		{
			if (expected != actual) {
				string msg = String.Format ("Actual value is '{0}' while the expected value was '{1}'.", actual, expected);
				throw new AssertFailedException (msg);
			}
		}

		public static void AreEqual (int expected, int actual, string message)
		{
			if (expected != actual) {
				string msg = String.Format ("Actual value is '{0}' while the expected value was '{1}'. {2}", actual, expected, message);
				throw new AssertFailedException (msg);
			}
		}

		public static void AreEqual (int expected, int actual, string message, params object [] parameters)
		{
			if (expected != actual) {
				string msg = String.Format ("Actual value is '{0}' while the expected value was '{1}'. {2}", actual, expected, string.Format (message, parameters));
				throw new AssertFailedException (msg);
			}
		}

		public static void AreEqual (double expected, double actual)
		{
			if (expected != actual) {
				string msg = String.Format ("Actual value is '{0}' while the expected value was '{1}'.", actual, expected);
				throw new AssertFailedException (msg);
			}
		}
		public static void AreEqual (double expected, double actual, string message)
		{
			if (expected != actual) {
				string msg = String.Format ("Actual value is '{0}' while the expected value was '{1}'. {2}", actual, expected, message);
				throw new AssertFailedException (msg);
			}

		}
		public static void AreEqual (double expected, double actual, string message, params object [] parameters)
		{
			if (expected != actual) {
				string msg = String.Format ("Actual value is '{0}' while the expected value was '{1}' {2}.", actual, expected, string.Format (message, parameters));
				throw new AssertFailedException (msg);
			}

		}

		public static void AreNotEqual (object expected, object actual)
		{
			AreNotEqual (expected, actual, null);
		}

		public static void AreNotEqual (object expected, object actual, string message)
		{
			if (object.Equals (expected, actual)) {
				string msg = String.Format ("Expected value and actual value were both the same. {0}", message);
				throw new AssertFailedException (msg);
			}
		}

		public static void AreNotEqual (object expected, object actual, string message, params object [] parameters)
		{
			AreNotEqual (expected, actual, string.Format (message, parameters));

		}

		public static void AreNotEqual (int expected, int actual)
		{
			if (expected == actual) {
				string msg = String.Format ("Expected value and actual value were both the same");
				throw new AssertFailedException (msg);
			}
		}

		public static void AreNotEqual (int expected, int actual, string message)
		{
			if (expected == actual) {
				string msg = String.Format ("Expected value and actual value were both the same. {0}", message);
				throw new AssertFailedException (msg);
			}
		}

		public static void AreNotEqual (int expected, int actual, string message, params object [] parameters)
		{
			if (expected == actual) {
				string msg = String.Format ("Expected value and actual value were both the same. {0}", string.Format (message, parameters));
				throw new AssertFailedException (msg);
			}
		}

		public static void AreNotEqual (double expected, double actual)
		{
			if (expected == actual) {
				string msg = String.Format ("Expected value and actual value were both the same");
				throw new AssertFailedException (msg);
			}
		}
		public static void AreNotEqual (double expected, double actual, string message)
		{
			if (expected == actual) {
				string msg = String.Format ("Expected value and actual value were both the same. {0}", message);
				throw new AssertFailedException (msg);
			}

		}
		public static void AreNotEqual (double expected, double actual, string message, params object [] parameters)
		{
			if (expected == actual) {
				string msg = String.Format ("Expected value and actual value were both the same. {0}", string.Format (message, parameters));
				throw new AssertFailedException (msg);
			}
		}

		public static void AreNotSame (object expected, object actual)
		{
			if (expected == actual) {
				string msg = String.Format ("Expected value and actual value were both the same");
				throw new AssertFailedException (msg);
			}
		}
		
		public static void AreNotSame (object expected, object actual, string message)
		{
			if (expected == actual) {
				string msg = String.Format ("Expected value and actual value were both the same. {0}", message);
				throw new AssertFailedException (msg);
			}
		}
		
		public static void AreNotSame (object expected, object actual, string message, params object [] parameters)
		{
			if (expected == actual) {
				string msg = String.Format ("Expected value and actual value were both the same. {0}", string.Format (message, parameters));
				throw new AssertFailedException (msg);
			}
		}

		public static void DoesNotThrow (TestCode code, string message, params object [] parameters)
		{
			try {
				code ();
			} catch (Exception ex) {
				Assert.Fail ("Expected no exception but got: {0}. {1}", ex.GetType ().Name, string.Format (message, parameters));
			}
		}

		public static void IsInstanceOfType<T> (object value)
		{
			IsInstanceOfType (value, typeof (T));
		}
		
		public static void IsInstanceOfType (object value, Type expectedType)
		{
			IsInstanceOfType (value, expectedType, null);
		}

		public static void IsInstanceOfType<T> (object value, string message)
		{
			IsInstanceOfType (value, typeof (T), message);
		}

		public static void IsInstanceOfType (object value, Type expectedType, string message)
		{
			message = string.Format ("Expected '{0}' but was '{1}'. {2}", expectedType.Name, value == null ? "<null>" : value.GetType ().Name, message);
			if (value == null)
				throw new AssertFailedException (message);
			else if (!expectedType.IsAssignableFrom (value.GetType ()))
				throw new AssertFailedException (message);
		}

		public static void IsInstanceOfType (object value, Type expectedType, string message, params object [] parameters)
		{
			IsInstanceOfType (value, expectedType, string.Format (message, parameters));
		}

		public static void IsNotInstanceOfType<T> (object value)
		{
			IsNotInstanceOfType (value, typeof (T));
		}

		public static void IsNotInstanceOfType (object value, Type expectedType)
		{
			IsNotInstanceOfType (value, expectedType, null);
		}

		public static void IsNotInstanceOfType<T> (object value, string message)
		{
			IsNotInstanceOfType (value, typeof (T), message);
		}

		public static void IsNotInstanceOfType (object value, Type expectedType, string message)
		{
			message = string.Format ("Item {1} should not be instance of '{0}'", expectedType.Name, value == null ? "<null>" : value.GetType ().Name, message);
			if (value == null)
				throw new AssertFailedException (message);
			else if (value.GetType () == expectedType || value.GetType ().IsSubclassOf (expectedType))
				throw new AssertFailedException (message);
		}

		public static void IsNotInstanceOfType (object value, Type expectedType, string message, params object [] parameters)
		{
			IsNotInstanceOfType (value, expectedType, string.Format (message, parameters));
		}

		public static void Fail ()
		{
			throw new AssertFailedException (string.Empty);
		}

		public static void Fail (string message)
		{
			throw new AssertFailedException (message);
		}

		public static void Fail (string message, params object [] parameters)
		{
			throw new AssertFailedException (string.Format (message, parameters));
		}
	}
}
