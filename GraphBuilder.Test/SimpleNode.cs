// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.Collections.Generic;

namespace GraphBuilder.Test
{
    public class SimpleNode
    {
        private readonly ICollection<SimpleNode> _children = new List<SimpleNode>();

        public string Value { get; private set; }

        public SimpleNode(string value)
        {
            Value = value;
        }

        public void AddChild(SimpleNode child)
        {
            _children.Add(child);
        }

        public IEnumerable<SimpleNode> GetChildren()
        {
            return _children;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(SimpleNode)) return false;
            var other = (SimpleNode)obj;
            return Equals(other.Value, Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
