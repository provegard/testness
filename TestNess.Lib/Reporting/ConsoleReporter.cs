using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
