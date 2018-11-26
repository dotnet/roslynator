// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Telemetry;
using static Roslynator.Logger;

namespace Roslynator.Diagnostics
{
    internal class CodeAnalyzer
    {
        private readonly AnalyzerAssemblyList _analyzerAssemblies = new AnalyzerAssemblyList();

        private readonly AnalyzerAssemblyList _analyzerReferences = new AnalyzerAssemblyList();

        private static readonly TimeSpan _minimalExecutionTime = TimeSpan.FromMilliseconds(1);

        private static readonly CompilationWithAnalyzersOptions _ignoreSuppressedDiagnosticsCompilationWithAnalyzersOptions = new CompilationWithAnalyzersOptions(
            options: default(AnalyzerOptions),
            onAnalyzerException: default(Action<Exception, DiagnosticAnalyzer, Diagnostic>),
            concurrentAnalysis: true,
            logAnalyzerExecutionTime: true,
            reportSuppressedDiagnostics: false);

        private static readonly CompilationWithAnalyzersOptions _reportSuppressedDiagnosticsCompilationWithAnalyzersOptions = new CompilationWithAnalyzersOptions(
            options: default(AnalyzerOptions),
            onAnalyzerException: default(Action<Exception, DiagnosticAnalyzer, Diagnostic>),
            concurrentAnalysis: true,
            logAnalyzerExecutionTime: true,
            reportSuppressedDiagnostics: true);

        public CodeAnalyzer(
            AbstractSyntaxFactsServiceFactory syntaxFactsFactory,
            IEnumerable<string> analyzerAssemblies = null,
            IFormatProvider formatProvider = null,
            CodeAnalyzerOptions options = null)
        {
            SyntaxFactsFactory = syntaxFactsFactory;

            Options = options ?? CodeAnalyzerOptions.Default;

            if (analyzerAssemblies != null)
                _analyzerAssemblies.LoadFrom(analyzerAssemblies, loadFixers: false);

            FormatProvider = formatProvider;
        }

        public AbstractSyntaxFactsServiceFactory SyntaxFactsFactory { get; }

        public CodeAnalyzerOptions Options { get; }

        public IFormatProvider FormatProvider { get; }

        public async Task<ImmutableArray<ProjectAnalysisResult>> AnalyzeSolutionAsync(Solution solution, CancellationToken cancellationToken = default)
        {
            foreach (string id in Options.IgnoredDiagnosticIds.OrderBy(f => f))
                WriteLine($"Ignore diagnostic '{id}'", Verbosity.Diagnostic);

            ImmutableArray<ProjectId> projectIds = solution
                .GetProjectDependencyGraph()
                .GetTopologicallySortedProjects(cancellationToken)
                .ToImmutableArray();

            WriteLine($"Analyze solution '{solution.FilePath}'", ConsoleColor.Cyan, Verbosity.Minimal);

            var results = new List<ProjectAnalysisResult>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            TimeSpan lastElapsed = TimeSpan.Zero;

            for (int i = 0; i < projectIds.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Project project = solution.GetProject(projectIds[i]);

                if (Options.IsSupportedProject(project))
                {
                    WriteLine($"Analyze '{project.Name}' {$"{i + 1}/{projectIds.Length}"}", Verbosity.Minimal);

                    ProjectAnalysisResult result = await AnalyzeProjectAsync(project, cancellationToken).ConfigureAwait(false);

                    if (result != null)
                        results.Add(result);
                }
                else
                {
                    WriteLine($"Skip '{project.Name}' {$"{i + 1}/{projectIds.Length}"}", ConsoleColor.DarkGray, Verbosity.Minimal);
                }

                lastElapsed = stopwatch.Elapsed;
            }

            stopwatch.Stop();

            if (results.Count > 0)
            {
                if (Options.ExecutionTime)
                    WriteExecutionTime(results);

                int totalCount = 0;

                foreach (ProjectAnalysisResult result in results)
                {
                    IEnumerable<Diagnostic> diagnostics = result.Diagnostics
                        .Where(f => !f.IsAnalyzerExceptionDiagnostic())
                        .Concat(result.CompilerDiagnostics);

                    totalCount += FilterDiagnostics(diagnostics, cancellationToken).Count();
                }

                if (totalCount > 0)
                {
                    WriteLine(Verbosity.Minimal);

                    Dictionary<DiagnosticDescriptor, int> diagnosticsByDescriptor = results
                        .SelectMany(f => FilterDiagnostics(f.Diagnostics.Concat(f.CompilerDiagnostics), cancellationToken))
                        .GroupBy(f => f.Descriptor, DiagnosticDescriptorComparer.Id)
                        .ToDictionary(f => f.Key, f => f.Count());

                    int maxCountLength = Math.Max(totalCount.ToString().Length, diagnosticsByDescriptor.Max(f => f.Value.ToString().Length));
                    int maxIdLength = diagnosticsByDescriptor.Max(f => f.Key.Id.Length);

                    foreach (KeyValuePair<DiagnosticDescriptor, int> kvp in diagnosticsByDescriptor.OrderBy(f => f.Key.Id))
                    {
                        WriteLine($"{kvp.Value.ToString().PadLeft(maxCountLength)} {kvp.Key.Id.PadRight(maxIdLength)} {kvp.Key.Title}", Verbosity.Normal);
                    }

                    WriteLine($"{totalCount} {((totalCount == 1) ? "diagnostic" : "diagnostics")} found", ConsoleColor.Green, Verbosity.Minimal);
                    WriteLine(Verbosity.Minimal);
                }
            }

            WriteLine($"Done analyzing solution '{solution.FilePath}' in {stopwatch.Elapsed:mm\\:ss\\.ff}", Verbosity.Minimal);

            return results.ToImmutableArray();
        }

