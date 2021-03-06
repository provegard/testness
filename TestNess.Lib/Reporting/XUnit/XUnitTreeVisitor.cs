﻿// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using GraphBuilder;

namespace TestNess.Lib.Reporting.XUnit
{
    public class XUnitTreeVisitor
    {
        private Graph<XUnitNode> _tree;
        protected int Level;

        public void Traverse(Graph<XUnitNode> tree)
        {
            _tree = tree;
            Level = 0;
            tree.Root.Accept(this);
        }

        public virtual void Visit(TestCaseNode node)
        {
            VisitHeads(node);
        }

        public virtual void Visit(TypeNode node)
        {
            VisitHeads(node);
        }

        public virtual void Visit(NamespaceNode node)
        {
            VisitHeads(node);
        }

        public virtual void Visit(AssemblyNode node)
        {
            VisitHeads(node);
        }

        private void VisitHeads(XUnitNode node)
        {
            Level++;
            foreach (var head in _tree.HeadsFor(node))
            {
                head.Accept(this);
            }
            Level--;
        }
    }
}
