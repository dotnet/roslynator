// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using Roslynator.CodeGeneration.Markdown;
using Roslynator.CodeGeneration.Xml;
using Roslynator.Metadata;
using Roslynator.Utilities;

namespace Roslynator.CodeGeneration
{
    internal class MetadataGenerator : Generator
    {
        private static readonly Encoding _utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

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
                MarkdownGenerator.CreateAnalyzersByCategoryMarkDown(Analyzers.Where(f => !f.IsObsolete), Comparer));

            foreach (AnalyzerDescriptor analyzer in Analyzers)
            {
                WriteAllText(
                    $@"..\docs\analyzers\{analyzer.Id}.md",
                    MarkdownGenerator.CreateAnalyzerMarkDown(analyzer),
                    fileMustExists: false);
            }

            WriteAllText(
                @"..\docs\refactorings\Refactorings.md",
                MarkdownGenerator.CreateRefactoringsMarkDown(Refactorings, Comparer));

            WriteAllText(
                @"Refactorings\README.md",
                MarkdownGenerator.CreateRefactoringsReadMe(Refactorings.Where(f => !f.IsObsolete), Comparer));

            foreach (RefactoringDescriptor refactoring in Refactorings)
            {
                WriteAllText(
                    $@"..\docs\refactorings\{refactoring.Id}.md",
                    MarkdownGenerator.CreateRefactoringMarkDown(refactoring),
                    fileMustExists: false);
            }

            WriteAllText(
                @"CodeFixes\README.md",
                MarkdownGenerator.CreateCodeFixesReadMe(CodeFixes, CompilerDiagnostics, Comparer));

            WriteAllText(
                @"CodeFixes\CodeFixesByDiagnosticId.md",
                MarkdownGenerator.CreateCodeFixesByDiagnosticId(CodeFixes, CompilerDiagnostics));

            WriteAllText(
                "DefaultConfigFile.xml",
                XmlGenerator.CreateDefaultConfigFile(Refactorings, CodeFixes));

            WriteAllText(
                @"VisualStudio\description.txt",
                File.ReadAllText(@"..\text\RoslynatorDescription.txt", Encoding.UTF8));

            WriteAllText(
                @"VisualStudio.Refactorings\description.txt",
                File.ReadAllText(@"..\text\RoslynatorRefactoringsDescription.txt", Encoding.UTF8));
        }

        public void WriteAllText(string relativePath, string content, bool onlyIfChanges = true, bool fileMustExists = true)
        {
            string path = GetPath(relativePath);

            Encoding encoding = (Path.GetExtension(path) == ".md") ? _utf8NoBom : Encoding.UTF8;

            FileHelper.WriteAllText(path, content, _utf8NoBom, onlyIfChanges, fileMustExists);
        }

        public void FindFilesToDelete()
        {
            foreach (string path in Directory.EnumerateFiles(GetPath(@"..\docs\refactorings")))
            {
                if (Path.GetFileName(path) != "Refactorings.md"
                    && Refactorings.FirstOrDefault(f => f.Id == Path.GetFileNameWithoutExtension(path)) == null)
                {
                    Console.WriteLine($"FILE TO DELETE: {path}");
                }
            }
        }

        public void FindMissingImages()
        {
            foreach (RefactoringDescriptor refactoring in Refactorings)
            {
                foreach (ImageDescriptor image in refactoring.ImagesOrDefaultImage())
                {
                    string imagePath = Path.Combine(GetPath(@"..\images\refactorings"), image.Name + ".png");

                    if (!File.Exists(imagePath))
                        Console.WriteLine($"MISSING IMAGE: {imagePath}");
                }
            }
        }    }
}
