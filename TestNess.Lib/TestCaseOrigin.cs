using Mono.Cecil;

namespace TestNess.Lib
{
    public class TestCaseOrigin
    {
        public TestCaseOrigin(AssemblyDefinition assembly, string fileName)
        {
            Assembly = assembly;
            AssemblyFileName = fileName;
        }

        public AssemblyDefinition Assembly { get; private set; }
        public string AssemblyFileName { get; private set; }
    }
}