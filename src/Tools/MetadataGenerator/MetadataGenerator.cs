// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using Roslynator.CodeGeneration.CSharp;
using Roslynator.CodeGeneration.Markdown;
using Roslynator.CodeGeneration.Xml;
using Roslynator.Metadata;
using Roslynator.Utilities;
using Microsoft.CodeAnalysis;

namespace Roslynator.CodeGeneration
{
    internal class MetadataGenerator : Generator
    {
        private static readonly UTF8Encoding _utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public MetadataGenerator(string rootPath, StringComparer comparer = null)
            : base(rootPath, comparer)
        {
        }

        public void Generate()
        {
            WriteAllText(
                @"Analyzers\README.md",
                MarkdownGenerator.CreateAnalyzersReadMe(Analyzers.Where(f => !f.IsObsolete), Comparer));

            WriteAllText(
                @"Analyzers\AnalyzersByCategory.md",
                MarkdownGenerator.CreateAnalyzersByCategoryMarkdown(Analyzers.Where(f => !f.IsObsolete), Comparer));

            foreach (AnalyzerDescriptor analyzer in Analyzers)
            {
                WriteAllText(
                    $@"..\docs\analyzers\{analyzer.Id}.md",
                    MarkdownGenerator.CreateAnalyzerMarkdown(analyzer),
                    fileMustExists: false);
            }

            WriteAllText(
                @"..\docs\refactorings\Refactorings.md",
                MarkdownGenerator.CreateRefactoringsMarkdown(Refactorings, Comparer));

            WriteAllText(
                @"Refactorings\README.md",
                MarkdownGenerator.CreateRefactoringsReadMe(Refactorings.Where(f => !f.IsObsolete), Comparer));

            foreach (RefactoringDescriptor refactoring in Refactorings)
            {
                WriteAllText(
                    $@"..\docs\refactorings\{refactoring.Id}.md",
                    MarkdownGenerator.CreateRefactoringMarkdown(refactoring),
                    fileMustExists: false);
            }

            WriteAllText(
                @"CodeFixes\README.md",
                MarkdownGenerator.CreateCodeFixesReadMe(CompilerDiagnostics, Comparer));

            foreach (CompilerDiagnosticDescriptor diagnostic in CompilerDiagnostics)
            {
                WriteAllText(
                    $@"..\docs\cs\{diagnostic.Id}.md",
                    MarkdownGenerator.CreateCompilerDiagnosticMarkdown(diagnostic, CodeFixes, Comparer),
                    fileMustExists: false);
            }

            WriteAllText(
                "DefaultConfigFile.xml",
                XmlGenerator.CreateDefaultConfigFile(Refactorings, CodeFixes));

            WriteAllText(
                "DefaultRuleSet.ruleset",
                XmlGenerator.CreateDefaultRuleSet(Analyzers));
        }

        public void WriteAllText(string relativePath, string content, bool onlyIfChanges = true, bool fileMustExists = true)
        {
            string path = GetPath(relativePath);

            Encoding encoding = (Path.GetExtension(path) == ".md") ? _utf8NoBom : Encoding.UTF8;

            FileHelper.WriteAllText(path, content, encoding, onlyIfChanges, fileMustExists);
        }

        public void FindFilesToDelete()
        {
            foreach (string path in Directory.EnumerateFiles(GetPath(@"..\docs\refactorings")))
            {
                if (Path.GetFileName(path) != "Refactorings.md"
                    && !Refactorings.Any(f => f.Id == Path.GetFileNameWithoutExtension(path)))
                {
                    Console.WriteLine($"FILE TO DELETE: {path}");
                }
            }
        }

        public void FindMissingSamples()
        {
            foreach (RefactoringDescriptor refactoring in Refactorings)
            {
                if (refactoring.Samples.Count == 0)
                {
                    foreach (ImageDescriptor image in refactoring.ImagesOrDefaultImage())
                    {
                        string imagePath = Path.Combine(GetPath(@"..\images\refactorings"), image.Name + ".png");

                        if (!File.Exists(imagePath))
                            Console.WriteLine($"MISSING SAMPLE: {imagePath}");
                    }
                }
            }
        }
    }
}
