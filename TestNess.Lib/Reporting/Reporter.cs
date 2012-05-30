/**
 * Copyright (C) 2011-2012 by Per Rovegård (per@rovegard.se)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GraphBuilder;
using TestNess.Lib.Analysis;
using TestNess.Lib.Analysis.Node;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Reporting
{
    public class Reporter
    {
        private readonly Graph<AnalysisNode> _graph;
        private readonly IViolationScorer _scorer;

        public Reporter(Graph<AnalysisNode> analysisGraph, IViolationScorer scorer)
        {
            _graph = analysisGraph;
            _scorer = scorer;
        }

        public XElement Generate()
        {
            //var doc = new XDocument(
            //    new XDeclaration("1.0", "UTF-8", "yes"));

            var generator = new XUnitXmlGenerator();
            generator.Traverse(_graph);
            //doc.Add(generator.GetRootElement());
            //return doc;
            return generator.GetRootElement();
        }
        
        private class XUnitXmlGenerator : AnalysisTreeVisitor
        {
            private readonly IList<Result> _results = new List<Result>(); 

            internal XElement GetRootElement()
            {
                //TODO: some validation here
                return _results.First().Element;
            }

            public override void Visit(AssemblyNode node)
            {
                var now = DateTime.Now;

                base.Visit(node);

                var elem = new XElement("assembly",
                                    new XAttribute("name", "TODO"), //TODO: assembly path
                                    new XAttribute("run-date", now.ToString("yyyy-MM-dd")),
                                    new XAttribute("run-time", now.ToString("HH:mm:ss")),
                                    new XAttribute("configFile", "TODO"), //TODO:???
                                    new XAttribute("environment", string.Format("{0}-bit .NET {1}", 
                                        Environment.Is64BitOperatingSystem ? 64 : 32, Environment.Version)),
                                    new XAttribute("test-framework", "TestNess")); //TODO: version
                var result = AggregateResults(elem);

                _results.Clear();
                _results.Add(result);
            }

            public override void Visit(TypeNode node)
            {
                base.Visit(node);

                var elem = new XElement("class",
                    new XAttribute("name", node.Type.FullName));
                var result = AggregateResults(elem);

                _results.Clear();
                _results.Add(result);
            }

            private Result AggregateResults(XElement element)
            {
                int passed = 0, failed = 0;
                var elapsed = 0d;
                foreach (var result in _results)
                {
                    element.Add(result.Element);
                    passed += result.PassCount;
                    elapsed += result.Elapsed;
                    failed += result.FailCount;
                }
                var total = passed + failed;

                // Add aggregation attributes
                element.Add(new XAttribute("time", elapsed.ToString("0.000", CultureInfo.InvariantCulture)),
                    new XAttribute("total", total),
                    new XAttribute("passed", passed),
                    new XAttribute("failed", failed),
                    new XAttribute("skipped", 0));

                return new Result { Elapsed = elapsed, Element = element, PassCount = passed, FailCount = total - passed };
            }

            public override void Visit(TestCaseNode node)
            {
                var rule = node.Rule;
                var testCase = node.TestCase;

                var sw = Stopwatch.StartNew();
                var violations = rule.Apply(testCase).ToList();
                sw.Stop();
                var elapsed = sw.ElapsedMilliseconds/1000d;

                var displayName = string.Format("{0} [{1}]", testCase.Name, rule);
                var pass = violations.Count == 0;

                var elem = new XElement("test",
                                        new XAttribute("name", displayName),
                                        new XAttribute("type", testCase.TestMethod.DeclaringType.FullName),
                                        new XAttribute("method", testCase.TestMethod.Name),
                                        new XAttribute("result", pass ? "Pass" : "Fail"),
                                        new XAttribute("time", elapsed.ToString("0.000", CultureInfo.InvariantCulture)));
                if (!pass)
                {
                    // Construct a single message from all violations
                    var sb = new StringBuilder();
                    foreach (var v in violations)
                    {
                        sb.AppendLine(ViolationToString(v));
                    }
                    //TODO: use some custom type for exceptio-type
                    var felem = new XElement("failure",
                                             new XAttribute("exception-type", "System.Exception"),
                                             new XElement("message", new XText(sb.ToString())));
                    elem.Add(felem);
                }

                var pc = pass ? 1 : 0;
                _results.Add(new Result {Element = elem, PassCount = pc, FailCount = 1 - pc, Elapsed = elapsed});
            }

            private string ViolationToString(Violation v)
            {
                return v.ToString();
                //var sb = new StringBuilder();
                //if (v.DocumentUrl != null)
                //{
                //    sb.Append(v.DocumentUrl);
                //    if (v.Location != null)
                //        sb.AppendFormat("({0},{1})", v.Location.StartLine, v.Location.StartColumn);
                //    sb.Append((": "));
                //}
                //sb.Append(v.Message);
                //return sb.ToString();
            }

            private class Result
            {
                public XElement Element { get; set; }
                public int PassCount { get; set; }
                public int FailCount { get; set; }
                public double Elapsed { get; set; }
            }
        }
    }
}
