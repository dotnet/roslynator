﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotMarkdown.Docusaurus;
using Roslynator.CodeGeneration;
using Roslynator.CodeGeneration.Markdown;
using Roslynator.Metadata;
using Roslynator.Utilities;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Roslynator.MetadataGenerator;

internal static class Program
{
    private static readonly UTF8Encoding _utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    private static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Invalid number of arguments");
            return;
        }

        string sourcePath = args[0];
        string destinationPath = args[1];

        var metadata = new RoslynatorMetadata(sourcePath);

        GenerateAnalyzersMarkdown(metadata, destinationPath);

        GenerateRefactoringsMarkdown(metadata, destinationPath);

        GenerateCodeFixesMarkdown(metadata, destinationPath);

        UpdateChangeLog();

        void UpdateChangeLog()
        {
            var issueRegex = new Regex(@"\(\#(?<issue>\d+)\)");
            var analyzerRegex = new Regex(@"(\p{Lu}\p{Ll}+){2,}\ +\((?<id>RCS\d{4}[a-z]?)\)");

            string path = Path.Combine(sourcePath, "../ChangeLog.md");
            string s = File.ReadAllText(path, _utf8NoBom);

            ImmutableDictionary<string, AnalyzerMetadata> dic = metadata.Analyzers
                .Where(f => f.Id is not null)
                .ToImmutableDictionary(f => f.Id, f => f);

            s = issueRegex.Replace(s, "([issue](https://github.com/dotnet/roslynator/issues/${issue}))");

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
    }

    private static void GenerateAnalyzersMarkdown(RoslynatorMetadata metadata, string destinationPath)
    {
        string analyzersDirPath = Path.Combine(destinationPath, "analyzers");

        Directory.CreateDirectory(analyzersDirPath);

        WriteAllText(
            $"{destinationPath}/analyzers.md",
            MarkdownGenerator.CreateAnalyzersMarkdown(metadata.Analyzers.Where(f => f.Status == AnalyzerStatus.Enabled), "Analyzers", StringComparer.InvariantCulture),
            sidebarLabel: "Analyzers");

        foreach (AnalyzerMetadata analyzer in metadata.Analyzers.Where(f => f.Status != AnalyzerStatus.Disabled))
        {
            string filePath = $"{analyzersDirPath}/{analyzer.Id}.md";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            WriteAllText(filePath, MarkdownGenerator.CreateAnalyzerMarkdown(analyzer, metadata.ConfigOptions));
        }

        DeleteInvalidAnalyzerMarkdowns();

        void DeleteInvalidAnalyzerMarkdowns()
        {
            IEnumerable<string> allIds = metadata.Analyzers.Select(f => f.Id);

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
    }

    private static void GenerateRefactoringsMarkdown(RoslynatorMetadata metadata, string destinationPath)
    {
        string refactoringsDirPath = Path.Combine(destinationPath, "refactorings");
        Directory.CreateDirectory(refactoringsDirPath);

        ImmutableArray<RefactoringMetadata> refactorings = metadata.Refactorings;

        WriteAllText(
            $"{destinationPath}/refactorings.md",
            MarkdownGenerator.CreateRefactoringsMarkdown(refactorings.Where(f => !f.IsObsolete), StringComparer.InvariantCulture),
            sidebarLabel: "Refactorings");

        int refactoringIndex = 0;
        foreach (RefactoringMetadata refactoring in refactorings.OrderBy(f => f.Title))
        {
            string filePath = $"{refactoringsDirPath}/{refactoring.Id}.md";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            WriteAllText(
                filePath,
                MarkdownGenerator.CreateRefactoringMarkdown(refactoring, refactoringIndex));

            refactoringIndex++;
        }

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
                Console.WriteLine($"MISSING SAMPLE: {refactoring.Id}");
            }
        }
    }

    private static void GenerateCodeFixesMarkdown(RoslynatorMetadata metadata, string destinationPath)
    {
        IEnumerable<CompilerDiagnosticMetadata> diagnostics = metadata.CompilerDiagnostics
            .Join(metadata.CodeFixes.SelectMany(f => f.FixableDiagnosticIds), f => f.Id, f => f, (f, _) => f)
            .Distinct();

        WriteAllText(
            $"{destinationPath}/fixes.md",
            MarkdownGenerator.CreateCodeFixesMarkdown(diagnostics, StringComparer.InvariantCulture),
            sidebarLabel: "Code Fixes for Compiler Diagnostics");

        List<CodeFixOption> codeFixOptions = typeof(CodeFixOptions).GetFields()
            .Select(f => (CodeFixOption)f.GetValue(null))
            .ToList();

        string fixesDirPath = Path.Combine(destinationPath, "fixes");
        Directory.CreateDirectory(fixesDirPath);

        foreach (CompilerDiagnosticMetadata diagnostic in diagnostics)
        {
            string filePath = $"{fixesDirPath}/{diagnostic.Id}.md";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            WriteAllText(
                filePath,
                MarkdownGenerator.CreateCodeFixMarkdown(diagnostic, metadata.CodeFixes, codeFixOptions, StringComparer.InvariantCulture));
        }
    }

    private static void WriteAllText(
        string path,
        string content,
        bool onlyIfChanges = true,
        bool fileMustExists = false,
        int? sidebarPosition = null,
        string sidebarLabel = null)
    {
        content = DocusaurusMarkdownFactory.FrontMatter(GetLabels()) + content;

        IEnumerable<(string, object)> GetLabels()
        {
            if (sidebarPosition is not null)
                yield return ("sidebar_position", sidebarPosition);

            if (sidebarLabel is not null)
                yield return ("sidebar_label", sidebarLabel);
        }

        Encoding encoding = (Path.GetExtension(path) == ".md") ? _utf8NoBom : Encoding.UTF8;

        FileHelper.WriteAllText(path, content, encoding, onlyIfChanges, fileMustExists);
    }
}
