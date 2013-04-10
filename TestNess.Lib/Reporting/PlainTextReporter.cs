// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System.IO;
using TestNess.Lib.Analysis;

namespace TestNess.Lib.Reporting
{
    public class PlainTextReporter : IReporter
    {
        public void GenerateReport(IReportReceiver recv, AnalysisResults results)
        {
            var writer = new StringWriter();
            writer.WriteLine("Analysis started at " + results.AnalysisTime);
            writer.WriteLine("Number of test cases analyzed = " + results.TestCaseCount);
            writer.WriteLine("Total analysis time (ms) = " + results.ElapsedTimeInMilliseconds);
            writer.WriteLine("Violations:");
            var score = 0m;
            foreach (var app in results.Applications)
            {
                score += app.Score;
                foreach (var violation in app.Violations)
                {
                    writer.WriteLine("  {0}", violation);
                }
            }

            writer.WriteLine();
            writer.WriteLine("Total score = " + score);
            recv.GenerateReport(writer.ToString());
        }
    }
}
