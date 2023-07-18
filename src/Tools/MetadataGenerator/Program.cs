// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Roslynator.CodeGeneration.Markdown;
using Roslynator.Metadata;
using Roslynator.Utilities;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Roslynator.CodeGeneration;

internal static class Program
{
    private static readonly UTF8Encoding _utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    private static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Invalid number of arguments");
        }

        if (args is null || args.Length == 0)
        {
            args = new string[] { Environment.CurrentDirectory };
        }

        string sourcePath = args[0];
        string destinationPath = args[1];

        StringComparer comparer = StringComparer.InvariantCulture;

        var metadata = new RoslynatorMetadata(sourcePath);

        ImmutableArray<AnalyzerMetadata> analyzers = metadata.Analyzers;
        ImmutableArray<AnalyzerMetadata> codeAnalysisAnalyzers = metadata.CodeAnalysisAnalyzers;
        ImmutableArray<AnalyzerMetadata> formattingAnalyzers = metadata.FormattingAnalyzers;
        ImmutableArray<RefactoringMetadata> refactorings = metadata.Refactorings;
        ImmutableArray<CodeFixMetadata> codeFixes = metadata.CodeFixes;
        ImmutableArray<CompilerDiagnosticMetadata> compilerDiagnostics = metadata.CompilerDiagnostics;

#if !DEBUG
        await GenerateSourceFilesXml(
            sourcePath,
            analyzers
                .Concat(codeAnalysisAnalyzers)
                .Concat(formattingAnalyzers),
            refactorings)
            .ConfigureAwait(false);
