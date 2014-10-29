using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace TestNess.Target
{
    [TestClass]
    public class MixedMockAndTestFrameworksTest
    {
        [TestMethod]
        public void TestMix()
        {
            var foo = Substitute.For<IFoo>();
            var baz = new Baz();
            baz.Do(foo);
            foo.Received().Bar();
        }

        public interface IFoo
        {
            void Bar();
        }

        public class Baz
        {
            public void Do(IFoo foo)
            {
                foo.Bar();
            }
        }
    }
}
