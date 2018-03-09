// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using DotMarkdown;
using DotMarkdown.Linq;
using Roslynator.Metadata;
using static System.Environment;
using static DotMarkdown.Linq.MFactory;

namespace Roslynator.CodeGeneration.Markdown
{
    public static class MarkdownGenerator
    {
        private static void AddFootnote(this MDocument document)
        {
            document.Add(NewLine, Italic("(Generated with ", Link("DotMarkdown", "http://github.com/JosefPihrt/DotMarkdown"), ")"));
        }

        public static string CreateReadMe(IEnumerable<AnalyzerDescriptor> analyzers, IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
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

        public static string CreateRefactoringsMarkdown(IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            MDocument document = Document(
                Heading2("Roslynator Refactorings"),
                GetRefactorings());

            document.AddFootnote();

            return document.ToString();

            IEnumerable<object> GetRefactorings()
            {
                foreach (RefactoringDescriptor refactoring in refactorings.OrderBy(f => f.Title, comparer))
                {
                    yield return Heading4($"{refactoring.Title} ({refactoring.Id})");
                    yield return BulletItem(Bold("Syntax"), ": ", string.Join(", ", refactoring.Syntaxes.Select(f => f.Name)));

                    if (!string.IsNullOrEmpty(refactoring.Span))
                        yield return BulletItem(Bold("Span"), ": ", refactoring.Span);

                    foreach (object item in GetRefactoringSamples(refactoring))
                        yield return item;
                }
            }
        }

        private static IEnumerable<object> GetRefactoringSamples(RefactoringDescriptor refactoring)
        {
            if (refactoring.Samples.Count > 0)
            {
                foreach (MElement element in GetSamples(refactoring.Samples, Heading4("Before"), Heading4("After")))
                    yield return element;
            }
            else if (refactoring.Images.Count > 0)
            {
                bool isFirst = true;

                foreach (ImageDescriptor image in refactoring.Images)
                {
                    if (!isFirst)
                        yield return NewLine;

                    yield return RefactoringImage(refactoring, image.Name);
                    yield return NewLine;

                    isFirst = false;
                }

                yield return NewLine;
            }
            else
            {
                yield return RefactoringImage(refactoring, refactoring.Identifier);
                yield return NewLine;
                yield return NewLine;
            }
        }

        private static IEnumerable<MElement> GetSamples(IEnumerable<SampleDescriptor> samples, MHeading beforeHeader, MHeading afterHeader)
        {
            bool isFirst = true;

            foreach (SampleDescriptor sample in samples)
            {
                if (!isFirst)
                {
                    yield return HorizontalRule();
                }
                else
                {
                    isFirst = false;
                }

                yield return beforeHeader;
                yield return FencedCodeBlock(sample.Before, LanguageIdentifiers.CSharp);

                if (!string.IsNullOrEmpty(sample.After))
                {
                    yield return afterHeader;
                    yield return FencedCodeBlock(sample.After, LanguageIdentifiers.CSharp);
                }
            }
        }

        public static string CreateRefactoringMarkdown(RefactoringDescriptor refactoring)
        {
            var format = new MarkdownFormat(tableOptions: MarkdownFormat.Default.TableOptions | TableOptions.FormatContent);

            MDocument document = Document(
                Heading2(refactoring.Title),
                Table(TableRow("Property", "Value"),
                    TableRow("Id", refactoring.Id),
                    TableRow("Title", refactoring.Title),
                    TableRow("Syntax", string.Join(", ", refactoring.Syntaxes.Select(f => f.Name))),
                    (!string.IsNullOrEmpty(refactoring.Span)) ? TableRow("Span", refactoring.Span) : null,
                    TableRow("Enabled by Default", CheckboxOrHyphen(refactoring.IsEnabledByDefault))),
                (!string.IsNullOrEmpty(refactoring.Summary)) ? Raw(refactoring.Summary) : null,
                Heading3("Usage"),
                GetRefactoringSamples(refactoring),
                Link("full list of refactorings", "Refactorings.md"),
                NewLine);

            document.AddFootnote();

            return document.ToString(format);
        }

        public static string CreateAnalyzerMarkdown(AnalyzerDescriptor analyzer)
        {
            var format = new MarkdownFormat(tableOptions: MarkdownFormat.Default.TableOptions | TableOptions.FormatContent);

            MDocument document = Document(
                Heading1($"{((analyzer.IsObsolete) ? "[deprecated] " : "")}{analyzer.Id}: {analyzer.Title.TrimEnd('.')}"),
                Table(
                    TableRow("Property", "Value"),
                    TableRow("Id", analyzer.Id),
                    TableRow("Category", analyzer.Category),
                    TableRow("Default Severity", analyzer.DefaultSeverity),
                    TableRow("Enabled by Default", CheckboxOrHyphen(analyzer.IsEnabledByDefault)),
                    TableRow("Supports Fade-Out", CheckboxOrHyphen(analyzer.SupportsFadeOut)),
                    TableRow("Supports Fade-Out Analyzer", CheckboxOrHyphen(analyzer.SupportsFadeOutAnalyzer))),
                (!string.IsNullOrEmpty(analyzer.Summary)) ? Raw(analyzer.Summary) : null,
                Samples(),
                Heading2("How to Suppress"),
                Heading3("SuppressMessageAttribute"),
                FencedCodeBlock($"[assembly: SuppressMessage(\"{analyzer.Category}\", \"{analyzer.Id}:{analyzer.Title}\", Justification = \"<Pending>\")]", LanguageIdentifiers.CSharp),
                Heading3("#pragma"),
                FencedCodeBlock($"#pragma warning disable {analyzer.Id} // {analyzer.Title}\r\n#pragma warning restore {analyzer.Id} // {analyzer.Title}", LanguageIdentifiers.CSharp),
                Heading3("Ruleset"),
                BulletItem(Link("How to configure rule set", "../HowToConfigureAnalyzers.md")));

            document.AddFootnote();

            return document.ToString(format);

            IEnumerable<MElement> Samples()
            {
                ReadOnlyCollection<SampleDescriptor> samples = analyzer.Samples;

                if (samples.Count > 0)
                {
                    yield return Heading2((samples.Count == 1) ? "Example" : "Examples");

                    foreach (MElement item in GetSamples(samples, Heading3("Code with Diagnostic"), Heading3("Code with Fix")))
                        yield return item;
                }
            }
        }

        public static string CreateCompilerDiagnosticMarkdown(CompilerDiagnosticDescriptor diagnostic, IEnumerable<CodeFixDescriptor> codeFixes, IComparer<string> comparer)
        {
            MDocument document = Document(
                Heading1(diagnostic.Id),
                Table(
                    TableRow("Property", "Value"),
                    TableRow("Id", diagnostic.Id),
                    TableRow("Title", diagnostic.Title),
                    TableRow("Severity", diagnostic.Severity),
                    (!string.IsNullOrEmpty(diagnostic.HelpUrl)) ? TableRow("Official Documentation", Link("link", diagnostic.HelpUrl)) : null),
                Heading2("Code Fixes"),
                BulletList(codeFixes
                    .Where(f => f.FixableDiagnosticIds.Any(diagnosticId => diagnosticId == diagnostic.Id))
                    .Select(f => f.Title)
                    .OrderBy(f => f, comparer)));

            document.AddFootnote();

            return document.ToString(MarkdownFormat.Default.WithTableOptions(MarkdownFormat.Default.TableOptions | TableOptions.FormatContent));
        }

        public static string CreateAnalyzersReadMe(IEnumerable<AnalyzerDescriptor> analyzers, IComparer<string> comparer)
        {
            MDocument document = Document(
                Heading2("Roslynator Analyzers"),
                Link("Search Analyzers", "http://pihrt.net/Roslynator/Analyzers"),
                Table(
                    TableRow("Id", "Title", "Category", TableColumn(HorizontalAlignment.Center, "Enabled by Default")),
                    analyzers.OrderBy(f => f.Id, comparer).Select(f =>
                    {
                        return TableRow(
                            f.Id,
                            Link(f.Title.TrimEnd('.'), $"../../docs/analyzers/{f.Id}.md"),
                            f.Category,
                            CheckboxOrHyphen(f.IsEnabledByDefault));
                    })));

            document.AddFootnote();

            return document.ToString();
        }

        public static string CreateRefactoringsReadMe(IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            MDocument document = Document(
                Heading2("Roslynator Refactorings"),
                Link("Search Refactorings", "http://pihrt.net/Roslynator/Refactorings"),
                Table(
                    TableRow("Id", "Title", TableColumn(HorizontalAlignment.Center, "Enabled by Default")),
                    refactorings.OrderBy(f => f.Title, comparer).Select(f =>
                    {
                        return TableRow(
                        f.Id,
                        Link(f.Title.TrimEnd('.'), $"../../docs/refactorings/{f.Id}.md"),
                        CheckboxOrHyphen(f.IsEnabledByDefault));
                    })));

            document.AddFootnote();

            return document.ToString();
        }

        public static string CreateCodeFixesReadMe(IEnumerable<CompilerDiagnosticDescriptor> diagnostics, IComparer<string> comparer)
        {
            MDocument document = Document(
                Heading2("Compiler Diagnostics Fixable with Roslynator"),
                Table(
                    TableRow("Id", "Title"),
                    GetRows()));

            document.AddFootnote();

            return document.ToString();

            IEnumerable<MTableRow> GetRows()
            {
                foreach (CompilerDiagnosticDescriptor diagnostic in diagnostics
                    .OrderBy(f => f.Id, comparer))
                {
                    yield return TableRow(
                        Link(diagnostic.Id, $"../../docs/cs/{diagnostic.Id}.md"),
                        diagnostic.Title);
                }
            }
        }

        public static string CreateAnalyzersByCategoryMarkdown(IEnumerable<AnalyzerDescriptor> analyzers, IComparer<string> comparer)
        {
            MDocument document = Document(
                Heading2("Roslynator Analyzers by Category"),
                Table(
                    TableRow("Category", "Title", "Id", TableColumn(HorizontalAlignment.Center, "Enabled by Default")),
                    GetRows()));

            document.AddFootnote();

            return document.ToString();

            IEnumerable<MTableRow> GetRows()
            {
                foreach (IGrouping<string, AnalyzerDescriptor> grouping in analyzers
                    .GroupBy(f => MarkdownEscaper.Escape(f.Category))
                    .OrderBy(f => f.Key, comparer))
                {
                    foreach (AnalyzerDescriptor analyzer in grouping.OrderBy(f => f.Title, comparer))
                    {
                        yield return TableRow(
                            grouping.Key,
                            Link(analyzer.Title.TrimEnd('.'), $"../../docs/analyzers/{analyzer.Id}.md"),
                            analyzer.Id,
                            CheckboxOrHyphen(analyzer.IsEnabledByDefault));
                    }
                }
            }
        }

        private static MImage RefactoringImage(RefactoringDescriptor refactoring, string fileName)
        {
            return Image(refactoring.Title, $"../../images/refactorings/{fileName}.png");
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
    }
}
