// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System.IO;
using System.Xml;
using System.Xml.Linq;
using TestNess.Lib.Analysis;

namespace TestNess.Lib.Reporting
{
    public abstract class XmlReporter : IReporter
    {
        public void GenerateReport(IReportReceiver receiver, AnalysisResults results)
        {
            var writer = new StringWriter();
            var xml = GenerateXml(results);
            var xwriter = XmlWriter.Create(writer);
            xml.WriteTo(xwriter);
            xwriter.Flush();
            receiver.GenerateReport(writer.ToString());
        }

        public abstract XDocument GenerateXml(AnalysisResults results);
    }
}
