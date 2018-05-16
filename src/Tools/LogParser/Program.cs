// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Roslynator.Diagnostics
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args?.Length != 1)
            {
                if (!Debugger.IsAttached)
                    return;

                args = new string[] { @"..\..\..\..\..\..\tools\msbuild.log" };
            }

            string filePath = args[0];

            Console.WriteLine($"Reading file \"{filePath}\"");

            List<ProjectDiagnosticInfo> projectDiagnostics = LogParser.Parse(filePath).ToList();

            int total = projectDiagnostics.Sum(f => f.Total);

            int totalElapsed = 0;

            foreach (AnalyzerDiagnosticInfo info in projectDiagnostics
                .SelectMany(f => f.AnalyzerDiagnostics)
                .Where(f => f.FullName.StartsWith("Roslynator.", StringComparison.Ordinal) && f.Elapsed > 0)
                .GroupBy(f => f.FullName)
                .Select(f => new AnalyzerDiagnosticInfo(f.Key, f.Sum(g => g.Elapsed), f.Sum(g => g.Percent) / f.Count()))
                .OrderBy(f => f.Elapsed))
            {
                Console.WriteLine(info.Elapsed.ToString("n0", CultureInfo.InvariantCulture) + " " + info.Name);

                totalElapsed += info.Elapsed;
            }

            Console.WriteLine();
            Console.WriteLine($"Total analyzer execution time: {total.ToString("n0", CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Sum of analyzer times: {totalElapsed.ToString("n0", CultureInfo.InvariantCulture)}");

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Done...");
                Console.ReadKey();
            }
        }
    }
}
