// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.IO;
using System.Xml;
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
