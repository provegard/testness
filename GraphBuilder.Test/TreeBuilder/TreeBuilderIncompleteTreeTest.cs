using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
