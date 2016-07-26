// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Pihrtsoft.CodeAnalysis.Metadata;

namespace MetadataGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var generator = new Generator();

            foreach (RefactoringInfo refactoring in RefactoringInfo
                .LoadFromFile(@"..\source\Refactorings\Refactorings.xml")
                .OrderBy(f => f.Identifier, StringComparer.InvariantCulture))
            {
                generator.Refactorings.Add(refactoring);
            }

            Console.WriteLine($"number of refactorings: {generator.Refactorings.Count}");

            foreach (AnalyzerInfo analyzer in AnalyzerInfo
                .LoadFromFile(@"..\source\Analyzers\Analyzers.xml")
                .OrderBy(f => f.Id, StringComparer.InvariantCulture))
            {
                generator.Analyzers.Add(analyzer);
            }

            Console.WriteLine($"number of analyzers: {generator.Analyzers.Count}");

            var writer = new CodeFileWriter();

            writer.SaveCode(
                @"..\source\Analyzers\Analyzers.xml",
                generator.CreateAnalyzersXml());

            writer.SaveCode(
                @"..\source\VisualStudio.AnalyzersAndRefactorings\description.txt",
                generator.CreateAnalyzersExtensionDescription());

            writer.SaveCode(
                @"..\source\VisualStudio.Refactorings\description.txt",
                generator.CreateRefactoringsExtensionDescription());

            writer.SaveCode(
                @"..\README.md",
                generator.CreateGitHubMarkDown());

            writer.SaveCode(
                @"..\Refactorings.md",
                generator.CreateRefactoringsMarkDown());

            Console.WriteLine("*** FINISHED ***");
            Console.ReadKey();
        }
    }
}
