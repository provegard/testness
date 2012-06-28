/**
 * Copyright (C) 2011-2012 by Per Rovegård (per@rovegard.se)
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphBuilder
{
    public class TreeBuilder<T> where T : class
    {
        private readonly IList<T> _headless = new List<T>();

        public static TreeBuilder<T> Create()
        {
            return new TreeBuilder<T>();
        }

        public Grp<T1> Group<T1>() where T1 : T
        {
            return new Grp<T1>(this);
        }

        public class Grp<T1> where T1 : T
        {
            private readonly TreeBuilder<T> _builder;
            private Func<T1, bool> _condition;

            internal Grp(TreeBuilder<T> builder)
            {
                _builder = builder;
            }

            public TreeBuilder<T> As<T2>(Func<T1, T2> func) where T2 : T
            {
                _builder.AddGrouping(typeof (T1), typeof (T2), func, _condition);
                return _builder;
            }

            public Grp<T1> When(Func<T1, bool> condition)
            {
                _condition = condition;
                return this;
            }
        }

        private readonly IDictionary<Type, IList<Grouping>> _groupings = new Dictionary<Type, IList<Grouping>>();

        internal void AddGrouping<T1, T2>(Type src, Type dst, Func<T1, T2> func, Func<T1, bool> condition)
        {
            var grouping = new Grouping {SourceType = src, DestType = dst, ConversionFunc = func, Condition = condition};
            IList<Grouping> groupings;
            if (!_groupings.TryGetValue(src, out groupings))
            {
                groupings = new List<Grouping>();
                _groupings.Add(src, groupings);
            }
            groupings.Add(grouping);
        }

        public Graph<T> Build(IEnumerable<T> initialList)
        {
            _headless.Clear();

            var gb = new GraphBuilder<T>(HeadFinder);
            var tempGraph = gb.Build(initialList);

            // There should be only one headless...
            if (_headless.Count != 1)
            {
                throw new ArgumentException("The list of leaf nodes and the set of groupings do not together result in a well-formed tree.");
            }
            
            // Reverse the graph!
            gb = new GraphBuilder<T>(tempGraph.TailsFor);
            return gb.Build(_headless.Single());
        }

        private IEnumerable<T> HeadFinder(T node)
        {
            var realType = node.GetType();
            IList<Grouping> grps;
            if (!_groupings.TryGetValue(realType, out grps))
            {
                _headless.Add(node);
                yield break;
            }

            // See which conditional grouping (if any) that applies!
            var grp = grps.FirstOrDefault(g => g.Condition != null && Applies(g.Condition, node)) ??
                      grps.FirstOrDefault(g => g.Condition == null);
            if (grp == null)
            {
                _headless.Add(node);
                yield break;
            }
            var result = (T)grp.ConversionFunc.DynamicInvoke(node);
            yield return result;
        }

        private bool Applies(Delegate condition, T node)
        {
            return (bool) condition.DynamicInvoke(node);
        }

        private class Grouping
        {
            internal Type SourceType { get; set; }
            internal Type DestType { get; set; }
            internal Delegate ConversionFunc { get; set; }
            internal Delegate Condition { get; set; }
        }
    }

}