#endif

        string analyzersDirPath = Path.Combine(destinationPath, "analyzers");

        Directory.CreateDirectory(analyzersDirPath);

        WriteAllText(
            $"{destinationPath}/analyzers.md",
            MarkdownGenerator.CreateAnalyzersMarkdown(metadata.GetAllAnalyzers().Where(f => !f.IsObsolete), "Analyzers", comparer),
            sidebarLabel: "Analyzers");

        WriteAnalyzerMarkdowns(codeAnalysisAnalyzers, new (string, string)[] { ("Roslynator.CodeAnalysis.Analyzers", "https://www.nuget.org/packages/Roslynator.CodeAnalysis.Analyzers") });

        WriteAnalyzerMarkdowns(formattingAnalyzers, new (string, string)[] { ("Roslynator.Formatting.Analyzers", "https://www.nuget.org/packages/Roslynator.Formatting.Analyzers") });

        WriteAnalyzerMarkdowns(analyzers);

        DeleteInvalidAnalyzerMarkdowns();

        string refactoringsDirPath = Path.Combine(destinationPath, "refactorings");

        Directory.CreateDirectory(refactoringsDirPath);

        foreach (RefactoringMetadata refactoring in refactorings)
        {
            string filePath = $"{refactoringsDirPath}/{refactoring.Id}.md";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            WriteAllText(
                filePath,
                MarkdownGenerator.CreateRefactoringMarkdown(refactoring));
        }

        IEnumerable<CompilerDiagnosticMetadata> fixableCompilerDiagnostics = compilerDiagnostics
            .Join(codeFixes.SelectMany(f => f.FixableDiagnosticIds), f => f.Id, f => f, (f, _) => f)
            .Distinct();

        ImmutableArray<CodeFixOption> codeFixOptions = typeof(CodeFixOptions).GetFields()
            .Select(f =>
            {
                var key = (string)f.GetValue(null);
                string value = f.GetCustomAttribute<CodeFixOptionAttribute>().Value;

                return new CodeFixOption(key, value);
            })
            .ToImmutableArray();

        string fixesDirPath = Path.Combine(destinationPath, "fixes");

        Directory.CreateDirectory(fixesDirPath);

        foreach (CompilerDiagnosticMetadata diagnostic in fixableCompilerDiagnostics)
        {
            string filePath = $"{fixesDirPath}/{diagnostic.Id}.md";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            WriteAllText(
                filePath,
                MarkdownGenerator.CreateCodeFixMarkdown(diagnostic, codeFixes, codeFixOptions, comparer));
        }

        WriteAllText(
            $"{destinationPath}/refactorings.md",
            MarkdownGenerator.CreateRefactoringsMarkdown(refactorings.Where(f => !f.IsObsolete), comparer),
            sidebarLabel: "Refactorings");

        WriteAllText(
            $"{destinationPath}/fixes.md",
            MarkdownGenerator.CreateCodeFixesReadMe(fixableCompilerDiagnostics, comparer),
            sidebarLabel: "Code Fixes for Compiler Diagnostics");

        // find files to delete
        foreach (string path in Directory.EnumerateFiles($"{refactoringsDirPath}"))
        {
            if (!refactorings.Any(f => f.Id == Path.GetFileNameWithoutExtension(path)))
                Console.WriteLine($"FILE TO DELETE: {path}");
        }

        // find missing samples
        foreach (RefactoringMetadata refactoring in refactorings)
        {
            if (!refactoring.IsObsolete
                && refactoring.Samples.Count == 0)
            {
                foreach (ImageMetadata image in refactoring.ImagesOrDefaultImage())
                {
                    string imagePath = Path.Combine(Path.Combine(sourcePath, "../images/refactorings"), image.Name + ".png");

                    if (!File.Exists(imagePath))
                        Console.WriteLine($"MISSING SAMPLE: {imagePath}");
                }
            }
        }

        UpdateChangeLog();

        void WriteAnalyzerMarkdowns(IEnumerable<AnalyzerMetadata> analyzers, IEnumerable<(string title, string url)> appliesTo = null)
        {
            foreach (AnalyzerMetadata analyzer in analyzers)
                WriteAnalyzerMarkdown(analyzer, appliesTo);
        }

        void WriteAnalyzerMarkdown(AnalyzerMetadata analyzer, IEnumerable<(string title, string url)> appliesTo = null)
        {
            string filePath = $"{analyzersDirPath}/{analyzer.Id}.md";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            WriteAllText(
                filePath,
                MarkdownGenerator.CreateAnalyzerMarkdown(analyzer, metadata.ConfigOptions, appliesTo));
        }

        void DeleteInvalidAnalyzerMarkdowns()
        {
            AnalyzerMetadata[] allAnalyzers = analyzers
                .Concat(codeAnalysisAnalyzers)
                .Concat(formattingAnalyzers)
                .ToArray();

            IEnumerable<string> allIds = allAnalyzers
                .Concat(allAnalyzers.SelectMany(f => f.OptionAnalyzers))
                .Select(f => f.Id);

            foreach (string id in Directory.GetFiles(analyzersDirPath, "*.*", SearchOption.TopDirectoryOnly)
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .Except(allIds))
            {
                if (id == "RCSXXXX")
                    break;

                string filePath = Path.Combine(analyzersDirPath, Path.ChangeExtension(id, ".md"));

                Console.WriteLine($"Delete file '{filePath}'");

                File.Delete(filePath);
            }
        }

        void UpdateChangeLog()
        {
            var issueRegex = new Regex(@"\(\#(?<issue>\d+)\)");
            var analyzerRegex = new Regex(@"(\p{Lu}\p{Ll}+){2,}\ +\((?<id>RCS\d{4}[a-z]?)\)");

            string path = Path.Combine(sourcePath, "../ChangeLog.md");
            string s = File.ReadAllText(path, _utf8NoBom);

            List<AnalyzerMetadata> allAnalyzers = analyzers
                .Concat(formattingAnalyzers)
                .Concat(codeAnalysisAnalyzers)
                .ToList();

            ImmutableDictionary<string, AnalyzerMetadata> dic = allAnalyzers
                .Concat(allAnalyzers.SelectMany(f => f.OptionAnalyzers))
                .Where(f => f.Id is not null)
                .ToImmutableDictionary(f => f.Id, f => f);

            s = issueRegex.Replace(s, "([issue](https://github.com/JosefPihrt/Roslynator/issues/${issue}))");

            s = analyzerRegex.Replace(
                s,
                m =>
                {
                    string id = m.Groups["id"].Value;

                    Debug.Assert(dic.ContainsKey(id), id);

                    AnalyzerMetadata analyzer = dic[id];

                    return $"[{id}](https://josefpihrt.github.io/docs/roslynator/analyzers/{id}) ({analyzer.Title.TrimEnd('.')})";
                });

            File.WriteAllText(path, s, _utf8NoBom);
        }

        void WriteAllText(
            string path,
            string content,
            bool onlyIfChanges = true,
            bool fileMustExists = false,
            int? sidebarPosition = null,
            string sidebarLabel = null)
        {
            Encoding encoding = (Path.GetExtension(path) == ".md") ? _utf8NoBom : Encoding.UTF8;

            content = DocusaurusUtility.CreateFrontMatter(position: sidebarPosition, label: sidebarLabel) + content;

            FileHelper.WriteAllText(path, content, encoding, onlyIfChanges, fileMustExists);
        }
    }
#if !DEBUG
    private static async Task GenerateSourceFilesXml(
        string sourcePath,
        IEnumerable<AnalyzerMetadata> analyzers,
        ImmutableArray<RefactoringMetadata> refactorings)
    {
        VisualStudioInstance instance = MSBuildLocator.QueryVisualStudioInstances().First(f => f.Version.Major == 7);

        MSBuildLocator.RegisterInstance(instance);

        using (MSBuildWorkspace workspace = MSBuildWorkspace.Create())
        {
            workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);

            string solutionPath = Path.Combine(sourcePath, "Roslynator.sln");

            Console.WriteLine($"Loading solution '{solutionPath}'");

            Solution solution = await workspace.OpenSolutionAsync(solutionPath).ConfigureAwait(false);

            Console.WriteLine($"Finished loading solution '{solutionPath}'");

            RoslynatorInfo roslynatorInfo = await RoslynatorInfo.Create(solution).ConfigureAwait(false);

            IOrderedEnumerable<SourceFile> sourceFiles = analyzers
                .Select(f => new SourceFile(f.Id, roslynatorInfo.GetAnalyzerFilesAsync(f.Identifier).Result))
                .Concat(refactorings
                    .Select(f => new SourceFile(f.Id, roslynatorInfo.GetRefactoringFilesAsync(f.Identifier).Result)))
                .OrderBy(f => f.Id);

            MetadataFile.SaveSourceFiles(sourceFiles, "../SourceFiles.xml");
        }
    }
#endif
}
