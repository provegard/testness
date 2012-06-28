// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using Mono.Cecil;

namespace TestNess.Lib.Reporting.XUnit
{
    public class TypeNode : XUnitNode
    {
        public TypeDefinition Type { get; set; }

        public override bool Equals(object obj)
        {
            var tn = obj as TypeNode;
            return tn != null && Equals(Type, tn.Type);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override void Accept(XUnitTreeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
