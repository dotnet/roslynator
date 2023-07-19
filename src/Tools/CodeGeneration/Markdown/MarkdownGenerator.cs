// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using DotMarkdown;
using DotMarkdown.Linq;
using Roslynator.Metadata;
using static System.Environment;
using static DotMarkdown.Linq.MFactory;

namespace Roslynator.CodeGeneration.Markdown;

public static class MarkdownGenerator
{
    private static readonly MarkdownFormat _defaultMarkdownFormat = new(
        tableOptions: MarkdownFormat.Default.TableOptions | TableOptions.FormatContent,
        angleBracketEscapeStyle: AngleBracketEscapeStyle.EntityRef);

#pragma warning disable IDE0060, RCS1175
    private static void AddFootnote(this MDocument document)
    {
    }
#pragma warning restore IDE0060, RCS1175

    public static string CreateReadMe(IEnumerable<AnalyzerMetadata> analyzers, IEnumerable<RefactoringMetadata> refactorings, IComparer<string> comparer)
    {
        MDocument document = Document(
            Heading3("List of Analyzers"),
            analyzers
                .OrderBy(f => f.Id, comparer)
                .Select(analyzer => BulletItem(analyzer.Id, " - ", Link(analyzer.Title.TrimEnd('.'), $"docs/analyzers/{analyzer.Id}.md"))),
            Heading3("List of Refactorings"),
            refactorings
                .OrderBy(f => f.Title, comparer)
                .Select(refactoring => BulletItem(Link(refactoring.Title.TrimEnd('.'), $"docs/refactorings/{refactoring.Id}.md"))));

        document.AddFootnote();

        return File.ReadAllText(@"..\text\ReadMe.txt", Encoding.UTF8)
            + NewLine
            + document;
    }

    private static IEnumerable<object> GetRefactoringSamples(RefactoringMetadata refactoring)
    {
        if (refactoring.Samples.Count > 0)
        {
            yield return Heading2("Usage");

            foreach (MElement element in GetSamples(refactoring.Samples, Heading4("Before"), Heading4("After")))
                yield return element;
        }

        //TODO: Add example to refactoring documentation
        //else if (refactoring.Images.Count > 0)
        //{
        //    var isFirst = true;

        //    foreach (ImageMetadata image in refactoring.Images)
        //    {
        //        if (!isFirst)
        //            yield return NewLine;

        //        yield return RefactoringImage(refactoring, image.Name);
        //        yield return NewLine;

        //        isFirst = false;
        //    }

        //    yield return NewLine;
        //}
        //else
        //{
        //    yield return RefactoringImage(refactoring, refactoring.Identifier);
        //    yield return NewLine;
        //    yield return NewLine;
        //}
    }

