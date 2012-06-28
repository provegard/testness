// Copyright (C) 2011-2012 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
using System;
using TestNess.Lib.Analysis;

namespace TestNess.Lib.Reporting
{
    public class ConsoleReporter : IReporter
    {
        public void GenerateReport(AnalysisResults results)
        {
            Console.WriteLine("Analysis started at " + results.AnalysisTime);
            Console.WriteLine("Total analysis time (ms) = " + results.ElapsedTimeInMilliseconds);
            Console.WriteLine("Violations:");
            var score = 0m;
            foreach (var app in results.Applications)
            {
                score += app.Score;
                foreach (var violation in app.Violations)
                {
                    Console.WriteLine("  {0}", violation);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Total score = " + score);
        }
    }
}
