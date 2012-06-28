// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using NUnit.Framework;

namespace GraphBuilder.Test.TreeBuilder
{
    [TestFixture]
    public class TreeBuilderIncompleteTreeTest
    {
        [Test]
        public void TestThatIncompleteTreeResultsInProperError()
        {
            var tb = new TreeBuilder<INode>();
            tb.Group<NameNode>().As(nn => new PartNode { Part = PartNode.Strip(nn.Text) });

            var nn1 = new NameNode { Text = "X.Y" };
            var nn2 = new NameNode { Text = "A.B" };

            Assert.Throws<ArgumentException>(() => tb.Build(new[] {nn1, nn2}));
        }
    }
}
