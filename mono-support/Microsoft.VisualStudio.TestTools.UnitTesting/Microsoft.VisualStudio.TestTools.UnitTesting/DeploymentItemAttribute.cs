// Type: Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute
// Assembly: Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F644D728-C1F1-48EF-B944-A5D0A59DED82
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\ReferenceAssemblies\v4.0\Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll

using System;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class DeploymentItemAttribute : Attribute
    {
        private string m_path;
        private string m_outputDirectory;

        public string Path
        {
            get
            {
                return this.m_path;
            }
        }

        public string OutputDirectory
        {
            get
            {
                return this.m_outputDirectory;
            }
        }

        public DeploymentItemAttribute(string path)
        {
            this.m_path = path;
            this.m_outputDirectory = string.Empty;
        }

        public DeploymentItemAttribute(string path, string outputDirectory)
        {
            this.m_path = path;
            this.m_outputDirectory = outputDirectory;
        }
    }
}
