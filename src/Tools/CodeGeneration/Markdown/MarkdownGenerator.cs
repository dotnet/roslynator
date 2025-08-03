﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DotMarkdown;
using DotMarkdown.Docusaurus;
using DotMarkdown.Linq;
using Roslynator.Metadata;
using static System.Environment;
using static DotMarkdown.Linq.MFactory;

namespace Roslynator.CodeGeneration.Markdown;

public static class MarkdownGenerator
{
    private static readonly MarkdownFormat _markdownFormat = new(
        bulletListStyle: BulletListStyle.Minus,
        tableOptions: MarkdownFormat.Default.TableOptions | TableOptions.FormatContent,
        angleBracketEscapeStyle: AngleBracketEscapeStyle.EntityRef);

#pragma warning disable IDE0060, RCS1175
    private static void AddFootnote(this MDocument document)
    {
    }
#pragma warning restore IDE0060, RCS1175

    public static string CreateRefactoringsMarkdown(IEnumerable<RefactoringMetadata> refactorings, IComparer<string> comparer)
    {
        MDocument document = Document(
            Heading1("Refactorings"),
            Table(
                TableRow("Id", "Title", TableColumn(HorizontalAlignment.Center, "Enabled by Default")),
                refactorings.OrderBy(f => f.Id, comparer).Select(f =>
                {
                    return TableRow(
                        InlineCode(f.Id),
                        Link(f.Title.TrimEnd('.'), $"refactorings/{f.Id}.md"),
                        CheckboxOrHyphen(f.IsEnabledByDefault));
                })));

        document.AddFootnote();

        return document.ToString();
    }

    public static string CreateRefactoringMarkdown(RefactoringMetadata refactoring, int position)
    {
        MDocument document = Document(
            CreateFrontMatter(title: $"Refactoring: {refactoring.Title}", position: position, label: refactoring.Title),
            Heading1(refactoring.Title),
            Table(
                TableRow("Property", "Value"),
                TableRow("Id", InlineCode(refactoring.Id)),
                TableRow("Applicable Syntax", string.Join(", ", refactoring.Syntaxes.Select(f => f.Name))),
                (!string.IsNullOrEmpty(refactoring.Span)) ? TableRow("Syntax Span", refactoring.Span) : null,
                TableRow("Enabled by Default", CheckboxOrHyphen(refactoring.IsEnabledByDefault))),
            CreateSummary(refactoring.Summary),
            GetSamples(refactoring),
            CreateConfiguration(refactoring),
            CreateRemarks(refactoring.Remarks),
            CreateSeeAlso(refactoring));

        document.AddFootnote();

        return document.ToString(_markdownFormat);

        static IEnumerable<object> GetSamples(RefactoringMetadata refactoring)
        {
            if (refactoring.Samples.Count > 0)
            {
                yield return Heading2("Usage");

                foreach (MElement element in MarkdownGenerator.CreateSamples(refactoring.Samples, "before", "after"))
                    yield return element;
            }
        }

        static IEnumerable<MElement> CreateSeeAlso(RefactoringMetadata refactoring)
        {
            if (refactoring.Links.Any())
            {
                yield return Heading2("See Also");
                yield return BulletList(refactoring.Links.Select(f => CreateLink(f)));
            }
        }

        static IEnumerable<object> CreateConfiguration(RefactoringMetadata refactoring)
        {
            yield return Heading2("Configuration");

            yield return FencedCodeBlock(
                $"roslynator_refactoring.{refactoring.OptionKey}.enabled = true|false",
                "editorconfig");
        }
    }

