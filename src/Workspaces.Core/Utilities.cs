// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator
{
    internal static class Utilities
    {
        public static ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(
            Project project,
            AnalyzerAssemblyList analyzerAssemblies,
            AnalyzerAssemblyList analyzerReferences,
            CodeAnalysisOptions options)
        {
            (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) = GetAnalyzersAndFixers(
                project: project,
                analyzerAssemblies: analyzerAssemblies,
                analyzerReferences: analyzerReferences,
                options: options,
                loadFixers: false);

            return analyzers;
        }

        public static (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) GetAnalyzersAndFixers(
            Project project,
            AnalyzerAssemblyList analyzerAssemblies,
            AnalyzerAssemblyList analyzerReferences,
            CodeAnalysisOptions options)
        {
            return GetAnalyzersAndFixers(
                project: project,
                analyzerAssemblies: analyzerAssemblies,
                analyzerReferences: analyzerReferences,
                options: options,
                loadFixers: true);
        }

        private static (ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers) GetAnalyzersAndFixers(
            Project project,
            AnalyzerAssemblyList analyzerAssemblies,
            AnalyzerAssemblyList analyzerReferences,
            CodeAnalysisOptions options,
            bool loadFixers = true)
        {
            string language = project.Language;

            ImmutableArray<Assembly> assemblies = (options.IgnoreAnalyzerReferences) ? ImmutableArray<Assembly>.Empty : project.AnalyzerReferences
                .Distinct()
                .OfType<AnalyzerFileReference>()
                .Select(f => f.GetAssembly())
                .Where(f => !analyzerAssemblies.ContainsAssembly(f.FullName))
                .ToImmutableArray();

            ImmutableArray<DiagnosticAnalyzer> analyzers = analyzerAssemblies
                .GetAnalyzers(language)
                .Concat(analyzerReferences.GetOrAddAnalyzers(assemblies, language))
                .Where(analyzer =>
                {
                    ImmutableArray<DiagnosticDescriptor> supportedDiagnostics = analyzer.SupportedDiagnostics;

                    if (options.SupportedDiagnosticIds.Count > 0)
                    {
                        bool success = false;

                        foreach (DiagnosticDescriptor supportedDiagnostic in supportedDiagnostics)
                        {
                            if (options.SupportedDiagnosticIds.Contains(supportedDiagnostic.Id))
                            {
                                success = true;
                                break;
                            }
                        }

                        if (!success)
                            return false;
                    }
                    else if (options.IgnoredDiagnosticIds.Count > 0)
                    {
                        bool success = false;

                        foreach (DiagnosticDescriptor supportedDiagnostic in supportedDiagnostics)
                        {
                            if (!options.IgnoredDiagnosticIds.Contains(supportedDiagnostic.Id))
                            {
                                success = true;
                                break;
                            }
                        }

                        if (!success)
                            return false;
                    }

                    foreach (DiagnosticDescriptor supportedDiagnostic in supportedDiagnostics)
                    {
                        ReportDiagnostic reportDiagnostic = supportedDiagnostic.GetEffectiveSeverity(project.CompilationOptions);

                        if (reportDiagnostic != ReportDiagnostic.Suppress
                            && reportDiagnostic.ToDiagnosticSeverity() >= options.SeverityLevel)
                        {
                            return true;
                        }
                    }

                    return false;
                })
                .ToImmutableArray();

            ImmutableArray<CodeFixProvider> fixers = ImmutableArray<CodeFixProvider>.Empty;

            if (analyzers.Any()
                && loadFixers)
            {
                fixers = analyzerAssemblies
                    .GetFixers(language)
                    .Concat(analyzerReferences.GetOrAddFixers(assemblies, language))
                    .ToImmutableArray();
            }

            return (analyzers, fixers);
        }

        public static async Task<bool> VerifySyntaxEquivalenceAsync(
            Document oldDocument,
            Document newDocument,
            SyntaxFactsService syntaxFacts,
            CancellationToken cancellationToken = default)
        {
            if (!string.Equals(
                (await newDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false)).NormalizeWhitespace("", false).ToFullString(),
                (await oldDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false)).NormalizeWhitespace("", false).ToFullString(),
                StringComparison.Ordinal))
            {
                WriteLine($"Syntax roots with normalized white-space are not equivalent '{oldDocument.FilePath}'", ConsoleColor.Magenta);
                return false;
            }

            if (!syntaxFacts.AreEquivalent(
                await newDocument.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false),
                await oldDocument.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false)))
            {
                WriteLine($"Syntax trees are not equivalent '{oldDocument.FilePath}'", ConsoleColor.Magenta);
                return false;
            }

            return true;
        }

        public static string GetShortLanguageName(string languageName)
        {
            switch (languageName)
            {
                case LanguageNames.CSharp:
                case LanguageNames.FSharp:
                    return languageName;
                case LanguageNames.VisualBasic:
                    return "VB";
            }

            Debug.Fail(languageName);

            return languageName;
        }

        public static IEnumerable<(string prefix, int count)> GetLetterPrefixes(IEnumerable<string> values)
        {
            foreach (IGrouping<string, string> grouping in values
                .Select(id =>
                {
                    int length = 0;

                    for (int i = 0; i < id.Length; i++)
                    {
                        if (char.IsLetter(id[i]))
                        {
                            length++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    return id.Substring(0, length);
                })
                .GroupBy(f => f)
                .OrderBy(f => f.Key))
            {
                yield return (grouping.Key, grouping.Count());
            }
        }
    }
}
