// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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

                args = new string[] { @"..\..\..\..\..\msbuild.log" };
            }

            string filePath = args[0];

            Console.WriteLine($"Reading file \"{filePath}\"");

            string content = File.ReadAllText(filePath, Encoding.UTF8);

            int totalElapsed = 0;

            foreach (AnalyzerLogInfo info in LogParser.Parse(content)
                .Where(f => f.FullName.StartsWith("Roslynator.", StringComparison.Ordinal))
                .GroupBy(f => f.FullName)
                .Select(f => new AnalyzerLogInfo(f.Key, f.Sum(g => g.Elapsed), f.Sum(g => g.Percent) / f.Count()))
                .OrderBy(f => f.Elapsed))
            {
                Console.WriteLine(info.Elapsed.ToString("n0", CultureInfo.InvariantCulture) + " " + info.Name);

                totalElapsed += info.Elapsed;
            }

            Console.WriteLine();
            Console.WriteLine($"Total seconds elapsed: {totalElapsed.ToString("n0", CultureInfo.InvariantCulture)}");

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Log parser finished");
                Console.ReadKey();
            }
        }
    }
}
