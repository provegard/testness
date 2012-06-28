using System.Xml;
using System.Xml.Linq;
using TestNess.Lib.Analysis;

namespace TestNess.Lib.Reporting
{
    public abstract class XmlReporter : IReporter
    {
        private readonly XmlWriter _writer;

        protected XmlReporter(XmlWriter writer)
        {
            _writer = writer;
        }

        public void GenerateReport(AnalysisResults results)
        {
            var xml = GenerateXml(results);
            xml.WriteTo(_writer);
            _writer.Flush();
        }

        public abstract XDocument GenerateXml(AnalysisResults results);
    }
}
