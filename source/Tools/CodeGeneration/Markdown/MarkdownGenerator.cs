// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Roslynator.Metadata;
using Roslynator.Markdown;
using static Roslynator.Markdown.MarkdownFactory;

namespace Roslynator.CodeGeneration.Markdown
{
    public static class MarkdownGenerator
    {
        public static string CreateReadMe(IEnumerable<AnalyzerDescriptor> analyzers, IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            var mb = new MarkdownBuilder();

            mb.AppendLineRaw(File.ReadAllText(@"..\text\ReadMe.txt", Encoding.UTF8));

            mb.AppendHeader3("List of Analyzers");

            foreach (AnalyzerDescriptor analyzer in analyzers.OrderBy(f => f.Id, comparer))
            {
                mb.AppendListItem(analyzer.Id, " - ", Link(analyzer.Title.TrimEnd('.'), $"docs/analyzers/{analyzer.Id}.md"));
            }

            mb.AppendHeader3("List of Refactorings");

            foreach (RefactoringDescriptor refactoring in refactorings.OrderBy(f => f.Title, comparer))
            {
                mb.AppendListItem(Link(refactoring.Title.TrimEnd('.'), $"docs/refactorings/{refactoring.Id}.md"));
            }

            return mb.ToString();
        }

        public static string CreateRefactoringsMarkdown(IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            var mb = new MarkdownBuilder();

            mb.Append(Header2("Roslynator Refactorings"));

            foreach (RefactoringDescriptor refactoring in refactorings.OrderBy(f => f.Title, comparer))
            {
                mb.AppendHeader4($"{refactoring.Title} ({refactoring.Id})");
                mb.AppendListItem(Bold("Syntax"), ": ", Join(", ", refactoring.Syntaxes.Select(f => f.Name)));

                if (!string.IsNullOrEmpty(refactoring.Span))
                    mb.AppendListItem(Bold("Span"), ": ", refactoring.Span);

                AppendRefactoringSamples(mb, refactoring);
            }

            return mb.ToString();
        }

        private static void AppendRefactoringSamples(MarkdownBuilder mb, RefactoringDescriptor refactoring)
        {
            if (refactoring.Samples.Count > 0)
            {
                AppendSamples(mb, refactoring.Samples, Header4("Before"), Header4("After"));
            }
            else if (refactoring.Images.Count > 0)
            {
                bool isFirst = true;

                foreach (ImageDescriptor image in refactoring.Images)
                {
                    if (!isFirst)
                        mb.AppendLine();

                    AppendRefactoringImage(mb, refactoring, image.Name);
                    isFirst = false;
                }

                mb.AppendLine();
            }
            else
            {
                AppendRefactoringImage(mb, refactoring, refactoring.Identifier);
                mb.AppendLine();
            }
        }

        private static void AppendSamples(MarkdownBuilder mb, IEnumerable<SampleDescriptor> samples, Header beforeHeader, Header afterHeader)
        {
            bool isFirst = true;

            foreach (SampleDescriptor sample in samples)
            {
                if (!isFirst)
                {
                    mb.AppendHorizonalRule();
                }
                else
                {
                    isFirst = false;
                }

                mb.Append(beforeHeader);
                mb.AppendCSharpCodeBlock(sample.Before);

                if (!string.IsNullOrEmpty(sample.After))
                {
                    mb.Append(afterHeader);
                    mb.AppendCSharpCodeBlock(sample.After);
                }
            }
        }

