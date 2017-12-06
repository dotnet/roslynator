// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Roslynator.Metadata;
using Roslynator.Utilities.Markdown;

namespace Roslynator.CodeGeneration.Markdown
{
    public static class MarkdownGenerator
    {
        public static string CreateReadMe(IEnumerable<AnalyzerDescriptor> analyzers, IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            var sb = new StringBuilder();

            sb.AppendLine(File.ReadAllText(@"..\text\ReadMe.txt", Encoding.UTF8));

            sb.AppendHeader3("List of Analyzers");
            sb.AppendLine();

            foreach (AnalyzerDescriptor analyzer in analyzers.OrderBy(f => f.Id, comparer))
            {
                sb.AppendUnorderedListItem();
                sb.Append(analyzer.Id);
                sb.Append(" - ");
                sb.AppendLink(analyzer.Title.TrimEnd('.'), $"docs/analyzers/{analyzer.Id}.md");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendHeader3("List of Refactorings");
            sb.AppendLine();

            foreach (RefactoringDescriptor refactoring in refactorings.OrderBy(f => f.Title, comparer))
            {
                sb.AppendUnorderedListItem();
                sb.AppendLink(refactoring.Title.TrimEnd('.'), $"docs/refactorings/{refactoring.Id}.md");
            }

            return sb.ToString();
        }

        public static string CreateRefactoringsMarkdown(IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            var sb = new StringBuilder();

            sb.AppendHeader2("Roslynator Refactorings");

            foreach (RefactoringDescriptor refactoring in refactorings
                .OrderBy(f => f.Title, comparer))
            {
                sb.AppendLine();
                sb.AppendHeader4($"{refactoring.Title} ({refactoring.Id})");
                sb.AppendLine();
                sb.AppendUnorderedListItem();
                sb.AppendBold("Syntax");
                sb.Append(": ");

                bool isFirst = true;

                foreach (SyntaxDescriptor syntax in refactoring.Syntaxes)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    sb.Append(syntax.Name.EscapeMarkdown());
                }

                sb.AppendLine();

                if (!string.IsNullOrEmpty(refactoring.Span))
                {
                    sb.AppendUnorderedListItem();
                    sb.AppendBold("Span");
                    sb.Append(": ");
                    sb.AppendLine(refactoring.Span.EscapeMarkdown());
                }

                sb.AppendLine();

                WriteRefactoringSamples(sb, refactoring);
            }

            return sb.ToString();
        }

        private static void WriteRefactoringSamples(StringBuilder sb, RefactoringDescriptor refactoring)
        {
            if (refactoring.Samples.Count > 0)
            {
                WriteSamples(sb, refactoring.Samples, new MarkdownHeader("Before", 4), new MarkdownHeader("After", 4));
            }
            else if (refactoring.Images.Count > 0)
            {
                bool isFirst = true;

                foreach (ImageDescriptor image in refactoring.Images)
                {
                    if (!isFirst)
                        sb.AppendLine();

                    AppendRefactoringImage(sb, refactoring, image.Name);
                    isFirst = false;
                }
            }
            else
            {
                AppendRefactoringImage(sb, refactoring, refactoring.Identifier);
            }
        }

        private static void WriteSamples(StringBuilder sb, IEnumerable<SampleDescriptor> samples, MarkdownHeader beforeHeader, MarkdownHeader afterHeader)
        {
            bool isFirst = true;

            foreach (SampleDescriptor sample in samples)
            {
                if (!isFirst)
                {
                    sb.AppendHorizonalRule();
                }
                else
                {
                    isFirst = false;
                }

                sb.AppendHeader(beforeHeader);
                sb.AppendLine();
                sb.AppendCSharpCodeBlock(sample.Before);

                if (!string.IsNullOrEmpty(sample.After))
                {
                    sb.AppendLine();
                    sb.AppendHeader(afterHeader);
                    sb.AppendLine();
                    sb.AppendCSharpCodeBlock(sample.After);
                }
            }
        }

