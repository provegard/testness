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
        public void GenerateReport(TextWriter writer, AnalysisResults results)
        {
            var xmlReporter = new XUnitReporter();
            var xml = xmlReporter.GenerateXml(results);

            var transform = new XslCompiledTransform();
            transform.Load(new XmlTextReader(new StringReader(Properties.Resources.XUnitHtml_xslt)));

            var xwriter = XmlWriter.Create(writer);
            transform.Transform(xml.CreateNavigator(), null, xwriter);
            xwriter.Flush();
        }
    }
}
