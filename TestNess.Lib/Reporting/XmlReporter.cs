// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
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
