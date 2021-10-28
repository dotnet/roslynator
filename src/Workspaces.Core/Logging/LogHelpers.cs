// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Spelling;
using Roslynator.Text;
using static Roslynator.Logger;

namespace Roslynator
{
    internal static class LogHelpers
    {
        private static readonly SymbolDisplayFormat _symbolDefinitionFormat = SymbolDisplayFormat.CSharpErrorMessageFormat.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
                | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                | SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName,
            parameterOptions: SymbolDisplayParameterOptions.IncludeParamsRefOut
                | SymbolDisplayParameterOptions.IncludeType
                | SymbolDisplayParameterOptions.IncludeName
                | SymbolDisplayParameterOptions.IncludeDefaultValue);

        public static void WriteDiagnostic(
            Diagnostic diagnostic,
            string baseDirectoryPath = null,
            IFormatProvider formatProvider = null,
            string indentation = null,
            Verbosity verbosity = Verbosity.Diagnostic)
        {
            string text = DiagnosticFormatter.FormatDiagnostic(diagnostic, baseDirectoryPath, formatProvider);

            Write(indentation, verbosity);
            WriteLine(text, diagnostic.Severity.GetColors(), verbosity);
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

        public static void WriteSpellingDiagnostic(
            SpellingDiagnostic diagnostic,
            SpellingFixerOptions options,
            SourceText sourceText,
            string baseDirectoryPath,
            string indentation,
            Verbosity verbosity)
        {
            WriteDiagnostic(diagnostic.Diagnostic, baseDirectoryPath, default(IFormatProvider), indentation, verbosity);

            TextSpan span = diagnostic.Span;
            TextLineCollection lines = sourceText.Lines;
            int lineIndex = lines.IndexOf(span.Start);
            TextLine line = lines[lineIndex];

            int start = Math.Max(0, lineIndex - options.CodeContext);

            for (int i = start; i < lineIndex; i++)
                WriteTextLine(i);

            int index = span.Start - line.Span.Start;
            string text = line.ToString();

            Write(indentation, verbosity);
            Write(text.Substring(0, index), ConsoleColors.DarkGray, verbosity);
            Write(diagnostic.Value, ConsoleColors.Cyan, verbosity);
            WriteLine(text.Substring(index + diagnostic.Length), ConsoleColors.DarkGray, verbosity);

            int max = Math.Min(lines.Count - 1, lineIndex + options.CodeContext);

            for (int i = lineIndex + 1; i <= max; i++)
                WriteTextLine(i);

            void WriteTextLine(int i)
            {
                Write(indentation, verbosity);
                WriteLine(lines[i].ToString(), ConsoleColors.DarkGray, verbosity);
            }
        }

        public static void WriteLineSpan(
            TextSpan span,
            int context,
            SourceText sourceText,
            string indentation,
            Verbosity verbosity)
        {
            TextLineCollection lines = sourceText.Lines;
            int lineIndex = lines.IndexOf(span.Start);
            TextLine line = lines[lineIndex];

            int start = Math.Max(0, lineIndex - context);

            for (int i = start; i < lineIndex; i++)
                WriteTextLine(i);

            int index = span.Start - line.Span.Start;
            string text = line.ToString();

            Write(indentation, verbosity);
            Write(text.Substring(0, index), verbosity);
            Write(text.Substring(index, span.Length), ConsoleColors.Cyan, verbosity);
            WriteLine(text.Substring(index + span.Length), verbosity);

            int max = Math.Min(lines.Count - 1, lineIndex + context);

            for (int i = lineIndex + 1; i <= max; i++)
                WriteTextLine(i);

            void WriteTextLine(int i)
            {
                Write(indentation, verbosity);
                WriteLine(lines[i].ToString(), ConsoleColors.DarkGray, verbosity);
            }
        }

        public static void WriteAnalyzerExceptionDiagnostics(ImmutableArray<Diagnostic> diagnostics)
        {
            foreach (string message in diagnostics
                .Where(f => f.IsAnalyzerExceptionDiagnostic())
                .Select(f => f.ToString())
                .Distinct())
            {
                WriteLine(message, ConsoleColors.Yellow, Verbosity.Diagnostic);
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
            WriteDiagnosticRules(unfixableDiagnostics, "Unfixable diagnostics:");
            WriteDiagnosticRules(unfixedDiagnostics, "Unfixed diagnostics:");
            WriteDiagnosticRules(fixedDiagnostics, "Fixed diagnostics:");

            void WriteDiagnosticRules(
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
            WriteLine("  Infinite loop detected: Reported diagnostics have been previously fixed", ConsoleColors.Yellow, Verbosity.Normal);

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
                WriteLine($"  Format '{PathUtilities.TrimStart(document.FilePath, solutionDirectory)}'", ConsoleColors.DarkGray, Verbosity.Detailed);
            }
        }

        public static void WriteUsedAnalyzers(
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            Func<DiagnosticDescriptor, bool> predicate,
            CodeAnalysisOptions options,
            ConsoleColors colors,
            Verbosity verbosity)
        {
            if (!analyzers.Any())
                return;

            if (!ShouldWrite(verbosity))
                return;

            IEnumerable<DiagnosticDescriptor> descriptors = analyzers
                .SelectMany(f => f.SupportedDiagnostics)
                .Distinct(DiagnosticDescriptorComparer.Id)
                .Where(f => options.IsSupportedDiagnosticId(f.Id));

            if (predicate != null)
                descriptors = descriptors.Where(predicate);

            foreach (IGrouping<DiagnosticDescriptor, DiagnosticDescriptor> grouping in descriptors
                .GroupBy(f => f, DiagnosticDescriptorComparer.IdPrefix)
                .OrderBy(f => f.Key, DiagnosticDescriptorComparer.IdPrefix))
            {
                int count = grouping.Count();
                string prefix = DiagnosticIdPrefix.GetPrefix(grouping.Key.Id);

                Write($"  {count} supported {((count == 1) ? "diagnostic" : "diagnostics")} with ", colors, verbosity);
                Write((string.IsNullOrEmpty(prefix)) ? "no prefix" : $"prefix '{prefix}'", colors, verbosity);

                using (IEnumerator<DiagnosticDescriptor> en = grouping
                    .OrderBy(f => f.Id)
                    .GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        Write(" (", colors, verbosity);

                        while (true)
                        {
                            Write(en.Current.Id, colors, verbosity);

                            if (en.MoveNext())
                            {
                                Write(", ", colors, verbosity);
                            }
                            else
                            {
                                break;
                            }
                        }

                        Write(")", colors, verbosity);
                    }
                }

                WriteLine("", colors, verbosity);
            }
        }

        public static void WriteUsedFixers(
            ImmutableArray<CodeFixProvider> fixers,
            CodeAnalysisOptions options,
            ConsoleColors colors,
            Verbosity verbosity)
        {
            if (!ShouldWrite(verbosity))
                return;

            foreach (IGrouping<string, string> grouping in fixers
                .SelectMany(f => f.FixableDiagnosticIds)
                .Distinct()
                .Where(f => options.IsSupportedDiagnosticId(f))
                .GroupBy(f => f, DiagnosticIdComparer.Prefix)
                .OrderBy(f => f.Key, DiagnosticIdComparer.Prefix))
            {
                int count = grouping.Count();
                string prefix = DiagnosticIdPrefix.GetPrefix(grouping.Key);

                Write($"  {count} fixable {((count == 1) ? "diagnostic" : "diagnostics")} with ", colors, verbosity);
                Write((string.IsNullOrEmpty(prefix)) ? "no prefix" : $"prefix '{prefix}'", colors, verbosity);

                using (IEnumerator<string> en = grouping
                    .OrderBy(f => f)
                    .GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        Write(" (", colors, verbosity);

                        while (true)
                        {
                            Write(en.Current, colors, verbosity);

                            if (en.MoveNext())
                            {
                                Write(", ", colors, verbosity);
                            }
                            else
                            {
                                break;
                            }
                        }

                        Write(")", colors, verbosity);
                    }
                }

                WriteLine("", colors, verbosity);
            }
        }