        public static string CreateRefactoringMarkdown(RefactoringDescriptor refactoring)
        {
            var sb = new StringBuilder();

            sb.AppendHeader2($"{refactoring.Title}");
            sb.AppendLine();

            sb.AppendTableHeader("Property", "Value");
            sb.AppendTableRow("Id", refactoring.Id);
            sb.AppendTableRow("Title", refactoring.Title);
            sb.AppendTableRow("Syntax", string.Join(", ", refactoring.Syntaxes.Select(f => f.Name)));

            if (!string.IsNullOrEmpty(refactoring.Span))
                sb.AppendTableRow("Span", refactoring.Span);

            sb.AppendTableRow("Enabled by Default", GetBooleanAsText(refactoring.IsEnabledByDefault));

            sb.AppendLine();
            sb.AppendHeader3("Usage");
            sb.AppendLine();

            WriteRefactoringSamples(sb, refactoring);

            sb.AppendLine();

            sb.AppendLink("full list of refactorings", "Refactorings.md");

            return sb.ToString();
        }

        public static string CreateAnalyzerMarkdown(AnalyzerDescriptor analyzer)
        {
            var sb = new StringBuilder();

            string title = analyzer.Title.TrimEnd('.');
            sb.AppendHeader($"{((analyzer.IsObsolete) ? "[deprecated] " : "")}{analyzer.Id}: {title}");
            sb.AppendLine();

            sb.AppendTableHeader("Property", "Value");
            sb.AppendTableRow("Id", analyzer.Id);
            sb.AppendTableRow("Category", analyzer.Category);
            sb.AppendTableRow("Default Severity", analyzer.DefaultSeverity);
            sb.AppendTableRow("Enabled by Default", GetBooleanAsText(analyzer.IsEnabledByDefault));
            sb.AppendTableRow("Supports Fade-Out", GetBooleanAsText(analyzer.SupportsFadeOut));
            sb.AppendTableRow("Supports Fade-Out Analyzer", GetBooleanAsText(analyzer.SupportsFadeOutAnalyzer));

            ReadOnlyCollection<SampleDescriptor> samples = analyzer.Samples;

            if (samples.Count > 0)
            {
                sb.AppendLine();
                sb.AppendHeader2((samples.Count == 1) ? "Example" : "Examples");
                sb.AppendLine();

                WriteSamples(sb, samples, new MarkdownHeader("Code with Diagnostic", 3), new MarkdownHeader("Code with Fix", 3));
            }

            sb.AppendLine();
            sb.AppendHeader2("How to Suppress");
            sb.AppendLine();

            sb.AppendHeader3("SuppressMessageAttribute");
            sb.AppendLine();

            sb.AppendCSharpCodeBlock($"[assembly: SuppressMessage(\"{analyzer.Category}\", \"{analyzer.Id}:{analyzer.Title}\", Justification = \"<Pending>\")]");
            sb.AppendLine();

            sb.AppendHeader3("#pragma");
            sb.AppendLine();

            sb.AppendCSharpCodeBlock($@"#pragma warning disable {analyzer.Id} // {analyzer.Title}
#pragma warning restore {analyzer.Id} // {analyzer.Title}");

            sb.AppendLine();

            sb.AppendHeader3("Ruleset");
            sb.AppendLine();

            sb.AppendUnorderedListItem();
            sb.AppendLink("How to configure rule set", "../HowToConfigureAnalyzers.md");
            sb.AppendLine();

            return sb.ToString();
        }

        public static string CreateAnalyzersReadMe(IEnumerable<AnalyzerDescriptor> analyzers, IComparer<string> comparer)
        {
            var sb = new StringBuilder();

            sb.AppendHeader2("Roslynator Analyzers");
            sb.AppendLine();

            sb.AppendTableHeader("Id", "Title", "Category", new MarkdownTableHeader("Enabled by Default", Alignment.Center));

            foreach (AnalyzerDescriptor analyzer in analyzers.OrderBy(f => f.Id, comparer))
            {
                sb.AppendTableRow(
                    analyzer.Id,
                    new MarkdownLink(analyzer.Title.TrimEnd('.'), $"../../docs/analyzers/{analyzer.Id}.md"),
                    analyzer.Category,
                    (analyzer.IsEnabledByDefault) ? "x" : "");
            }

            return sb.ToString();
        }

        public static string CreateRefactoringsReadMe(IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            var sb = new StringBuilder();

            sb.AppendHeader2("Roslynator Refactorings");
            sb.AppendLine();

            sb.AppendTableHeader("Id", "Title", new MarkdownTableHeader("Enabled by Default", Alignment.Center));

            foreach (RefactoringDescriptor refactoring in refactorings.OrderBy(f => f.Title, comparer))
            {
                sb.AppendTableRow(
                    refactoring.Id,
                    new MarkdownLink(refactoring.Title.TrimEnd('.'), $"../../docs/refactorings/{refactoring.Id}.md"),
                    (refactoring.IsEnabledByDefault) ? "x" : "");
            }

            return sb.ToString();
        }

        public static string CreateCodeFixesReadMe(IEnumerable<CodeFixDescriptor> codeFixes, IEnumerable<CompilerDiagnosticDescriptor> diagnostics, IComparer<string> comparer)
        {
            var sb = new StringBuilder();

            sb.AppendHeader2("Roslynator Code Fixes");
            sb.AppendLine();

            sb.AppendTableHeader("Id", "Title", "Fixable Diagnostics", new MarkdownTableHeader("Enabled by Default", Alignment.Center));

            foreach (CodeFixDescriptor codeFix in codeFixes.OrderBy(f => f.Title, comparer))
            {
                IEnumerable<MarkdownLink> links = codeFix
                    .FixableDiagnosticIds
                    .Join(diagnostics, f => f, f => f.Id, (f, g) => new MarkdownLink(g.Id, g.HelpUrl));

                sb.AppendTableRow(
                    codeFix.Id,
                    codeFix.Title.TrimEnd('.'),
                    new MarkdownText(string.Join(", ", links), shouldEscape: false),
                    (codeFix.IsEnabledByDefault) ? "x" : "");
            }

            return sb.ToString();
        }

        public static string CreateCodeFixesByDiagnosticId(IEnumerable<CodeFixDescriptor> codeFixes, IEnumerable<CompilerDiagnosticDescriptor> diagnostics)
        {
            var sb = new StringBuilder();

            sb.AppendHeader2("Roslynator Code Fixes by Diagnostic Id");
            sb.AppendLine();

            sb.AppendTableHeader("Diagnostic", "Code Fixes");

            foreach (var grouping in codeFixes
                .SelectMany(codeFix => codeFix.FixableDiagnosticIds.Select(diagnosticId => new { DiagnosticId = diagnosticId, CodeFixDescriptor = codeFix }))
                .OrderBy(f => f.DiagnosticId)
                .ThenBy(f => f.CodeFixDescriptor.Id)
                .GroupBy(f => f.DiagnosticId))
            {
                CompilerDiagnosticDescriptor diagnostic = diagnostics.FirstOrDefault(f => f.Id == grouping.Key);

                if (!string.IsNullOrEmpty(diagnostic?.HelpUrl))
                {
                    sb.AppendLink(diagnostic.Id, diagnostic.HelpUrl);
                }
                else
                {
                    sb.Append(grouping.Key);
                }

                sb.Append('|');
                sb.Append(string.Join(", ", grouping.Select(f => f.CodeFixDescriptor.Id)));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string CreateAnalyzersByCategoryMarkdown(IEnumerable<AnalyzerDescriptor> analyzers, IComparer<string> comparer)
        {
            var sb = new StringBuilder();

            sb.AppendHeader2("Roslynator Analyzers by Category");
            sb.AppendLine();

            sb.AppendTableHeader("Category", "Title", "Id", new MarkdownTableHeader("Enabled by Default", Alignment.Center));

            foreach (IGrouping<string, AnalyzerDescriptor> grouping in analyzers
                .GroupBy(f => f.Category.EscapeMarkdown())
                .OrderBy(f => f.Key, comparer))
            {
                foreach (AnalyzerDescriptor analyzer in grouping.OrderBy(f => f.Title, comparer))
                {
                    sb.AppendTableRow(
                        grouping.Key,
                        new MarkdownLink(analyzer.Title.TrimEnd('.'), $"../../docs/analyzers/{analyzer.Id}.md"),
                        analyzer.Id,
                        (analyzer.IsEnabledByDefault) ? "x" : "");
                }
            }

            return sb.ToString();
        }

        private static StringBuilder AppendRefactoringImage(StringBuilder sb, RefactoringDescriptor refactoring, string fileName)
        {
            return sb
                .AppendImage(refactoring.Title, $"../../images/refactorings/{fileName}.png")
                .AppendLine();
        }

        private static string GetBooleanAsText(bool value)
        {
            return (value) ? "yes" : "no";
        }
    }
}
