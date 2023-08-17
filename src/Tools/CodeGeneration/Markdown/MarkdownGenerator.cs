// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
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
            CreateFrontMatter(position: position, label: refactoring.Title),
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

                foreach (MElement element in MarkdownGenerator.CreateSamples(refactoring.Samples, Heading4("Before"), Heading4("After")))
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

    public static string CreateAnalyzerMarkdown(AnalyzerMetadata analyzer, ImmutableArray<ConfigOptionMetadata> options)
    {
        MInlineCode[] requiredOptions = analyzer.ConfigOptions
            .Where(f => f.IsRequired)
            .Select(f => InlineCode(f.Key))
            .ToArray();

        string title = analyzer.Title.TrimEnd('.');

        MDocument document = Document(
            CreateFrontMatter(label: analyzer.Id),
            Heading1($"{((analyzer.IsObsolete) ? "[deprecated] " : "")}{analyzer.Id}: {title}"),
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
            AnalyzerOptionKind kind = analyzer.Kind;

            if (samples.Count > 0)
            {
                yield return Heading2((samples.Count == 1) ? "Example" : "Examples");

                string beforeHeading = (kind == AnalyzerOptionKind.Disable)
                    ? "Code"
                    : "Code with Diagnostic";

                foreach (MElement item in MarkdownGenerator.CreateSamples(samples, Heading3(beforeHeading), Heading3("Code with Fix")))
                    yield return item;
            }
        }

        static IEnumerable<object> CreateConfiguration(AnalyzerMetadata analyzer, ImmutableArray<ConfigOptionMetadata> options)
        {
            IEnumerable<ConfigOptionMetadata> analyzerOptions = analyzer.ConfigOptions
                .Join(options, f => f.Key, f => f.Key, (_, g) => g)
                .OrderBy(f => f.Key);

            using (IEnumerator<ConfigOptionMetadata> en = analyzerOptions
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    yield return Heading2("Configuration");

                    MInlineCode[] requiredOptions = analyzer.ConfigOptions
                        .Where(f => f.IsRequired)
                        .Select(f => InlineCode(f.Key))
                        .ToArray();

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

                        sb.Append('#');
                        sb.AppendLine(en.Current.Description);
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
                if (!requiredOptions.Any())
                    yield break;

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
                yield return BulletItem(Link("Roslynator.Formatting.Analyzers", "https://www.nuget.org/packages/Roslynator.Formatting.Analyzers"));

            if (analyzer.Id.StartsWith("RCS1"))
                yield return BulletItem(Link("Roslynator.Analyzers", "https://www.nuget.org/packages/Roslynator.Analyzers"));

            if (analyzer.Id.StartsWith("RCS9"))
                yield return BulletItem(Link("Roslynator.CodeAnalysis.Analyzers", "https://www.nuget.org/packages/Roslynator.CodeAnalysis.Analyzers"));
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
        ImmutableArray<CodeFixOption> options,
        IComparer<string> comparer)
    {
        MDocument document = Document(
            CreateFrontMatter(label: diagnostic.Id),
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
        MHeading beforeHeading,
        MHeading afterHeading)
    {
        using (IEnumerator<SampleMetadata> en = samples.GetEnumerator())
        {
            if (en.MoveNext())
            {
                while (true)
                {
                    yield return beforeHeading;
                    yield return FencedCodeBlock(en.Current.Before, LanguageIdentifiers.CSharp);

                    if (!string.IsNullOrEmpty(en.Current.After))
                    {
                        yield return afterHeading;
                        yield return FencedCodeBlock(en.Current.After, LanguageIdentifiers.CSharp);
                    }

                    if (en.MoveNext())
                    {
                        yield return HorizontalRule();
                    }
                    else
                    {
                        break;
                    }
                }
            }
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

    private static MObject CreateFrontMatter(int? position = null, string label = null)
    {
        var labels = new List<(string, object)>();

        if (position is not null)
            labels.Add(("sidebar_position", position));

        if (label is not null)
            labels.Add(("sidebar_label", label));

        return DocusaurusMarkdownFactory.FrontMatter(labels);
    }
}
