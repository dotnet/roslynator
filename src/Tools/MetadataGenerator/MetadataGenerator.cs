// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Roslynator.CodeGeneration.Markdown;
using Roslynator.CodeGeneration.Xml;
using Roslynator.Metadata;
using Roslynator.Utilities;

namespace Roslynator.CodeGeneration
{
    internal class MetadataGenerator : Generator
    {
        private static readonly UTF8Encoding _utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public MetadataGenerator(string rootPath, StringComparer comparer = null)
            : base(rootPath, comparer)
        {
        }

        public async Task GenerateAsync()
        {
            WriteAllText(
                @"Analyzers\README.md",
                MarkdownGenerator.CreateAnalyzersReadMe(Analyzers.Where(f => !f.IsObsolete), Comparer));

            WriteAllText(
                @"Analyzers\AnalyzersByCategory.md",
                MarkdownGenerator.CreateAnalyzersByCategoryMarkdown(Analyzers.Where(f => !f.IsObsolete), Comparer));

            VisualStudioInstance instance = MSBuildLocator.QueryVisualStudioInstances().Single();

            MSBuildLocator.RegisterInstance(instance);

            using (MSBuildWorkspace workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);

                string solutionPath = Path.Combine(RootPath, "Roslynator.sln");

                Console.WriteLine($"Loading solution '{solutionPath}'");

                Solution solution = await workspace.OpenSolutionAsync(solutionPath).ConfigureAwait(false);

                Console.WriteLine($"Finished loading solution '{solutionPath}'");

                RoslynatorInfo roslynatorInfo = await RoslynatorInfo.Create(solution).ConfigureAwait(false);

                IOrderedEnumerable<SourceFile> sourceFiles = Analyzers
                    .Select(f => new SourceFile(f.Id, roslynatorInfo.GetAnalyzerFilesAsync(f.Identifier).Result))
                    .Concat(Refactorings
                        .Select(f => new SourceFile(f.Id, roslynatorInfo.GetRefactoringFilesAsync(f.Identifier).Result)))
                    .OrderBy(f => f.Id);

                MetadataFile.SaveSourceFiles(sourceFiles, @"..\SourceFiles.xml");

                foreach (AnalyzerDescriptor analyzer in Analyzers)
                {
                    //IEnumerable<string> filePaths = await roslynatorInfo.GetAnalyzerFilesAsync(analyzer.Identifier).ConfigureAwait(false);

                    WriteAllText(
                        $@"..\docs\analyzers\{analyzer.Id}.md",
                        MarkdownGenerator.CreateAnalyzerMarkdown(analyzer, Array.Empty<string>()),
                        fileMustExists: false);
                }

                foreach (RefactoringDescriptor refactoring in Refactorings)
                {
                    //IEnumerable<string> filePaths = await roslynatorInfo.GetRefactoringFilesAsync(refactoring.Identifier).ConfigureAwait(false);

                    WriteAllText(
                        $@"..\docs\refactorings\{refactoring.Id}.md",
                        MarkdownGenerator.CreateRefactoringMarkdown(refactoring, Array.Empty<string>()),
                        fileMustExists: false);
                }

                foreach (CompilerDiagnosticDescriptor diagnostic in CompilerDiagnostics)
                {
                    //IEnumerable<string> filePaths = await roslynatorInfo.GetCompilerDiagnosticFilesAsync(diagnostic.Identifier).ConfigureAwait(false);

                    WriteAllText(
                        $@"..\docs\cs\{diagnostic.Id}.md",
                        MarkdownGenerator.CreateCompilerDiagnosticMarkdown(diagnostic, CodeFixes, Comparer, Array.Empty<string>()),
                        fileMustExists: false);
                }
            }

            WriteAllText(
                @"..\docs\refactorings\Refactorings.md",
                MarkdownGenerator.CreateRefactoringsMarkdown(Refactorings, Comparer));

            WriteAllText(
                @"Refactorings\README.md",
                MarkdownGenerator.CreateRefactoringsReadMe(Refactorings.Where(f => !f.IsObsolete), Comparer));

            WriteAllText(
                @"CodeFixes\README.md",
                MarkdownGenerator.CreateCodeFixesReadMe(CompilerDiagnostics, Comparer));

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
