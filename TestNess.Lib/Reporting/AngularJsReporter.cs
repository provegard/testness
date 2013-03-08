// Copyright (C) 2011-2013 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TestNess.Lib.Analysis;
using TestNess.Lib.Properties;
using TestNess.Lib.Rule;

namespace TestNess.Lib.Reporting
{
    public class AngularJsReporter : IReporter
    {
        public void GenerateReport(IReportReceiver recv, AnalysisResults results)
        {
            var text = Resources.angular_report;
            var json = JsonConvert.SerializeObject(new Report(results));
            text = text.Replace("@@ANALYSIS_RESULTS@@", json);
            recv.GenerateReport(text);
        }

        private class Report
        {
            internal Report(AnalysisResults results)
            {
                TotalTimeMs = 0;
                TotalScore = 0m;
                Applications = new List<ReportApplication>();
                TestCase tc = null;
                foreach (var a in results.Applications)
                {
                    Applications.Add(new ReportApplication(a));
                    TotalScore += a.Score;
                    TotalTimeMs += a.ElapsedTimeInMilliseconds;
                    if (tc == null)
                    {
                        tc = a.TestCase;
                    }
                }
                Target = tc != null ? tc.Origin.Assembly.Name.Name : "Unknown";
                When = results.AnalysisTime;
            }

            public string Target { get; private set; }
            public decimal TotalScore { get; private set; }
            public long TotalTimeMs { get; private set; }
            public DateTime When { get; private set; }
            public IList<ReportApplication> Applications { get; private set; }
        }

        private class ReportApplication
        {
            public ReportApplication(TestCaseRuleApplication app)
            {
                RuleName = app.Rule.ToString();
                TimeMs = app.ElapsedTimeInMilliseconds;
                TestCaseName = app.TestCase.Name;
                Violations = app.Violations.Select(v => new ReportViolation(v)).ToList();
                Score = app.Score;
            }

            public decimal Score { get; private set; }
            public long TimeMs { get; private set; }
            public string RuleName { get; private set; }
            public string TestCaseName { get; private set; }
            public IList<ReportViolation> Violations { get; private set; }
        }

        private class ReportViolation
        {
            public ReportViolation(Violation violation)
            {
                Where = violation.DocumentUrl;
                Location = violation.Location;
                Message = violation.Message;
            }

            public string Message { get; private set; }
            public string Where { get; private set; }
            public Violation.Coordinates Location { get; private set; }
        }
    }

}
