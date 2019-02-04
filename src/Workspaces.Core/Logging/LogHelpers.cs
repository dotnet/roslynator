// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CodeFixes;
using static Roslynator.Logger;

namespace Roslynator
{
    internal static class LogHelpers
    {
        public static void WriteDiagnostic(
            Diagnostic diagnostic,
            string baseDirectoryPath = null,
            IFormatProvider formatProvider = null,
            string indentation = null,
            Verbosity verbosity = Verbosity.Diagnostic)
        {
            string text = DiagnosticFormatter.FormatDiagnostic(diagnostic, baseDirectoryPath, formatProvider);

            Write(indentation, verbosity);
            WriteLine(text, diagnostic.Severity.GetColor(), verbosity);
        }

        public static void WriteDiagnostics(
            ImmutableArray<Diagnostic> diagnostics,
            string baseDirectoryPath = null,
            IFormatProvider formatProvider = null,
            string indentation = null,
            int maxCount = int.MaxValue,
            Verbosity verbosity = Verbosity.None)
        {
            if (!diagnostics.Any())
                return;

            if (!ShouldWrite(verbosity))
                return;

            int count = 0;

            foreach (Diagnostic diagnostic in diagnostics.OrderBy(f => f, DiagnosticComparer.IdThenFilePathThenSpanStart))
            {
                WriteDiagnostic(diagnostic, baseDirectoryPath, formatProvider, indentation, verbosity);

                count++;

                if (count > maxCount)
                {
                    int remainingCount = diagnostics.Length - count;

                    if (remainingCount > 0)
                    {
                        Write(indentation, verbosity);
                        WriteLine($"and {remainingCount} more diagnostics", verbosity);
                    }
                }
            }
        }

        public static void WriteAnalyzerExceptionDiagnostics(ImmutableArray<Diagnostic> diagnostics)
        {
            foreach (string message in diagnostics
                .Where(f => f.IsAnalyzerExceptionDiagnostic())
                .Select(f => f.ToString())
                .Distinct())
            {
                WriteLine(message, ConsoleColor.Yellow, Verbosity.Diagnostic);
            }
        }

        public static void WriteFixSummary(
            IEnumerable<Diagnostic> fixedDiagnostics,
            IEnumerable<Diagnostic> unfixedDiagnostics,
            IEnumerable<Diagnostic> unfixableDiagnostics,
            string baseDirectoryPath = null,
            string indentation = null,
            bool addEmptyLine = false,
            IFormatProvider formatProvider = null,
            Verbosity verbosity = Verbosity.None)
        {
            WriteDiagnosticDescriptors(unfixableDiagnostics, "Unfixable diagnostics:");
            WriteDiagnosticDescriptors(unfixedDiagnostics, "Unfixed diagnostics:");
            WriteDiagnosticDescriptors(fixedDiagnostics, "Fixed diagnostics:");

            void WriteDiagnosticDescriptors(
                IEnumerable<Diagnostic> diagnostics,
                string title)
            {
                List<(DiagnosticDescriptor descriptor, ImmutableArray<Diagnostic> diagnostics)> diagnosticsById = diagnostics
                    .GroupBy(f => f.Descriptor, DiagnosticDescriptorComparer.Id)
                    .Select(f => (descriptor: f.Key, diagnostics: f.ToImmutableArray()))
                    .OrderByDescending(f => f.diagnostics.Length)
                    .ThenBy(f => f.descriptor.Id)
                    .ToList();

                if (diagnosticsById.Count > 0)
                {
                    if (addEmptyLine)
                        WriteLine(verbosity);

                    Write(indentation, verbosity);
                    WriteLine(title, verbosity);

                    int maxIdLength = diagnosticsById.Max(f => f.descriptor.Id.Length);
                    int maxCountLength = diagnosticsById.Max(f => f.diagnostics.Length.ToString("n0").Length);

                    foreach ((DiagnosticDescriptor descriptor, ImmutableArray<Diagnostic> diagnostics2) in diagnosticsById)
                    {
                        Write(indentation, verbosity);
                        WriteLine($"  {diagnostics2.Length.ToString("n0").PadLeft(maxCountLength)} {descriptor.Id.PadRight(maxIdLength)} {descriptor.Title.ToString(formatProvider)}", verbosity);

                        if (baseDirectoryPath != null)
                        {
                            WriteDiagnostics(diagnostics2, baseDirectoryPath: baseDirectoryPath, formatProvider: formatProvider, indentation: indentation + "    ", verbosity: verbosity);
                        }
                    }
                }
            }
        }

