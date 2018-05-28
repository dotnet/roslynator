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

            IOrderedEnumerable<IGrouping<string, AnalyzerDiagnosticInfo>> groupedByRootNamespace = projectDiagnostics
                .SelectMany(f => f.AnalyzerDiagnostics)
                .Where(f => f.Elapsed > 0)
                .GroupBy(f => f.RootNamespace)
                .OrderBy(f => f.Key);

            foreach (IGrouping<string, AnalyzerDiagnosticInfo> grouping in groupedByRootNamespace)
            {
                Console.WriteLine(grouping.Key);

                foreach (AnalyzerDiagnosticInfo info in grouping
                    .OrderBy(f => f.Elapsed))
                {
                    Console.WriteLine(info.Elapsed.ToString("n0", CultureInfo.InvariantCulture) + " " + info.Name);
                }

                Console.WriteLine();
            }

            foreach ((string rootNamespace, int elapsed) in groupedByRootNamespace
                .Select(f => (rootNamespace: f.Key, elapsed: f.Sum(info => info.Elapsed)))
                .OrderBy(f => f.elapsed))
            {
                Console.WriteLine($"{rootNamespace}: {elapsed.ToString("n0", CultureInfo.InvariantCulture)}");
            }

            Console.WriteLine($"Total analyzer execution time: {projectDiagnostics.Sum(f => f.Total).ToString("n0", CultureInfo.InvariantCulture)}");

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Done...");
                Console.ReadKey();
            }
        }
    }
}