    public static string CreateAnalyzersMarkdown(IEnumerable<AnalyzerMetadata> analyzers, string title, IComparer<string> comparer)
    {
        MDocument document = Document(
            Heading1(title),
            Heading2("Groups"),
            Table(
                TableRow("Prefix", "Comment"),
                TableRow(InlineCode("RCS1"), "common analyzers"),
                TableRow(InlineCode("RCS0"), "formatting analyzers"),
                TableRow(InlineCode("RCS9"), Inline("suitable for projects that reference Roslyn packages (", InlineCode("Microsoft.CodeAnalysis*"), ")"))
            ),
            Heading2("List of Analyzers"),
            Table(
                TableRow("Id", "Title", TableColumn(HorizontalAlignment.Center, "Default Severity")),
                analyzers.OrderBy(f => f.Id, comparer).Select(f =>
                {
                    return TableRow(
                        InlineCode(f.Id),
                        Link(f.Title.TrimEnd('.'), $"analyzers/{f.Id}.md"),
                        (f.IsEnabledByDefault) ? f.DefaultSeverity : "-");
                })));

        document.AddFootnote();

        return document.ToString();
    }

    public static string CreateAnalyzerMarkdown(AnalyzerMetadata analyzer, ImmutableArray<AnalyzerOptionMetadata> options)
    {
        MInlineCode[] requiredOptions = analyzer.ConfigOptions
            .Where(f => f.IsRequired)
            .Select(f => InlineCode(f.Key))
            .ToArray();

        string title = $"{analyzer.Id}: {analyzer.Title.TrimEnd('.')}";

        MDocument document = Document(
            CreateFrontMatter(
                title: title,
                label: (string.IsNullOrEmpty(analyzer.ObsoleteMessage)) ? analyzer.Id : $"[deprecated] {analyzer.Id}"),
            Heading1(title),
            CreateObsoleteWarning(analyzer),
            Heading2("Properties"),
            Table(
                TableRow("Property", "Value"),
                TableRow("Default Severity", (analyzer.IsEnabledByDefault) ? analyzer.DefaultSeverity : "disabled by default"),
                TableRow("Minimum language version", (!string.IsNullOrEmpty(analyzer.MinLanguageVersion)) ? InlineCode(analyzer.MinLanguageVersion) : "-")
            ),
            CreateSummary(analyzer.Summary),
            CreateSamples(analyzer),
            CreateConfiguration(analyzer, options),
            CreateRemarks(analyzer.Remarks),
            CreateAppliesTo(analyzer));

        document.AddFootnote();

        return document.ToString(_markdownFormat);

        static IEnumerable<MElement> CreateSamples(AnalyzerMetadata analyzer)
        {
            IReadOnlyList<SampleMetadata> samples = analyzer.Samples;

            if (samples.Count > 0)
            {
                yield return Heading2("Examples");

                foreach (MElement item in MarkdownGenerator.CreateSamples(samples, "diagnostic", "fix"))
                    yield return item;
            }
        }

        static IEnumerable<object> CreateConfiguration(AnalyzerMetadata analyzer, ImmutableArray<AnalyzerOptionMetadata> options)
        {
            IEnumerable<AnalyzerOptionMetadata> analyzerOptions = analyzer.ConfigOptions
                .Join(options, f => f.Key, f => f.Key, (_, g) => g)
                .OrderBy(f => f.Key);

            using (IEnumerator<AnalyzerOptionMetadata> en = analyzerOptions
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    yield return Heading2("Configuration");

                    MInlineCode[] requiredOptions = analyzer.ConfigOptions
                        .Where(f => f.IsRequired)
                        .Select(f => InlineCode(f.Key))
                        .ToArray();

                    if (requiredOptions.Any())
                        yield return CreateRequiredOptionsInfoBlock(requiredOptions);

                    var sb = new StringBuilder();
                    var isFirst = true;

                    do
                    {
                        if (!isFirst)
                        {
                            sb.AppendLine();
                            sb.AppendLine();
                        }

                        isFirst = false;

                        sb.Append("# ");

                        if (!string.IsNullOrEmpty(en.Current.Description))
                            sb.AppendLine(en.Current.Description);

                        string defaultValue = en.Current.DefaultValue;

                        if (!string.IsNullOrEmpty(defaultValue))
                        {
                            sb.Append("# Default value is ");
                            sb.AppendLine(defaultValue);
                        }

                        sb.Append(en.Current.Key);
                        sb.Append(" = ");
                        sb.Append(en.Current.DefaultValuePlaceholder ?? "true");
                    }
                    while (en.MoveNext());

                    yield return DocusaurusMarkdownFactory.CodeBlock(
                        sb.ToString(),
                        "editorconfig",
                        ".editorconfig");
                }
            }
        }