        public static void WriteInfiniteLoopSummary(ImmutableArray<Diagnostic> diagnostics, ImmutableArray<Diagnostic> previousDiagnostics, Project project, IFormatProvider formatProvider = null)
        {
            WriteLine("  Infinite loop detected: Reported diagnostics have been previously fixed", ConsoleColor.Yellow, Verbosity.Normal);

            string baseDirectoryPath = Path.GetDirectoryName(project.FilePath);

            WriteLine(Verbosity.Detailed);
            WriteLine("  Diagnostics:", Verbosity.Detailed);
            WriteDiagnostics(diagnostics, baseDirectoryPath: baseDirectoryPath, formatProvider: formatProvider, indentation: "    ", verbosity: Verbosity.Detailed);
            WriteLine(Verbosity.Detailed);
            WriteLine("  Previous diagnostics:", Verbosity.Detailed);
            WriteDiagnostics(previousDiagnostics, baseDirectoryPath: baseDirectoryPath, formatProvider: formatProvider, indentation: "    ", verbosity: Verbosity.Detailed);
            WriteLine(Verbosity.Detailed);
        }

        public static void WriteFormattedDocuments(ImmutableArray<DocumentId> documentIds, Project project, string solutionDirectory)
        {
            foreach (DocumentId documentId in documentIds)
            {
                Document document = project.GetDocument(documentId);
                WriteLine($"  Format '{PathUtilities.TrimStart(document.FilePath, solutionDirectory)}'", ConsoleColor.DarkGray, Verbosity.Detailed);
            }
        }

        public static void WriteUsedAnalyzers(ImmutableArray<DiagnosticAnalyzer> analyzers, ConsoleColor color, Verbosity verbosity)
        {
            if (!analyzers.Any())
                return;

            if (!ShouldWrite(verbosity))
                return;

            WriteLine($"  Use {analyzers.Length} {((analyzers.Length == 1) ? "analyzer" : "analyzers")}", color, verbosity);

            foreach ((string prefix, int count) in DiagnosticIdPrefix.CountPrefixes(analyzers.SelectMany(f => f.SupportedDiagnostics).Select(f => f.Id)))
            {
                WriteLine($"    {count} supported {((count == 1) ? "diagnostic" : "diagnostics")} with prefix '{prefix}'", color, verbosity);
            }
        }

        public static void WriteUsedFixers(ImmutableArray<CodeFixProvider> fixers, ConsoleColor color, Verbosity verbosity)
        {
            if (!ShouldWrite(verbosity))
                return;

            WriteLine($"  Use {fixers.Length} {((fixers.Length == 1) ? "fixer" : "fixers")}", color, verbosity);

            foreach ((string prefix, int count) in DiagnosticIdPrefix.CountPrefixes(fixers.SelectMany(f => f.FixableDiagnosticIds)))
            {
                WriteLine($"    {count} fixable {((count == 1) ? "diagnostic" : "diagnostics")} with prefix '{prefix}'", color, verbosity);
            }
        }

        public static void WriteMultipleFixersSummary(string diagnosticId, CodeFixProvider fixer1, CodeFixProvider fixer2)
        {
            WriteLine($"  Diagnostic '{diagnosticId}' is fixable with multiple fixers", ConsoleColor.Yellow, Verbosity.Diagnostic);
            WriteLine($"    Fixer 1: '{fixer1.GetType().FullName}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
            WriteLine($"    Fixer 2: '{fixer2.GetType().FullName}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
        }

        public static void WriteMultipleOperationsSummary(CodeAction fix)
        {
            WriteLine("  Code action has multiple operations", ConsoleColor.Yellow, Verbosity.Diagnostic);
            WriteLine($"    Title:           {fix.Title}", ConsoleColor.Yellow, Verbosity.Diagnostic);
            WriteLine($"    EquivalenceKey: {fix.EquivalenceKey}", ConsoleColor.Yellow, Verbosity.Diagnostic);
        }

        public static void WriteProjectFixResults(
            List<ProjectFixResult> results,
            CodeFixerOptions options,
            IFormatProvider formatProvider = null)
        {
            if (options.FileBannerLines.Any())
            {
                int count = results.Sum(f => f.NumberOfAddedFileBanners);
                WriteLine();
                WriteLine($"{count} file {((count == 1) ? "banner" : "banners")} added", Verbosity.Normal);
            }

            if (options.Format)
            {
                int count = results.Sum(f => f.NumberOfFormattedDocuments);
                WriteLine();
                WriteLine($"{count} {((count == 1) ? "document" : "documents")} formatted", Verbosity.Normal);
            }

            WriteFixSummary(
                results.SelectMany(f => f.FixedDiagnostics),
                results.SelectMany(f => f.UnfixedDiagnostics),
                results.SelectMany(f => f.UnfixableDiagnostics),
                addEmptyLine: true,
                formatProvider: formatProvider,
                verbosity: Verbosity.Normal);

            WriteLine(Verbosity.Minimal);

            int fixedCount = results.Sum(f => f.FixedDiagnostics.Length);

            WriteLine($"{fixedCount} {((fixedCount == 1) ? "diagnostic" : "diagnostics")} fixed", ConsoleColor.Green, Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);
        }
    }
}