    private static IEnumerable<MElement> GetSamples(
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

    private static IEnumerable<MElement> GetAnalyzerSamples(AnalyzerMetadata analyzer)
    {
        return GetAnalyzerSamples(analyzer.Samples, analyzer.Kind);
    }

    private static IEnumerable<MElement> GetAnalyzerSamples(IReadOnlyList<SampleMetadata> samples, AnalyzerOptionKind kind)
    {
        if (samples.Count > 0)
        {
            yield return Heading2((samples.Count == 1) ? "Example" : "Examples");

            string beforeHeading = (kind == AnalyzerOptionKind.Disable)
                ? "Code"
                : "Code with Diagnostic";

            foreach (MElement item in GetSamples(samples, Heading3(beforeHeading), Heading3("Code with Fix")))
                yield return item;
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

    public static string CreateRefactoringMarkdown(RefactoringMetadata refactoring)
    {
        MDocument document = Document(
            CreateFrontMatter(label: refactoring.Id),
            Heading1(refactoring.Title),
            Table(
                TableRow("Property", "Value"),
                TableRow("Id", InlineCode(refactoring.Id)),
                TableRow("Title", refactoring.Title),
                TableRow("Syntax", string.Join(", ", refactoring.Syntaxes.Select(f => f.Name))),
                (!string.IsNullOrEmpty(refactoring.Span)) ? TableRow("Span", refactoring.Span) : null,
                TableRow("Enabled by Default", CheckboxOrHyphen(refactoring.IsEnabledByDefault))),
            CreateSummary(refactoring.Summary),
            GetRefactoringSamples(refactoring),
            CreateRemarks(refactoring.Remarks),
            CreateSeeAlso(refactoring));

        document.AddFootnote();

        return document.ToString(_defaultMarkdownFormat);

        static IEnumerable<MElement> CreateSeeAlso(RefactoringMetadata refactoring)
        {
            if (refactoring.Links.Any())
            {
                yield return Heading2("See Also");
                yield return BulletList(refactoring.Links.Select(f => CreateLink(f)));
            }
        }
    }

    public static string CreateAnalyzerMarkdown(AnalyzerMetadata analyzer, ImmutableArray<ConfigOptionMetadata> options, IEnumerable<(string title, string url)> appliesTo = null)
    {
        IEnumerable<MInlineCode> requiredOptions = analyzer.ConfigOptions
            .Where(f => f.IsRequired)
            .Select(f => InlineCode(f.Key));

        MDocument document = Document(
            CreateFrontMatter(label: analyzer.Id),
            Heading1($"{((analyzer.IsObsolete) ? "[deprecated] " : "")}{analyzer.Id}: {analyzer.Title.TrimEnd('.')}"),
            Table(
                TableRow("Property", "Value"),
                TableRow("Id", InlineCode(analyzer.Id)),
                TableRow("Severity", (analyzer.IsEnabledByDefault) ? analyzer.DefaultSeverity : "None"),
                (!string.IsNullOrEmpty(analyzer.MinLanguageVersion)) ? TableRow("Minimum language version", analyzer.MinLanguageVersion) : null,
                (requiredOptions.Any()) ? TableRow("Required option", Join(" or ", requiredOptions)) : null
            ),
            CreateSummary(analyzer.Summary),
            GetAnalyzerSamples(analyzer),
            CreateOptions(analyzer, options),
            CreateRemarks(analyzer.Remarks),
            CreateAppliesTo(appliesTo));

        document.AddFootnote();

        return document.ToString(_defaultMarkdownFormat);
    }

    public static string CreateAnalyzerOptionMarkdown(AnalyzerOptionMetadata option)
    {
        string id = option.ParentId + option.Id;

        string optionKey = option.OptionKey;

        if (!optionKey.StartsWith("roslynator."))
            optionKey = $"roslynator.{option.ParentId}." + optionKey;

        string optionValue = option.OptionValue ?? "true";

        MDocument document = Document(
            Heading1($"[deprecated] {id}: {option.Title.TrimEnd('.')}"),
            Bold($"Option {id} is obsolete, use EditorConfig option instead:"),
            FencedCodeBlock($"{optionKey} = {optionValue}"),
            GetAnalyzerSamples(option.Samples, option.Kind));

        document.AddFootnote();

        return document.ToString(_defaultMarkdownFormat);
    }

    private static IEnumerable<MElement> CreateAppliesTo(IEnumerable<(string title, string url)> appliesTo)
    {
        if (appliesTo is not null)
        {
            yield return Heading2("Applies to");
            yield return BulletList(appliesTo.Select(f => LinkOrText(f.title, f.url)));
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
                TableRow("Id", InlineCode(diagnostic.Id)),
                TableRow("Title", diagnostic.Title),
                TableRow("Severity", diagnostic.Severity),
                (!string.IsNullOrEmpty(diagnostic.HelpUrl)) ? TableRow("Official Documentation", Link("link", diagnostic.HelpUrl)) : null),
            Heading2("Code Fixes"),
            BulletList(codeFixes
                .Where(f => f.FixableDiagnosticIds.Any(diagnosticId => diagnosticId == diagnostic.Id))
                .Select(f => f.Title)
                .OrderBy(f => f, comparer)),
            GetOptions());

        document.AddFootnote();

        return document.ToString(_defaultMarkdownFormat);

        IEnumerable<MElement> GetOptions()
        {
            string content = string.Join(
                NewLine,
                options
                    .Where(f => f.Key.Contains(diagnostic.Id))
                    .OrderBy(f => f.Key)
                    .Select(f => $"{f.Key} = {f.Value}"));

            if (!string.IsNullOrEmpty(content))
            {
                yield return Heading2("Options");

                yield return FencedCodeBlock(
                    content,
                    "editorconfig");
            }
        }
    }

    public static string CreateAnalyzersMarkdown(IEnumerable<AnalyzerMetadata> analyzers, string title, IComparer<string> comparer)
    {
        MDocument document = Document(
            Heading1(title),
            Heading2("Overview"),
            Table(
                TableRow("Package", "Prefix", "Comment"),
                TableRow(Link("Roslynator.Analyzers", "https://www.nuget.org/packages/Roslynator.Analyzers"), InlineCode("RCS1"), "common analyzers"),
                TableRow(Link("Roslynator.Formatting.Analyzers", "https://www.nuget.org/packages/Roslynator.Formatting.Analyzers"), InlineCode("RCS0"), "-"),
                TableRow(
                    Link("Roslynator.CodeAnalysis.Analyzers", "https://www.nuget.org/packages/Roslynator.CodeAnalysis.Analyzers"),
                    InlineCode("RCS9"),
                    Inline("suitable for projects that reference Roslyn packages (", InlineCode("Microsoft.CodeAnalysis*"), ")"))
            ),
            Heading2("List of Analyzers"),
            Table(
                TableRow("Id", "Title", "Severity"),
                analyzers.OrderBy(f => f.Id, comparer).Select(f =>
                {
                    return TableRow(
                        Link(InlineCode(f.Id), $"analyzers/{f.Id}.md"),
                        f.Title.TrimEnd('.'),
                        (f.IsEnabledByDefault) ? f.DefaultSeverity : "None");
                })));

        document.AddFootnote();

        return document.ToString();
    }

    public static string CreateRefactoringsMarkdown(IEnumerable<RefactoringMetadata> refactorings, IComparer<string> comparer)
    {
        MDocument document = Document(
            Heading1("Refactorings"),
            Table(
                TableRow("Id", "Title", TableColumn(HorizontalAlignment.Center, "Enabled by Default")),
                refactorings.OrderBy(f => f.Id, comparer).Select(f =>
                {
                    return TableRow(
                        Link(InlineCode(f.Id), $"refactorings/{f.Id}.md"),
                        f.Title.TrimEnd('.'),
                        CheckboxOrHyphen(f.IsEnabledByDefault));
                })));

        document.AddFootnote();

        return document.ToString();
    }

    public static string CreateCodeFixesReadMe(IEnumerable<CompilerDiagnosticMetadata> diagnostics, IComparer<string> comparer)
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

    private static IEnumerable<MElement> CreateSummary(string summary)
    {
        if (!string.IsNullOrEmpty(summary))
        {
            yield return Heading2("Summary");
            yield return Raw(summary);
        }
    }

    private static IEnumerable<MElement> CreateOptions(AnalyzerMetadata analyzer, ImmutableArray<ConfigOptionMetadata> options)
    {
        IEnumerable<ConfigOptionMetadata> analyzerOptions = analyzer.ConfigOptions
            .Join(options, f => f.Key, f => f.Key, (_, g) => g)
            .OrderBy(f => f.Key);

        using (IEnumerator<ConfigOptionMetadata> en = analyzerOptions
            .GetEnumerator())
        {
            if (en.MoveNext())
            {
                yield return Heading2("Options");

                do
                {
                    string optionKey = en.Current.Key;
                    string title = en.Current.Description;
                    const string summary = null;
                    string defaultValue = en.Current.DefaultValuePlaceholder;

                    yield return Heading3(title?.TrimEnd('.'));

                    if (!string.IsNullOrEmpty(summary))
                    {
                        yield return new MText(summary);
                        yield return new MText(NewLine);
                    }

                    string helpValue = optionKey;

                    helpValue += " = ";
                    helpValue += defaultValue ?? "true";

                    yield return FencedCodeBlock(
                        helpValue,
                        "editorconfig");
                }
                while (en.MoveNext());
            }
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
        if (position is not null
            || label is not null)
        {
            return Raw(DocusaurusUtility.CreateFrontMatter(position, label));
        }

        return null;
    }
}
