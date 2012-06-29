// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Linq;
using NUnit.Framework;

namespace GraphBuilder.Test
{
    [TestFixture]
    public class TreeBuilderSingleStepTest
    {
        private Graph<INode> _result;

        [TestFixtureSetUp]
        public void GivenATreeBuiltWithNameGroupingOnly()
        {
            var tb = new TreeBuilder<INode>();
            tb.Group<NameNode>().As(nn => new PartNode { Part = PartNode.Strip(nn.Text) });

            var nn1 = new NameNode { Text = "X.Y.U" };
            var nn2 = new NameNode { Text = "X.Y.V" };

            _result = tb.Build(new[] { nn1, nn2 });
        }

        [Test]
        public void ThenTheRootOfTheGraphIsAPartNode()
        {
            Assert.IsInstanceOf<PartNode>(_result.Root);
        }

        [Test]
        public void ThenThePartNodeRootContainsTheCorrectNamePart()
        {
            Assert.AreEqual("X.Y", ((PartNode) _result.Root).Part);
        }

        [Test]
        public void ThenThePartNodeHasTwoChildren()
        {
            Assert.AreEqual(2, _result.HeadsFor(_result.Root).Count());
        }
    }
}