        public static void WriteMultipleFixersSummary(string diagnosticId, CodeFixProvider fixer1, CodeFixProvider fixer2)
        {
            WriteLine($"  Diagnostic '{diagnosticId}' is fixable with multiple fixers", ConsoleColors.Yellow, Verbosity.Diagnostic);
            WriteLine($"    Fixer 1: '{fixer1.GetType().FullName}'", ConsoleColors.Yellow, Verbosity.Diagnostic);
            WriteLine($"    Fixer 2: '{fixer2.GetType().FullName}'", ConsoleColors.Yellow, Verbosity.Diagnostic);
        }

        public static void WriteMultipleOperationsSummary(CodeAction fix)
        {
            WriteLine("  Code action has multiple operations", ConsoleColors.Yellow, Verbosity.Diagnostic);
            WriteLine($"    Title:           {fix.Title}", ConsoleColors.Yellow, Verbosity.Diagnostic);
            WriteLine($"    EquivalenceKey: {fix.EquivalenceKey}", ConsoleColors.Yellow, Verbosity.Diagnostic);
        }

        public static void WriteSymbolDefinition(
            ISymbol symbol,
            string baseDirectoryPath = null,
            string indentation = null,
            Verbosity verbosity = Verbosity.Diagnostic)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            sb.Append(indentation);
            DiagnosticFormatter.FormatLocation(symbol.Locations[0], baseDirectoryPath, ref sb);
            sb.Append(GetSymbolTitle(symbol));

