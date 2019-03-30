// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Roslynator.CodeGeneration
{
    internal static class Program
    {
        private static readonly UTF8Encoding _utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private static async Task Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
#if DEBUG
                args = new string[] { @"..\..\..\..\.." };
#else
                args = new string[] { Environment.CurrentDirectory };
#endif
            }

            string rootPath = args[0];

            StringComparer comparer = StringComparer.InvariantCulture;

            var metadata = new RoslynatorMetadata(rootPath);

            ImmutableArray<AnalyzerMetadata> analyzers = metadata.Analyzers;
            ImmutableArray<RefactoringMetadata> refactorings = metadata.Refactorings;
            ImmutableArray<CodeFixMetadata> codeFixes = metadata.CodeFixes;
            ImmutableArray<CompilerDiagnosticMetadata> compilerDiagnostics = metadata.CompilerDiagnostics;

            WriteAnalyzersReadMe(@"Analyzers\README.md", analyzers);

            WriteAnalyzersByCategory(@"Analyzers\AnalyzersByCategory.md", analyzers);
#if !DEBUG
            VisualStudioInstance instance = MSBuildLocator.QueryVisualStudioInstances().First(f => f.Version.Major == 15);

            MSBuildLocator.RegisterInstance(instance);

            using (MSBuildWorkspace workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);

                string solutionPath = Path.Combine(rootPath, "Roslynator.sln");

                Console.WriteLine($"Loading solution '{solutionPath}'");

                Solution solution = await workspace.OpenSolutionAsync(solutionPath).ConfigureAwait(false);

                Console.WriteLine($"Finished loading solution '{solutionPath}'");

                RoslynatorInfo roslynatorInfo = await RoslynatorInfo.Create(solution).ConfigureAwait(false);

                IOrderedEnumerable<SourceFile> sourceFiles = analyzers
                    .Select(f => new SourceFile(f.Id, roslynatorInfo.GetAnalyzerFilesAsync(f.Identifier).Result))
                    .Concat(refactorings
                        .Select(f => new SourceFile(f.Id, roslynatorInfo.GetRefactoringFilesAsync(f.Identifier).Result)))
                    .OrderBy(f => f.Id);

                MetadataFile.SaveSourceFiles(sourceFiles, @"..\SourceFiles.xml");
            }
#endif
            foreach (AnalyzerMetadata analyzer in analyzers)
            {
                WriteAllText(
                    $@"..\docs\analyzers\{analyzer.Id}.md",
                    MarkdownGenerator.CreateAnalyzerMarkdown(analyzer, Array.Empty<string>()),
                    fileMustExists: false);
            }

            foreach (RefactoringMetadata refactoring in refactorings)
            {
                WriteAllText(
                    $@"..\docs\refactorings\{refactoring.Id}.md",
                    MarkdownGenerator.CreateRefactoringMarkdown(refactoring, Array.Empty<string>()),
                    fileMustExists: false);
            }

            foreach (CompilerDiagnosticMetadata diagnostic in compilerDiagnostics)
            {
                WriteAllText(
                    $@"..\docs\cs\{diagnostic.Id}.md",
                    MarkdownGenerator.CreateCompilerDiagnosticMarkdown(diagnostic, codeFixes, comparer, Array.Empty<string>()),
                    fileMustExists: false);
            }

            WriteAllText(
                @"..\docs\refactorings\Refactorings.md",
                MarkdownGenerator.CreateRefactoringsMarkdown(refactorings, comparer));

            WriteAllText(
                @"Refactorings\README.md",
                MarkdownGenerator.CreateRefactoringsReadMe(refactorings.Where(f => !f.IsObsolete), comparer));

            WriteAllText(
                @"CodeFixes\README.md",
                MarkdownGenerator.CreateCodeFixesReadMe(compilerDiagnostics, comparer));

            WriteAllText(
                "DefaultConfigFile.xml",
                XmlGenerator.CreateDefaultConfigFile(refactorings, codeFixes));

            WriteAllText(
                "default.ruleset",
                XmlGenerator.CreateDefaultRuleSet(analyzers));

            // find files to delete
            foreach (string path in Directory.EnumerateFiles(GetPath(@"..\docs\refactorings")))
            {
                if (Path.GetFileName(path) != "Refactorings.md"
                    && !refactorings.Any(f => f.Id == Path.GetFileNameWithoutExtension(path)))
                {
                    Console.WriteLine($"FILE TO DELETE: {path}");
                }
            }

            // find missing samples
            foreach (RefactoringMetadata refactoring in refactorings)
            {
                if (refactoring.Samples.Count == 0)
                {
                    foreach (ImageMetadata image in refactoring.ImagesOrDefaultImage())
                    {
                        string imagePath = Path.Combine(GetPath(@"..\images\refactorings"), image.Name + ".png");

                        if (!File.Exists(imagePath))
                            Console.WriteLine($"MISSING SAMPLE: {imagePath}");
                    }
                }
            }

            void WriteAnalyzersReadMe(string path, ImmutableArray<AnalyzerMetadata> descriptors)
            {
                WriteAllText(
                    path,
                    MarkdownGenerator.CreateAnalyzersReadMe(descriptors.Where(f => !f.IsObsolete), comparer));
            }

            void WriteAnalyzersByCategory(string path, ImmutableArray<AnalyzerMetadata> descriptors)
            {
                WriteAllText(
                    path,
                    MarkdownGenerator.CreateAnalyzersByCategoryMarkdown(descriptors.Where(f => !f.IsObsolete), comparer));
            }

            void WriteAllText(string relativePath, string content, bool onlyIfChanges = true, bool fileMustExists = true)
            {
                string path = GetPath(relativePath);

                Encoding encoding = (Path.GetExtension(path) == ".md") ? _utf8NoBom : Encoding.UTF8;

                FileHelper.WriteAllText(path, content, encoding, onlyIfChanges, fileMustExists);
            }

            string GetPath(string path)
            {
                return Path.Combine(rootPath, path);
            }
        }
    }
}
