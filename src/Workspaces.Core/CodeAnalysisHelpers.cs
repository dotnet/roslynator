// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    internal static class CodeAnalysisHelpers
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

            ImmutableArray<Assembly> assemblies = ImmutableArray<Assembly>.Empty;

            if (!options.IgnoreAnalyzerReferences)
            {
                assemblies = project.AnalyzerReferences
                    .Distinct()
                    .OfType<AnalyzerFileReference>()
                    .Select(f => f.GetAssembly())
                    .Where(f => !analyzerAssemblies.ContainsAssembly(f.FullName))
                    .ToImmutableArray();
            }

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
    }
}
