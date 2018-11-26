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

namespace Roslynator
{
    internal static class Logger
    {
        public static ConsoleWriter ConsoleOut { get; } = ConsoleWriter.Instance;

        public static TextWriterWithVerbosity Out { get; set; }

        public static void Write(char value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(char[] buffer)
        {
            ConsoleOut.Write(buffer);
            Out?.Write(buffer);
        }

        public static void Write(char[] buffer, int index, int count)
        {
            ConsoleOut.Write(buffer, index, count);
            Out?.Write(buffer, index, count);
        }

        public static void Write(bool value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(int value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(uint value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(long value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(ulong value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(float value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(double value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(decimal value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(string value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(string value, Verbosity verbosity)
        {
            ConsoleOut.Write(value, verbosity: verbosity);
            Out?.Write(value, verbosity: verbosity);
        }

        public static void Write(string value, ConsoleColor color)
        {
            ConsoleOut.Write(value, color);
            Out?.Write(value);
        }

        public static void Write(string value, ConsoleColor color, Verbosity verbosity)
        {
            ConsoleOut.Write(value, color, verbosity: verbosity);
            Out?.Write(value, verbosity: verbosity);
        }

        public static void WriteIf(bool condition, string value)
        {
            ConsoleOut.WriteIf(condition, value);
            Out?.WriteIf(condition, value);
        }

        public static void WriteIf(bool condition, string value, ConsoleColor color)
        {
            ConsoleOut.WriteIf(condition, value, color);
            Out?.WriteIf(condition, value);
        }

        public static void Write(object value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(string format, object arg0)
        {
            ConsoleOut.Write(format, arg0);
            Out?.Write(format, arg0);
        }

        public static void Write(string format, object arg0, object arg1)
        {
            ConsoleOut.Write(format, arg0, arg1);
            Out?.Write(format, arg0, arg1);
        }

        public static void Write(string format, object arg0, object arg1, object arg2)
        {
            ConsoleOut.Write(format, arg0, arg1, arg2);
            Out?.Write(format, arg0, arg1, arg2);
        }

        public static void Write(string format, params object[] arg)
        {
            ConsoleOut.Write(format, arg);
            Out?.Write(format, arg);
        }

        public static void WriteLine()
        {
            ConsoleOut.WriteLine();
            Out?.WriteLine();
        }

        public static void WriteLine(Verbosity verbosity)
        {
            ConsoleOut.WriteLine(verbosity);
            Out?.WriteLine(verbosity);
        }

        public static void WriteLineIf(bool condition)
        {
            ConsoleOut.WriteLineIf(condition);
            Out?.WriteLineIf(condition);
        }

        public static void WriteLine(char value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(char[] buffer)
        {
            ConsoleOut.WriteLine(buffer);
            Out?.WriteLine(buffer);
        }

        public static void WriteLine(char[] buffer, int index, int count)
        {
            ConsoleOut.WriteLine(buffer, index, count);
            Out?.WriteLine(buffer, index, count);
        }

        public static void WriteLine(bool value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(int value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(uint value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(long value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(ulong value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(float value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(double value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(decimal value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(string value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(string value, Verbosity verbosity)
        {
            ConsoleOut.WriteLine(value, verbosity: verbosity);
            Out?.WriteLine(value, verbosity: verbosity);
        }

        public static void WriteLine(string value, ConsoleColor color)
        {
            ConsoleOut.WriteLine(value, color);
            Out?.WriteLine(value);
        }

        public static void WriteLine(string value, ConsoleColor color, Verbosity verbosity)
        {
            ConsoleOut.WriteLine(value, color, verbosity: verbosity);
            Out?.WriteLine(value, verbosity: verbosity);
        }

        public static void WriteLineIf(bool condition, string value)
        {
            ConsoleOut.WriteLineIf(condition, value);
            Out?.WriteLineIf(condition, value);
        }

        public static void WriteLineIf(bool condition, string value, ConsoleColor color)
        {
            ConsoleOut.WriteLineIf(condition, value, color);
            Out?.WriteLineIf(condition, value);
        }

        public static void WriteLine(object value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(string format, object arg0)
        {
            ConsoleOut.WriteLine(format, arg0);
            Out?.WriteLine(format, arg0);
        }

        public static void WriteLine(string format, object arg0, object arg1)
        {
            ConsoleOut.WriteLine(format, arg0, arg1);
            Out?.WriteLine(format, arg0, arg1);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            ConsoleOut.WriteLine(format, arg0, arg1, arg2);
            Out?.WriteLine(format, arg0, arg1, arg2);
        }

        public static void WriteLine(string format, params object[] arg)
        {
            ConsoleOut.WriteLine(format, arg);
            Out?.WriteLine(format, arg);
        }

        public static void WriteDiagnostic(
            Diagnostic diagnostic,
            string baseDirectoryPath = null,
            IFormatProvider formatProvider = null,
            string indentation = null,
            Verbosity verbosity = Verbosity.Diagnostic)
        {
            Write(indentation, verbosity);

            string text = DiagnosticFormatter.FormatDiagnostic(diagnostic, baseDirectoryPath, formatProvider);

            ConsoleOut.WriteLine(text, diagnostic.Severity.GetColor(), verbosity);
            Out?.WriteLine(text, verbosity);
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

            foreach ((Diagnostic diagnostic, string message) in DiagnosticFormatter.FormatDiagnostics(diagnostics, baseDirectoryPath, formatProvider))
            {
                Write(indentation, verbosity);
                ConsoleOut.WriteLine(message, diagnostic.Severity.GetColor(), verbosity);
                Out?.WriteLine(message, verbosity);

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

        public static void WriteFixSummary(
            IEnumerable<Diagnostic> fixedDiagnostics,
            IEnumerable<Diagnostic> unfixedDiagnostics,
            IEnumerable<Diagnostic> unfixableDiagnostics,
            string indent = null,
            bool addEmptyLine = false,
            IFormatProvider formatProvider = null,
            Verbosity verbosity = Verbosity.None)
        {
            WriteDiagnosticDescriptors(unfixableDiagnostics, "Unfixable diagnostics:");
            WriteDiagnosticDescriptors(unfixedDiagnostics, "Unfixed diagnostics:");
            WriteDiagnosticDescriptors(fixedDiagnostics, "Fixed diagnostics:");

            bool WriteDiagnosticDescriptors(
                IEnumerable<Diagnostic> diagnostics,
                string title)
            {
                List<(DiagnosticDescriptor descriptor, int count)> diagnosticsById = diagnostics
                    .GroupBy(f => f.Descriptor, DiagnosticDescriptorComparer.Id)
                    .Select(f => (descriptor: f.Key, count: f.Count()))
                    .OrderByDescending(f => f.count)
                    .ThenBy(f => f.descriptor.Id)
                    .ToList();

                if (diagnosticsById.Count > 0)
                {
                    if (addEmptyLine)
                        WriteLine(verbosity);

                    Write(indent, verbosity);
                    WriteLine(title, verbosity);

                    int maxIdLength = diagnosticsById.Max(f => f.descriptor.Id.Length);
                    int maxCountLength = diagnosticsById.Max(f => f.count.ToString("n0").Length);

                    foreach ((DiagnosticDescriptor descriptor, int count) in diagnosticsById)
                    {
                        Write(indent, verbosity);
                        WriteLine($"  {count.ToString("n0").PadLeft(maxCountLength)} {descriptor.Id.PadRight(maxIdLength)} {descriptor.Title.ToString(formatProvider)}", verbosity);
                    }

                    return true;
                }

                return false;
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
            if (ShouldWrite(verbosity))
            {
                WriteLine($"  Use {analyzers.Length} {((analyzers.Length == 1) ? "analyzer" : "analyzers")}", color, verbosity);

                if (ShouldWrite(verbosity))
                {
                    foreach ((string prefix, int count) in Utilities.GetLetterPrefixes(analyzers.SelectMany(f => f.SupportedDiagnostics).Select(f => f.Id)))
                    {
                        WriteLine($"    {count} supported {((count == 1) ? "diagnostic" : "diagnostics")} with prefix '{prefix}'", color, verbosity);
                    }
                }
            }
        }

        public static void WriteUsedFixers(ImmutableArray<CodeFixProvider> fixers, ConsoleColor color, Verbosity verbosity)
        {
            if (ShouldWrite(verbosity))
            {
                WriteLine($"  Use {fixers.Length} {((fixers.Length == 1) ? "fixer" : "fixers")}", color, verbosity);

                if (ShouldWrite(verbosity))
                {
                    foreach ((string prefix, int count) in Utilities.GetLetterPrefixes(fixers.SelectMany(f => f.FixableDiagnosticIds)))
                    {
                        WriteLine($"    {count} fixable {((count == 1) ? "diagnostic" : "diagnostics")} with prefix '{prefix}'", color, verbosity);
                    }
                }
            }
        }

        public static void WriteMultipleFixersSummary(string diagnosticId, CodeFixProvider fixer1, CodeFixProvider fixer2)
        {
            WriteLine($"  Diagnostic '{diagnosticId}' is fixable with multiple fixers", ConsoleColor.Yellow, Verbosity.Diagnostic);
            WriteLine($"    Fixer 1: '{fixer1.GetType().FullName}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
            WriteLine($"    Fixer 2: '{fixer2.GetType().FullName}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
        }

        public static void WriteMultipleActionsSummary(in MultipleFixesInfo info)
        {
            WriteLine($"  '{info.Fixer.GetType().FullName}' registered multiple actions to fix diagnostic '{info.DiagnosticId}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
            WriteLine($"    EquivalenceKey 1: '{info.EquivalenceKey1}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
            WriteLine($"    EquivalenceKey 2: '{info.EquivalenceKey2}'", ConsoleColor.Yellow, Verbosity.Diagnostic);
        }

        public static void WriteMultipleOperationsSummary(CodeAction fix)
        {
            WriteLine("  Code action has multiple operations", ConsoleColor.Yellow, Verbosity.Diagnostic);
            WriteLine($"    Title:           {fix.Title}", ConsoleColor.Yellow, Verbosity.Diagnostic);
            WriteLine($"    EquivalenceKey: {fix.EquivalenceKey}", ConsoleColor.Yellow, Verbosity.Diagnostic);
        }

        private static bool ShouldWrite(Verbosity verbosity)
        {
            return verbosity <= ConsoleOut.Verbosity
                || (Out != null && verbosity <= Out.Verbosity);
        }
    }
}
