// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;

namespace Roslynator.CodeGeneration
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
#if DEBUG
                args = new string[] { @"..\..\..\.." };
#else
                args = new string[] { Environment.CurrentDirectory };
#endif
            }

            string dirPath = args[0];

            var generator = new CodeGenerator(dirPath, StringComparer.InvariantCulture);

            generator.Generate();

            Console.WriteLine($"number of analyzers: {generator.Analyzers.Count(f => !f.IsObsolete)}");
            Console.WriteLine($"number of refactorings: {generator.Refactorings.Length}");
            Console.WriteLine($"number of code fixes: {generator.CodeFixes.Length}");
            Console.WriteLine($"number of fixable compiler diagnostics: {generator.CodeFixes.SelectMany(f => f.FixableDiagnosticIds).Distinct().Count()}");
        }
    }
}
