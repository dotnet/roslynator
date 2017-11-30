// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Roslynator.Metadata;
using Roslynator.Utilities;

namespace Roslynator.CodeGeneration.Markdown
{
    public static class MarkdownGenerator
    {
        public static string CreateReadMe(IEnumerable<AnalyzerDescriptor> analyzers, IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine(File.ReadAllText(@"..\text\ReadMe.txt", Encoding.UTF8));

                sw.WriteLine("### List of Analyzers");
                sw.WriteLine();

                foreach (AnalyzerDescriptor info in analyzers.OrderBy(f => f.Id, comparer))
                {
                    sw.WriteLine($"* {info.Id} - [{info.Title.TrimEnd('.').EscapeMarkdown()}](docs/analyzers/{info.Id}.md)");
                }

                sw.WriteLine();
                sw.WriteLine("### List of Refactorings");
                sw.WriteLine();

                foreach (RefactoringDescriptor info in refactorings.OrderBy(f => f.Title, comparer))
                {
                    sw.WriteLine($"* [{info.Title.TrimEnd('.').EscapeMarkdown()}](docs/refactorings/{info.Id}.md)");
                }

                return sw.ToString();
            }
        }

        public static string CreateRefactoringsMarkDown(IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## Roslynator Refactorings");

                foreach (RefactoringDescriptor info in refactorings
                    .OrderBy(f => f.Title, comparer))
                {
                    sw.WriteLine("");
                    sw.WriteLine($"#### {info.Title.EscapeMarkdown()} ({info.Id})");
                    sw.WriteLine("");
                    sw.WriteLine($"* **Syntax**: {string.Join(", ", info.Syntaxes.Select(f => f.Name.EscapeMarkdown()))}");

                    if (!string.IsNullOrEmpty(info.Span))
                        sw.WriteLine($"* **Span**: {info.Span.EscapeMarkdown()}");

                    sw.WriteLine("");

                    WriteRefactoringSamples(sw, info);
                }

                return sw.ToString();
            }
        }

        private static void WriteRefactoringSamples(StringWriter sw, RefactoringDescriptor refactoring)
        {
            if (refactoring.Samples.Count > 0)
            {
                bool isFirst = true;

                foreach (SampleDescriptor sample in refactoring.Samples)
                {
                    if (!isFirst)
                    {
                        sw.WriteLine("_____");
                    }
                    else
                    {
                        isFirst = false;
                    }

                    sw.WriteLine("#### Before");
                    sw.WriteLine();
                    sw.WriteLine("```csharp");
                    sw.WriteLine(sample.Before);
                    sw.WriteLine("```");
                    sw.WriteLine();

                    sw.WriteLine("#### After");
                    sw.WriteLine();
                    sw.WriteLine("```csharp");
                    sw.WriteLine(sample.After);
                    sw.WriteLine("```");
                }
            }
            else if (refactoring.Images.Count > 0)
            {
                bool isFirst = true;

                foreach (ImageDescriptor image in refactoring.Images)
                {
                    if (!isFirst)
                        sw.WriteLine();

                    sw.WriteLine(CreateImageMarkDown(refactoring, image.Name));
                    isFirst = false;
                }
            }
            else
            {
                sw.WriteLine(CreateImageMarkDown(refactoring, refactoring.Identifier));
            }
        }

        public static string CreateRefactoringMarkDown(RefactoringDescriptor refactoring)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine($"## {refactoring.Title}");
                sw.WriteLine("");

                sw.WriteLine("Property | Value");
                sw.WriteLine("--- | --- ");
                sw.WriteLine($"Id | {refactoring.Id}");
                sw.WriteLine($"Title | {refactoring.Title.EscapeMarkdown()}");
                sw.WriteLine($"Syntax | {string.Join(", ", refactoring.Syntaxes.Select(f => f.Name.EscapeMarkdown()))}");

                if (!string.IsNullOrEmpty(refactoring.Span))
                    sw.WriteLine($"Span | {refactoring.Span.EscapeMarkdown()}");

                sw.WriteLine($"Enabled by Default | {GetBooleanAsText(refactoring.IsEnabledByDefault)}");

                sw.WriteLine("");
                sw.WriteLine("### Usage");
                sw.WriteLine("");

                WriteRefactoringSamples(sw, refactoring);

                sw.WriteLine("");

                sw.WriteLine("[full list of refactorings](Refactorings.md)");

                return sw.ToString();
            }
        }

        public static string CreateAnalyzerMarkDown(AnalyzerDescriptor analyzer)
        {
            using (var sw = new StringWriter())
            {
                string title = analyzer.Title.TrimEnd('.').EscapeMarkdown();
                sw.WriteLine($"#{((analyzer.IsObsolete) ? " [deprecated]" : "")} {analyzer.Id}: {title}");
                sw.WriteLine("");

                sw.WriteLine("Property | Value");
                sw.WriteLine("--- | --- ");
                sw.WriteLine($"Id | {analyzer.Id}");
                sw.WriteLine($"Category | {analyzer.Category}");
                sw.WriteLine($"Default Severity | {analyzer.DefaultSeverity}");
                sw.WriteLine($"Enabled by Default | {GetBooleanAsText(analyzer.IsEnabledByDefault)}");
                sw.WriteLine($"Supports Fade-Out | {GetBooleanAsText(analyzer.SupportsFadeOut)}");
                sw.WriteLine($"Supports Fade-Out Analyzer | {GetBooleanAsText(analyzer.SupportsFadeOutAnalyzer)}");

                sw.WriteLine();

                sw.WriteLine("## How to Suppress");
                sw.WriteLine();

                sw.WriteLine("### SuppressMessageAttribute");
                sw.WriteLine();

                sw.WriteLine("```csharp");
                sw.WriteLine($"[assembly: SuppressMessage(\"{analyzer.Category}\", \"{analyzer.Id}:{analyzer.Title}\", Justification = \"<Pending>\")]");
                sw.WriteLine("```");
                sw.WriteLine();

                sw.WriteLine(@"### \#pragma");
                sw.WriteLine();

                sw.WriteLine("```csharp");
                sw.WriteLine($"#pragma warning disable {analyzer.Id} // {analyzer.Title}");
                sw.WriteLine($"#pragma warning restore {analyzer.Id} // {analyzer.Title}");
                sw.WriteLine("```");
                sw.WriteLine();

                sw.WriteLine("### Ruleset");
                sw.WriteLine();

                sw.Write("* [How to configure rule set](../HowToConfigureAnalyzers.md)");
                sw.WriteLine();

                return sw.ToString();
            }
        }

        public static string CreateAnalyzersReadMe(IEnumerable<AnalyzerDescriptor> analyzers, IComparer<string> comparer)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## Roslynator Analyzers");
                sw.WriteLine();

                sw.WriteLine(" Id | Title | Category | Enabled by Default ");
                sw.WriteLine(" --- | --- | --- |:---:");

                foreach (AnalyzerDescriptor info in analyzers.OrderBy(f => f.Id, comparer))
                {
                    sw.Write(info.Id);
                    sw.Write('|');
                    sw.Write($"[{info.Title.TrimEnd('.').EscapeMarkdown()}](../../docs/analyzers/{info.Id}.md)");
                    sw.Write('|');
                    sw.Write(info.Category.EscapeMarkdown());
                    sw.Write('|');
                    sw.Write((info.IsEnabledByDefault) ? "x" : "");

                    sw.WriteLine();
                }

                return sw.ToString();
            }
        }

        public static string CreateRefactoringsReadMe(IEnumerable<RefactoringDescriptor> refactorings, IComparer<string> comparer)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## Roslynator Refactorings");
                sw.WriteLine();

                sw.WriteLine("Id | Title | Enabled by Default ");
                sw.WriteLine("--- | --- |:---:");

                foreach (RefactoringDescriptor info in refactorings.OrderBy(f => f.Title, comparer))
                {
                    sw.Write(info.Id);
                    sw.Write('|');
                    sw.Write($"[{info.Title.TrimEnd('.').EscapeMarkdown()}](../../docs/refactorings/{info.Id}.md)");
                    sw.Write('|');
                    sw.Write((info.IsEnabledByDefault) ? "x" : "");
                    sw.WriteLine();
                }

                return sw.ToString();
            }
        }

        public static string CreateCodeFixesReadMe(IEnumerable<CodeFixDescriptor> codeFixes, IEnumerable<CompilerDiagnosticDescriptor> diagnostics, IComparer<string> comparer)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## Roslynator Code Fixes");
                sw.WriteLine();

                sw.WriteLine("Id | Title | Fixable Diagnostics | Enabled by Default ");
                sw.WriteLine("--- | --- | --- |:---:");

                foreach (CodeFixDescriptor descriptor in codeFixes.OrderBy(f => f.Title, comparer))
                {
                    IEnumerable<string> fixableDiagnostics = descriptor
                        .FixableDiagnosticIds
                        .Join(diagnostics, f => f, f => f.Id, (f, g) => (!string.IsNullOrEmpty(g.HelpUrl)) ? $"[{g.Id}]({g.HelpUrl})" : g.Id);

                    sw.Write(descriptor.Id);
                    sw.Write('|');
                    sw.Write(descriptor.Title.TrimEnd('.').EscapeMarkdown());
                    sw.Write('|');
                    sw.Write(string.Join(", ", fixableDiagnostics));
                    sw.Write('|');
                    sw.Write((descriptor.IsEnabledByDefault) ? "x" : "");
                    sw.WriteLine();
                }

                return sw.ToString();
            }
        }

        public static string CreateCodeFixesByDiagnosticId(IEnumerable<CodeFixDescriptor> codeFixes, IEnumerable<CompilerDiagnosticDescriptor> diagnostics)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## Roslynator Code Fixes by Diagnostic Id");
                sw.WriteLine();

                sw.WriteLine("Diagnostic | Code Fixes");
                sw.WriteLine("--- | ---");

                foreach (var grouping in codeFixes
                    .SelectMany(f => f.FixableDiagnosticIds.Select(ff => new { DiagnosticId = ff, CodeFixDescriptor = f}))
                    .OrderBy(f => f.DiagnosticId)
                    .ThenBy(f => f.CodeFixDescriptor.Id)
                    .GroupBy(f => f.DiagnosticId))
                {
                    CompilerDiagnosticDescriptor diagnostic = diagnostics.FirstOrDefault(f => f.Id == grouping.Key);

                    if (!string.IsNullOrEmpty(diagnostic?.HelpUrl))
                    {
                        sw.Write($"[{diagnostic.Id}]({diagnostic.HelpUrl})");
                    }
                    else
                    {
                        sw.Write(grouping.Key);
                    }

                    sw.Write('|');
                    sw.Write(string.Join(", ", grouping.Select(f => f.CodeFixDescriptor.Id)));
                    sw.WriteLine();
                }

                return sw.ToString();
            }
        }

        public static string CreateAnalyzersByCategoryMarkDown(IEnumerable<AnalyzerDescriptor> analyzers, IComparer<string> comparer)
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## Roslynator Analyzers by Category");
                sw.WriteLine();

                sw.WriteLine(" Category | Title | Id | Enabled by Default ");
                sw.WriteLine(" --- | --- | --- |:---:");

                foreach (IGrouping<string, AnalyzerDescriptor> grouping in analyzers
                    .GroupBy(f => f.Category.EscapeMarkdown())
                    .OrderBy(f => f.Key, comparer))
                {
                    foreach (AnalyzerDescriptor info in grouping.OrderBy(f => f.Title, comparer))
                    {
                        sw.Write(grouping.Key);
                        sw.Write('|');
                        sw.Write($"[{info.Title.TrimEnd('.').EscapeMarkdown()}](../../docs/analyzers/{info.Id}.md)");
                        sw.Write('|');
                        sw.Write(info.Id);
                        sw.Write('|');
                        sw.Write((info.IsEnabledByDefault) ? "x" : "");

                        sw.WriteLine();
                    }
                }

                return sw.ToString();
            }
        }

        private static string CreateImageMarkDown(RefactoringDescriptor refactoring, string fileName)
        {
            return $"![{refactoring.Title.EscapeMarkdown()}](../../images/refactorings/{fileName.EscapeMarkdown()}.png)";
        }

        private static string GetBooleanAsText(bool value)
        {
            return (value) ? "yes" : "no";
        }
    }
}
