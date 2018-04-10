// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
            if (args == null)
                return;

            if (args.Length != 1)
                return;

            string filePath = args[0];

            string content = File.ReadAllText(filePath, Encoding.UTF8);

            foreach (AnalyzerLogInfo info in LogParser.Parse(content)
                .Where(f => !f.FullName.StartsWith("Microsoft.CodeAnalysis", StringComparison.Ordinal))
                .GroupBy(f => f.FullName)
                .Select(f => new AnalyzerLogInfo(f.Key, f.Sum(g => g.Elapsed), f.Sum(g => g.Percent) / f.Count()))
                .OrderByDescending(f => f.Elapsed))
            {
                Console.WriteLine(info.Elapsed.ToString("n0", CultureInfo.InvariantCulture) + " " + info.FullName);
            }

            Console.WriteLine("DONE");
        }
    }
}