            if (symbol.IsKind(SymbolKind.Parameter, SymbolKind.TypeParameter))
            {
                sb.Append(" '");
                sb.Append(symbol.Name);
                sb.Append("': ");

                if ((symbol.ContainingSymbol as IMethodSymbol)?.MethodKind == MethodKind.LambdaMethod)
                {
                    sb.Append("anonymous function");
                }
                else
                {
                    Debug.Assert(symbol.ContainingSymbol.IsKind(SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property), symbol.Kind.ToString());

                    sb.Append(symbol.ContainingSymbol.ToDisplayString(_symbolDefinitionFormat));
                }
            }
            else
            {
                sb.Append(": ");
                sb.Append(symbol.ToDisplayString(_symbolDefinitionFormat));
            }

            WriteLine(StringBuilderCache.GetStringAndFree(sb), ConsoleColors.Cyan, verbosity);

            static string GetSymbolTitle(ISymbol symbol)
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.Event:
                        {
                            return "event";
                        }
                    case SymbolKind.Field:
                        {
                            return (symbol.ContainingType.TypeKind == TypeKind.Enum) ? "enum field" : "field";
                        }
                    case SymbolKind.Local:
                        {
                            return "local";
                        }
                    case SymbolKind.Method:
                        {
                            var methodSymbol = (IMethodSymbol)symbol;

                            switch (methodSymbol.MethodKind)
                            {
                                case MethodKind.Ordinary:
                                    return "method";
                                case MethodKind.LocalFunction:
                                    return "local function";
                            }

                            Debug.Fail(methodSymbol.MethodKind.ToString());
                            break;
                        }
                    case SymbolKind.NamedType:
                        {
                            var typeSymbol = (INamedTypeSymbol)symbol;

                            switch (typeSymbol.TypeKind)
                            {
                                case TypeKind.Class:
                                    return "class";
                                case TypeKind.Delegate:
                                    return "delegate";
                                case TypeKind.Enum:
                                    return "enum";
                                case TypeKind.Interface:
                                    return "interface";
                                case TypeKind.Struct:
                                    return "struct";
                            }

                            Debug.Fail(typeSymbol.TypeKind.ToString());
                            break;
                        }
                    case SymbolKind.Parameter:
                        {
                            return "parameter";
                        }
                    case SymbolKind.Property:
                        {
                            return (((IPropertySymbol)symbol).IsIndexer) ? "indexer" : "property";
                        }
                    case SymbolKind.TypeParameter:
                        {
                            return "type parameter";
                        }
                }

                Debug.Fail(symbol.Kind.ToString());
                return symbol.Kind.ToString();
            }
        }

        public static int WriteCompilerErrors(
            ImmutableArray<Diagnostic> diagnostics,
            string baseDirectoryPath = null,
            ImmutableHashSet<string> ignoredCompilerDiagnosticIds = null,
            IFormatProvider formatProvider = null,
            string indentation = null,
            int limit = 1000)
        {
            ignoredCompilerDiagnosticIds ??= ImmutableHashSet<string>.Empty;

            using (IEnumerator<Diagnostic> en = diagnostics
                .Where(f => f.Severity == DiagnosticSeverity.Error
                    && !ignoredCompilerDiagnosticIds.Contains(f.Id))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    const int maxCount = 10;

                    int count = 0;

                    do
                    {
                        count++;

                        if (count <= maxCount)
                        {
                            WriteDiagnostic(
                                en.Current,
                                baseDirectoryPath: baseDirectoryPath,
                                formatProvider: formatProvider,
                                indentation: indentation,
                                verbosity: Verbosity.Normal);
                        }
                        else
                        {
                            break;
                        }

                    } while (en.MoveNext());

                    count = 0;

                    var plus = false;

                    while (en.MoveNext())
                    {
                        count++;

                        if (count == limit)
                        {
                            plus = true;
                            break;
                        }
                    }

                    if (count > maxCount)
                    {
                        Write(indentation);
                        WriteLine($"and {count}{((plus) ? "+" : "")} more errors", verbosity: Verbosity.Normal);
                    }

                    return count;
                }
            }

            return 0;
        }
    }
}