        public static string CreateRefactoringMarkdown(RefactoringDescriptor refactoring)
        {
            var mb = new MarkdownBuilder(new MarkdownSettings(tableFormatting: TableFormatting.All));

            mb.AppendHeader2(refactoring.Title);

            Table("Property", "Value")
                .AddRow("Id", refactoring.Id)
                .AddRow("Title", refactoring.Title)
                .AddRow("Syntax", string.Join(", ", refactoring.Syntaxes.Select(f => f.Name)))
                .AddRowIf(!string.IsNullOrEmpty(refactoring.Span), "Span", refactoring.Span)
                .AddRow("Enabled by Default", RawText(GetBooleanAsText(refactoring.IsEnabledByDefault)))
                .AppendTo(mb);

            mb.AppendHeader3("Usage");

            AppendRefactoringSamples(mb, refactoring);

            mb.AppendLink("full list of refactorings", "Refactorings.md");

            return mb.ToString();
        }

        public static string CreateAnalyzerMarkdown(AnalyzerDescriptor analyzer)
        {
            var mb = new MarkdownBuilder(new MarkdownSettings(tableFormatting: TableFormatting.All));

            mb.AppendHeader1($"{((analyzer.IsObsolete) ? "[deprecated] " : "")}{analyzer.Id}: {analyzer.Title.TrimEnd('.')}");

            Table table = Table("Property", "Value")
                .AddRow("Id", analyzer.Id)
                .AddRow("Category", analyzer.Category)
                .AddRow("Default Severity", analyzer.DefaultSeverity)
                .AddRow("Enabled by Default", RawText(GetBooleanAsText(analyzer.IsEnabledByDefault)))
                .AddRow("Supports Fade-Out", RawText(GetBooleanAsText(analyzer.SupportsFadeOut)))
                .AddRow("Supports Fade-Out Analyzer", RawText(GetBooleanAsText(analyzer.SupportsFadeOutAnalyzer)));

            mb.Append(table);

            ReadOnlyCollection<SampleDescriptor> samples = analyzer.Samples;

            if (samples.Count > 0)
            {
                mb.AppendHeader2((samples.Count == 1) ? "Example" : "Examples");

                AppendSamples(mb, samples, Header3("Code with Diagnostic"), Header3("Code with Fix"));
            }

            mb.AppendHeader2("How to Suppress");

            mb.AppendHeader3("SuppressMessageAttribute");

            mb.AppendCSharpCodeBlock($"[assembly: SuppressMessage(\"{analyzer.Category}\", \"{analyzer.Id}:{analyzer.Title}\", Justification = \"<Pending>\")]");

            mb.AppendHeader3("#pragma");

            mb.AppendCSharpCodeBlock($"#pragma warning disable {analyzer.Id} // {analyzer.Title}\r\n#pragma warning restore {analyzer.Id} // {analyzer.Title}");

            mb.AppendHeader3("Ruleset");

            mb.AppendListItem(Link("How to configure rule set", "../HowToConfigureAnalyzers.md"));

            return mb.ToString();
        }

        public static string CreateAnalyzersReadMe(IEnumerable<AnalyzerDescriptor> analyzers, IComparer<string> comparer)
        {
            var mb = new MarkdownBuilder();

            mb.AppendHeader2("Roslynator Analyzers");

            Table table = Table("Id", "Title", "Category", TableHeader("Enabled by Default", Alignment.Center));

            foreach (AnalyzerDescriptor analyzer in analyzers.OrderBy(f => f.Id, comparer))
            {
                table.AddRow(
                    analyzer.Id,
                    Link(analyzer.Title.TrimEnd('.'), $"../../docs/analyzers/{analyzer.Id}.md"),
                    analyzer.Category,
                    RawText((analyzer.IsEnabledByDefault) ? "&#x2713;" : ""));
            }

            mb.Append(table);

            return mb.ToString();
        }

        public static string CreateRefactoringsReadMe(IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            var mb = new MarkdownBuilder();

            mb.AppendHeader2("Roslynator Refactorings");

            Table table = Table("Id", "Title", TableHeader("Enabled by Default", Alignment.Center));

            foreach (RefactoringDescriptor refactoring in refactorings.OrderBy(f => f.Title, comparer))
            {
                table.AddRow(
                    refactoring.Id,
                    Link(refactoring.Title.TrimEnd('.'), $"../../docs/refactorings/{refactoring.Id}.md"),
                    RawText((refactoring.IsEnabledByDefault) ? "&#x2713;" : ""));
            }

            mb.Append(table);

            return mb.ToString();
        }

