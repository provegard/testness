// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Collections.Generic;
using System.Linq;

namespace TestNess.Lib
{
    public static class EnumerableExtensions
    {
        public static bool EndsWith<T>(this IEnumerable<T> coll, IEnumerable<T> tail)
        {
            // Convert to lists, so we only enumerate once.
            var collList = coll.ToList();
            var tailList = tail.ToList();

            if (tailList.Count > collList.Count)
                return false;

            // Trim leading excess from the collection
            var lastCollItems = ((IEnumerable<T>)collList).Reverse().Take(tailList.Count).Reverse();
            var e1 = lastCollItems.GetEnumerator();
            var e2 = tailList.GetEnumerator();
            while (e1.MoveNext())
            {
                e2.MoveNext();
                if (!Equals(e1.Current, e2.Current))
                    return false;
            }
            return true;
        }
    }
}
