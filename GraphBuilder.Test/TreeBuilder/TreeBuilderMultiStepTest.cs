// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.Linq;
using NUnit.Framework;

namespace GraphBuilder.Test
{
    [TestFixture(4, "X.Y.U", "X.Y.V")]
    [TestFixture(6, "X.Y.U", "X.Y.V.a", "X.Y.V.b")]
    public class TreeBuilderMultiStepTest
    {
        private Graph<INode> _result;
        private readonly string[] _names;
        private readonly int _expectedNodeCount;

        public TreeBuilderMultiStepTest(int nc, params string[] names)
        {
            _expectedNodeCount = nc;
            _names = names;
        }

        [TestFixtureSetUp]
        public void GivenATreeBuiltWithMultipleSteps()
        {
            var tb = new TreeBuilder<INode>();
            tb.Group<NameNode>().As(nn => new PartNode { Part = PartNode.Strip(nn.Text) });
            tb.Group<PartNode>()
                .When(pn => pn.Part.Contains("."))
                .As(pn => new PartNode { Part = PartNode.Strip(pn.Part) });

            _result = tb.Build(_names.Select(name => new NameNode { Text = name }));
        }

        [Test]
        public void ThenTheRootOfTheGraphIsAPartNode()
        {
            Assert.IsInstanceOf<PartNode>(_result.Root);
        }

        [Test]
        public void ThenThePartNodeRootContainsTheCorrectNamePart()
        {
            Assert.AreEqual("X", ((PartNode)_result.Root).Part);
        }

        [Test]
        public void ThenThePartNodeRootContainsTheCorrectNumberOfNodes()
        {
            Assert.AreEqual(_expectedNodeCount, _result.Order);
        }
    }
}