        public static string CreateCodeFixesReadMe(IEnumerable<CodeFixDescriptor> codeFixes, IEnumerable<CompilerDiagnosticDescriptor> diagnostics, IComparer<string> comparer)
        {
            var mb = new MarkdownBuilder();

            mb.AppendHeader2("Roslynator Code Fixes");

            Table table = Table("Id", "Title", "Fixable Diagnostics", TableHeader("Enabled by Default", Alignment.Center));

            foreach (CodeFixDescriptor codeFix in codeFixes.OrderBy(f => f.Title, comparer))
            {
                table.AddRow(
                    codeFix.Id,
                    codeFix.Title.TrimEnd('.'),
                    Join(", ", codeFix.FixableDiagnosticIds.Join(diagnostics, f => f, f => f.Id, (f, g) => (object)Link(f, g.HelpUrl))),
                    RawText((codeFix.IsEnabledByDefault) ? "&#x2713;" : ""));
            }

            mb.Append(table);

            return mb.ToString();
        }

        public static string CreateCodeFixesByDiagnosticId(IEnumerable<CodeFixDescriptor> codeFixes, IEnumerable<CompilerDiagnosticDescriptor> diagnostics)
        {
            var mb = new MarkdownBuilder();

            mb.AppendHeader2("Roslynator Code Fixes by Diagnostic Id");

            Table table = Table("Diagnostic", "Code Fixes");

            foreach (var grouping in codeFixes
                .SelectMany(codeFix => codeFix.FixableDiagnosticIds.Select(diagnosticId => new { DiagnosticId = diagnosticId, CodeFixDescriptor = codeFix }))
                .OrderBy(f => f.DiagnosticId)
                .ThenBy(f => f.CodeFixDescriptor.Id)
                .GroupBy(f => f.DiagnosticId))
            {
                CompilerDiagnosticDescriptor diagnostic = diagnostics.FirstOrDefault(f => f.Id == grouping.Key);

                table.AddRow(
                    Link(diagnostic?.Id ?? grouping.Key, diagnostic?.HelpUrl),
                    Join(", ", grouping.Select(f => f.CodeFixDescriptor.Id)));
            }

            mb.Append(table);

            return mb.ToString();
        }

        public static string CreateAnalyzersByCategoryMarkdown(IEnumerable<AnalyzerDescriptor> analyzers, IComparer<string> comparer)
        {
            var mb = new MarkdownBuilder();

            mb.AppendHeader2("Roslynator Analyzers by Category");

            Table table = Table("Category", "Title", "Id", TableHeader("Enabled by Default", Alignment.Center));

            foreach (IGrouping<string, AnalyzerDescriptor> grouping in analyzers
                .GroupBy(f => MarkdownEscaper.Escape(f.Category, mb.Settings.ShouldBeEscaped))
                .OrderBy(f => f.Key, comparer))
            {
                foreach (AnalyzerDescriptor analyzer in grouping.OrderBy(f => f.Title, comparer))
                {
                    table.AddRow(
                        grouping.Key,
                        Link(analyzer.Title.TrimEnd('.'), $"../../docs/analyzers/{analyzer.Id}.md"),
                        analyzer.Id,
                        RawText((analyzer.IsEnabledByDefault) ? "&#x2713;" : ""));
                }
            }

            mb.Append(table);

            return mb.ToString();
        }

        private static void AppendRefactoringImage(MarkdownBuilder mb, RefactoringDescriptor refactoring, string fileName)
        {
            mb.AppendImage(refactoring.Title, $"../../images/refactorings/{fileName}.png");
            mb.AppendLine();
        }

        private static string GetBooleanAsText(bool value)
        {
            return (value) ? "&#x2713;" : "-";
        }
    }
}
