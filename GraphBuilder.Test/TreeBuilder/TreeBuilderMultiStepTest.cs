using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