        static MContainer CreateRequiredOptionsInfoBlock(MInlineCode[] requiredOptions)
        {
            return DocusaurusMarkdownFactory.InfoBlock(CreateContent(requiredOptions));

            static IEnumerable<MObject> CreateContent(MInlineCode[] requiredOptions)
            {
                if (requiredOptions.Length == 1)
                {
                    yield return Inline("Option ", requiredOptions[0], " is required to be set for this analyzer to work.");
                }
                else
                {
                    yield return new MText($"One of the following options is required to be set for this analyzer to work: {NewLine}");

                    foreach (MInlineCode option in requiredOptions)
                        yield return BulletItem(option);
                }
            }
        }

        static IEnumerable<MElement> CreateAppliesTo(AnalyzerMetadata analyzer)
        {
            yield return Heading2("Applies to");

            if (!analyzer.Id.StartsWith("RCS9"))
            {
                yield return BulletItem(Link("Extension for VS 2022", "https://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2022"));
                yield return BulletItem(Link("Extension for VS Code", "https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator"));
                yield return BulletItem(Link("Extension for Open VSX", "https://open-vsx.org/extension/josefpihrt-vscode/roslynator"));
            }

            if (analyzer.Id.StartsWith("RCS0"))
                yield return BulletItem(Link(new[] { "Package ", "Roslynator.Formatting.Analyzers" }, "https://www.nuget.org/packages/Roslynator.Formatting.Analyzers"));

            if (analyzer.Id.StartsWith("RCS1"))
                yield return BulletItem(Link(new[] { "Package ", "Roslynator.Analyzers" }, "https://www.nuget.org/packages/Roslynator.Analyzers"));

            if (analyzer.Id.StartsWith("RCS9"))
                yield return BulletItem(Link(new[] { "Package ", "Roslynator.CodeAnalysis.Analyzers" }, "https://www.nuget.org/packages/Roslynator.CodeAnalysis.Analyzers"));
        }
    }

    private static DocusaurusCautionBlock CreateObsoleteWarning(AnalyzerMetadata analyzer)
    {
        string message = analyzer.ObsoleteMessage;

        if (message is null)
            return null;

        return new DocusaurusCautionBlock("This analyzer is obsolete. ", GetTextParts(), ".") { Title = "WARNING" };

        IEnumerable<object> GetTextParts()
        {
            int index = 0;
            Match match = Regex.Match(message, @"\bRCS\d\d\d\d\b");
            while (match.Success)
            {
                yield return message.Substring(index, match.Index);
                yield return Link(match.Value, $"/docs/roslynator/analyzers/{match.Value}");
                index = match.Index + match.Length;
                match = match.NextMatch();
            }

            yield return message.Substring(index);
        }
    }

    public static string CreateCodeFixesMarkdown(IEnumerable<CompilerDiagnosticMetadata> diagnostics, IComparer<string> comparer)
    {
        MDocument document = Document(
            Heading1("Code Fixes for Compiler Diagnostics"),
            Table(
                TableRow("Id", "Title"),
                GetRows()));

        document.AddFootnote();

        return document.ToString();

        IEnumerable<MTableRow> GetRows()
        {
            foreach (CompilerDiagnosticMetadata diagnostic in diagnostics
                .OrderBy(f => f.Id, comparer))
            {
                yield return TableRow(
                    Link(InlineCode(diagnostic.Id), $"fixes/{diagnostic.Id}.md"),
                    diagnostic.Title);
            }
        }
    }

    public static string CreateCodeFixMarkdown(
        CompilerDiagnosticMetadata diagnostic,
        IEnumerable<CodeFixMetadata> codeFixes,
        IEnumerable<CodeFixOption> options,
        IComparer<string> comparer)
    {
        MDocument document = Document(
            CreateFrontMatter(title: $"Code fix for {diagnostic.Id}", label: diagnostic.Id),
            Heading1(diagnostic.Id),
            Table(
                TableRow("Property", "Value"),
                TableRow("Title", diagnostic.Title),
                TableRow("Severity", diagnostic.Severity)),
            Heading2("Code Fixes"),
            BulletList(codeFixes
                .Where(f => f.FixableDiagnosticIds.Any(diagnosticId => diagnosticId == diagnostic.Id))
                .Select(f => f.Title)
                .OrderBy(f => f, comparer)),
            CreateConfiguration(),
            CreateSeeAlso(diagnostic));

        document.AddFootnote();

        return document.ToString(_markdownFormat);

        IEnumerable<MElement> CreateConfiguration()
        {
            string content = string.Join(
                NewLine,
                options
                    .Where(f => f.Key.Contains(diagnostic.Id))
                    .OrderBy(f => f.Key)
                    .Select(f => $"{f.Key} = {f.Value}"));

            if (!string.IsNullOrEmpty(content))
            {
                yield return Heading2("Configuration");

                yield return FencedCodeBlock(
                    content,
                    "editorconfig");
            }
        }

        static IEnumerable<MElement> CreateSeeAlso(CompilerDiagnosticMetadata diagnostic)
        {
            if (!string.IsNullOrEmpty(diagnostic.HelpUrl))
            {
                yield return Heading2("See Also");
                yield return BulletItem(Link("Official Documentation", diagnostic.HelpUrl));
            }
        }
    }

    private static IEnumerable<MElement> CreateSamples(
        IEnumerable<SampleMetadata> samples,
        string beforeTitle,
        string afterTitle)
    {
        int i = 1;
        foreach (SampleMetadata sample in samples)
        {
            yield return Heading3($"Example #{i}");

            ImmutableArray<(string Key, string Value)>.Enumerator en = sample.ConfigOptions.GetEnumerator();
            if (en.MoveNext())
            {
                var sb = new StringBuilder();
                var isFirst = true;

                do
                {
                    if (!isFirst)
                    {
                        sb.AppendLine();
                        sb.AppendLine();
                    }

                    isFirst = false;

                    string key = en.Current.Key;

                    if (!key.StartsWith("roslynator_", StringComparison.Ordinal))
                        key = "roslynator_" + key;

                    sb.Append(key);
                    sb.Append(" = ");
                    sb.Append(en.Current.Value);
                }
                while (en.MoveNext());

                yield return DocusaurusMarkdownFactory.CodeBlock(
                    sb.ToString(),
                    "editorconfig",
                    ".editorconfig");
            }

            yield return DocusaurusMarkdownFactory.CodeBlock(sample.Before, LanguageIdentifiers.CSharp, beforeTitle + ".cs");

            if (!string.IsNullOrEmpty(sample.After))
            {
                yield return DocusaurusMarkdownFactory.CodeBlock(sample.After, LanguageIdentifiers.CSharp, afterTitle + ".cs");
            }

            i++;
        }
    }

    private static MElement CreateLink(in LinkMetadata link)
    {
        if (string.IsNullOrEmpty(link.Text))
        {
            return new MAutolink(link.Url);
        }
        else
        {
            return Link(link.Text, link.Url, link.Title);
        }
    }

    private static IEnumerable<MElement> CreateSummary(string summary)
    {
        if (!string.IsNullOrEmpty(summary))
        {
            yield return Heading2("Summary");
            yield return Raw(summary);
        }
    }

    private static IEnumerable<MElement> CreateRemarks(string remarks)
    {
        if (!string.IsNullOrEmpty(remarks))
        {
            yield return Heading2("Remarks");
            yield return Raw(remarks);
        }
    }

    private static MElement CheckboxOrHyphen(bool value)
    {
        if (value)
        {
            return CharEntity((char)0x2713);
        }
        else
        {
            return new MText("-");
        }
    }

    private static DocusaurusFrontMatter CreateFrontMatter(string title = null, int? position = null, string label = null)
    {
        return DocusaurusMarkdownFactory.FrontMatter(GetLabels());

        IEnumerable<(string, object)> GetLabels()
        {
            if (title is not null)
                yield return ("title", title);

            if (position is not null)
                yield return ("sidebar_position", position);

            if (label is not null)
                yield return ("sidebar_label", label);
        }
    }
}
