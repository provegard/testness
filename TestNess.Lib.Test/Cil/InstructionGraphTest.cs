using System;
using System.Linq;
using NUnit.Framework;
using TestNess.Lib.Cil;

namespace TestNess.Lib.Test.Cil
{
    [TestFixture]
    public class InstructionGraphTest
    {
        private static readonly Func<int> _sut = () => 1 + 2;
        private static readonly Func<int> _branchedSut = () => _sut != null ? 1 : 2;

        private static readonly Action _looper = () =>
        {
            for (var i = 0; i < 3; i++)
            {
                Console.WriteLine(i);
            }
        };

        [Test]
        public void ItShouldBePossibleToCreateFromMethod()
        {
            var method = _sut.AsMethodDef();
            var igraph = InstructionGraph.CreateFrom(method);
            Assert.AreEqual(5, igraph.Order);
        }

        [Test]
        public void ItShouldHaveASinglePathWhenThereIsNoBranching()
        {
            var method = _sut.AsMethodDef();
            var igraph = InstructionGraph.CreateFrom(method);
            var paths = igraph.FindInstructionPaths();
            Assert.AreEqual(1, paths.Count());
        }

        [Test]
        public void ItShouldHaveTwoPathsWhenThereIsABranchInstruction()
        {
            var method = _branchedSut.AsMethodDef();
            var igraph = InstructionGraph.CreateFrom(method);
            var paths = igraph.FindInstructionPaths();
            Assert.AreEqual(2, paths.Count());
        }

        [Test]
        public void ItShouldHaveTwoPathsWhenThereIsALoop()
        {
            var method = _looper.AsMethodDef();
            var igraph = InstructionGraph.CreateFrom(method);
            var paths = igraph.FindInstructionPaths();
            Assert.AreEqual(2, paths.Count());
        }

        [Test]
        public void ItShouldDetectALoopingPath()
        {
            var method = _looper.AsMethodDef();
            var igraph = InstructionGraph.CreateFrom(method);
            var paths = igraph.FindInstructionPaths();
            Assert.True(paths.Any(p => p.ContainsLoop()));
        }

        [Test]
        public void ItShouldNotReportANonLoopingPathAsLooping()
        {
            var method = _sut.AsMethodDef();
            var igraph = InstructionGraph.CreateFrom(method);
            var paths = igraph.FindInstructionPaths();
            Assert.False(paths.Any(p => p.ContainsLoop()));
        }
    }
}
