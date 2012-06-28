using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using TestNess.Lib.Analysis;

namespace TestNess.Lib.Reporting.XUnit
{
    public class XUnitHtmlReporter : IReporter
    {
        private readonly XmlWriter _writer;

        public XUnitHtmlReporter(XmlWriter writer)
        {
            _writer = writer;
        }

        public void GenerateReport(AnalysisResults results)
        {
            var xmlReporter = new XUnitReporter(null);
            var xml = xmlReporter.GenerateXml(results);

            var transform = new XslCompiledTransform();
            transform.Load(new XmlTextReader(new StringReader(Properties.Resources.XUnitHtml_xslt)));

            transform.Transform(xml.CreateNavigator(), null, _writer);
        }
    }
}
