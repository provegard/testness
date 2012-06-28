// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using GraphBuilder;
using TestNess.Lib.Analysis;

namespace TestNess.Lib.Reporting.XUnit
{
    public class XUnitReporter : XmlReporter
    {
        public XUnitReporter(XmlWriter writer)
            : base(writer)
        {
        }

        public override XDocument GenerateXml(AnalysisResults results)
        {
            var gen = new XUnitXmlGenerator(results);
            var tree = BuildTree(results);
            gen.Traverse(tree);
            var elem = gen.GetRootElement();
            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), elem);
        }

        private Graph<XUnitNode> BuildTree(AnalysisResults results)
        {
            //TODO: handle the empty list case!
            var leafNodes = results.Applications.Select(app => new TestCaseNode { Application = app }).ToList();
            var ass = leafNodes.First().Application.TestCase.TestMethod.DeclaringType.Module.Assembly;
            return TreeBuilder<XUnitNode>.Create()
                .Group<TestCaseNode>()
                .As(tcn => new TypeNode {Type = tcn.Application.TestCase.TestMethod.DeclaringType})
                .Group<TypeNode>()
                .As(tyn => new NamespaceNode {Namespace = tyn.Type.Namespace})
                .Group<NamespaceNode>()
                .When(nn => nn.Namespace.Contains("."))
                .As(nn => new NamespaceNode {Namespace = nn.ParentNamespace()})
                .Group<NamespaceNode>()
                .When(nn => !nn.Namespace.Contains("."))
                .As(nn => new AssemblyNode {Assembly = ass})
                .Build(leafNodes);
        }

        private class XUnitXmlGenerator : XUnitTreeVisitor
        {
            private readonly IList<Result> _intermediate = new List<Result>();
            private readonly AnalysisResults _ar;

            public XUnitXmlGenerator(AnalysisResults results)
            {
                _ar = results;
            }

            internal XElement GetRootElement()
            {
                //TODO: some validation here
                return _intermediate.First().Element;
            }

            public override void Visit(AssemblyNode node)
            {
                var now = _ar.AnalysisTime;
                var ass = _ar.Applications.First().TestCase.TestMethod.DeclaringType.Module.Assembly;

                base.Visit(node);

                var elem = new XElement("assembly",
                                        new XAttribute("name", ass.Name.Name),
                                        new XAttribute("run-date", now.ToString("yyyy-MM-dd")),
                                        new XAttribute("run-time", now.ToString("HH:mm:ss")),
                                        new XAttribute("configFile", ""),
                                        new XAttribute("environment", string.Format("{0}-bit .NET {1}",
                                            Environment.Is64BitOperatingSystem ? 64 : 32, Environment.Version)),
                                        new XAttribute("test-framework", "TestNess"));
                var result = AggregateResults(elem);

                _intermediate.Add(result);
            }

            public override void Visit(TypeNode node)
            {
                base.Visit(node);

                var elem = new XElement("class", 
                    new XAttribute("name", node.Type.FullName));
                var result = AggregateResults(elem);

                _intermediate.Add(result);
            }

            private Result AggregateResults(XElement element)
            {
                int passed = 0, failed = 0;
                var elapsed = 0L;
                foreach (var result in _intermediate.Where(r => r.Level > Level).ToList())
                {
                    element.Add(result.Element);
                    passed += result.PassCount;
                    elapsed += result.Elapsed;
                    failed += result.FailCount;
                    _intermediate.Remove(result);
                }
                var total = passed + failed;

                // Add aggregation attributes
                element.Add(new XAttribute("skipped", 0),
                            new XAttribute("failed", failed),
                            new XAttribute("passed", passed),
                            new XAttribute("total", total),
                            new XAttribute("time", ToTimeString(elapsed)));

                return new Result { Elapsed = elapsed, Element = element, PassCount = passed, FailCount = total - passed, Level = Level };
            }

            public override void Visit(TestCaseNode node)
            {
                var app = node.Application;
                var violations = app.Violations.ToList();
                var elapsed = app.ElapsedTimeInMilliseconds;
                var testCase = app.TestCase;
                var rule = app.Rule;

                var displayName = string.Format("{0} [{1}]", GetTestCaseName(testCase), rule);
                var pass = violations.Count == 0;

                var elem = new XElement("test",
                                        new XAttribute("name", displayName),
                                        new XAttribute("type", testCase.TestMethod.DeclaringType.FullName),
                                        new XAttribute("method", testCase.TestMethod.Name),
                                        new XAttribute("result", pass ? "Pass" : "Fail"),
                                        new XAttribute("time", ToTimeString(elapsed)));
                if (!pass)
                {
                    // Construct a single message from all violations
                    var sb = new StringBuilder();
                    foreach (var v in violations)
                    {
                        sb.AppendLine(v.Message);
                    }
                    var felem = new XElement("failure",
                                             new XAttribute("exception-type", "System.Exception"),
                                             new XElement("message", new XText(sb.ToString())));
                    elem.Add(felem);
                }

                var pc = pass ? 1 : 0;
                _intermediate.Add(new Result { Element = elem, PassCount = pc, FailCount = 1 - pc, Elapsed = elapsed, Level = Level });
            }

            private string ToTimeString(long elapsed)
            {
                var seconds = elapsed/1000d;
                return seconds.ToString("0.000", CultureInfo.InvariantCulture);
            }

            private string GetTestCaseName(TestCase testCase)
            {
                var name = testCase.Name;
                name = name.Replace("::", ".");
                name = name.Replace("()", "");
                return name;
            }

            private class Result
            {
                public XElement Element { get; set; }
                public int PassCount { get; set; }
                public int FailCount { get; set; }
                public long Elapsed { get; set; }
                public int Level { get; set; }
            }
        }
    }
}