        public async Task<ProjectAnalysisResult> AnalyzeProjectAsync(Project project, CancellationToken cancellationToken = default)
        {
            ImmutableArray<DiagnosticAnalyzer> analyzers = Utilities.GetAnalyzers(
                project: project,
                analyzerAssemblies: _analyzerAssemblies,
                analyzerReferences: _analyzerReferences,
                options: Options);

            if (!analyzers.Any())
            {
                WriteLine($"  No analyzers found to analyze '{project.Name}'", ConsoleColor.DarkGray, Verbosity.Normal);
                return default;
            }

            WriteUsedAnalyzers(analyzers, ConsoleColor.DarkGray, Verbosity.Diagnostic);

            cancellationToken.ThrowIfCancellationRequested();

            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = (Options.IgnoreCompilerDiagnostics)
                ? ImmutableArray<Diagnostic>.Empty
                : compilation.GetDiagnostics(cancellationToken);

            compilerDiagnostics = FilterDiagnostics(compilerDiagnostics, cancellationToken).ToImmutableArray();

            CompilationWithAnalyzersOptions compilationWithAnalyzersOptions = (Options.ReportSuppressedDiagnostics)
                ? _reportSuppressedDiagnosticsCompilationWithAnalyzersOptions
                : _ignoreSuppressedDiagnosticsCompilationWithAnalyzersOptions;

            var compilationWithAnalyzers = new CompilationWithAnalyzers(compilation, analyzers, compilationWithAnalyzersOptions);

            ImmutableArray<Diagnostic> diagnostics = default;
            ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo> telemetry = ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo>.Empty;

            if (Options.ExecutionTime)
            {
                AnalysisResult analysisResult = await compilationWithAnalyzers.GetAnalysisResultAsync(cancellationToken).ConfigureAwait(false);

                diagnostics = analysisResult.GetAllDiagnostics();
                telemetry = analysisResult.AnalyzerTelemetryInfo;
            }
            else
            {
                diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken).ConfigureAwait(false);
            }

            string projectDirectoryPath = Path.GetDirectoryName(project.FilePath);

            WriteDiagnostics(FilterDiagnostics(diagnostics.Where(f => f.IsAnalyzerExceptionDiagnostic()), cancellationToken).ToImmutableArray(), baseDirectoryPath: projectDirectoryPath, formatProvider: FormatProvider, indentation: "  ", verbosity: Verbosity.Diagnostic);

            diagnostics = FilterDiagnostics(diagnostics.Where(f => !f.IsAnalyzerExceptionDiagnostic()), cancellationToken).ToImmutableArray();

            WriteDiagnostics(compilerDiagnostics, baseDirectoryPath: projectDirectoryPath, formatProvider: FormatProvider, indentation: "  ", verbosity: Verbosity.Normal);

            WriteDiagnostics(diagnostics, baseDirectoryPath: projectDirectoryPath, formatProvider: FormatProvider, indentation: "  ", verbosity: Verbosity.Normal);

            return new ProjectAnalysisResult(project.Id, analyzers, compilerDiagnostics, diagnostics, telemetry);
        }

        private IEnumerable<Diagnostic> FilterDiagnostics(IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken = default)
        {
            foreach (Diagnostic diagnostic in diagnostics)
            {
                if (Options.IsSupportedDiagnostic(diagnostic)
                    && (Options.ReportNotConfigurable || !diagnostic.Descriptor.CustomTags.Contains(WellKnownDiagnosticTags.NotConfigurable)))
                {
                    if (diagnostic.Descriptor.CustomTags.Contains(WellKnownDiagnosticTags.Compiler))
                    {
                        Debug.Assert(diagnostic.Id.StartsWith("CS", "VB", StringComparison.Ordinal), diagnostic.Id);

                        SyntaxTree tree = diagnostic.Location.SourceTree;

                        if (tree == null
                            || !GeneratedCodeUtility.IsGeneratedCode(tree, f => SyntaxFactsFactory.GetService(tree.Options.Language).IsComment(f), cancellationToken))
                        {
                            yield return diagnostic;
                        }
                    }
                    else
                    {
                        yield return diagnostic;
                    }
                }
            }
        }

        private static void WriteExecutionTime(List<ProjectAnalysisResult> results)
        {
            var telemetryInfos = new Dictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo>();

            foreach (ProjectAnalysisResult result in results)
            {
                foreach (KeyValuePair<DiagnosticAnalyzer, AnalyzerTelemetryInfo> kvp in result.Telemetry)
                {
                    DiagnosticAnalyzer analyzer = kvp.Key;

                    if (!telemetryInfos.TryGetValue(analyzer, out AnalyzerTelemetryInfo telemetryInfo))
                        telemetryInfo = new AnalyzerTelemetryInfo();

                    telemetryInfo.Add(kvp.Value);

                    telemetryInfos[analyzer] = telemetryInfo;
                }
            }

            WriteLine(Verbosity.Minimal);

            foreach (KeyValuePair<DiagnosticAnalyzer, AnalyzerTelemetryInfo> kvp in telemetryInfos
                .Where(f => f.Value.ExecutionTime >= _minimalExecutionTime)
                .OrderByDescending(f => f.Value.ExecutionTime))
            {
                WriteLine($"{kvp.Value.ExecutionTime:mm\\:ss\\.fff} '{kvp.Key.GetType().FullName}'", Verbosity.Minimal);
            }
        }
    }
}
