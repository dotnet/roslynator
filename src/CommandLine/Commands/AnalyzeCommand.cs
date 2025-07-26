﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CommandLine.Json;
using Roslynator.CommandLine.Xml;
using Roslynator.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator.CommandLine;

internal class AnalyzeCommand : MSBuildWorkspaceCommand<AnalyzeCommandResult>
{
    public AnalyzeCommand(AnalyzeCommandLineOptions options, DiagnosticSeverity severityLevel, in ProjectFilter projectFilter, FileSystemFilter fileSystemFilter) : base(projectFilter, fileSystemFilter)
    {
        Options = options;
        SeverityLevel = severityLevel;
    }

    public AnalyzeCommandLineOptions Options { get; }

    public DiagnosticSeverity SeverityLevel { get; }

    public override async Task<AnalyzeCommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
    {
        var codeAnalyzerOptions = new CodeAnalyzerOptions(
            fileSystemFilter: FileSystemFilter,
            ignoreAnalyzerReferences: Options.IgnoreAnalyzerReferences,
            ignoreCompilerDiagnostics: Options.IgnoreCompilerDiagnostics,
            reportNotConfigurable: Options.ReportNotConfigurable,
            reportSuppressedDiagnostics: Options.ReportSuppressedDiagnostics,
            logAnalyzerExecutionTime: Options.ExecutionTime,
            severityLevel: SeverityLevel,
            supportedDiagnosticIds: Options.SupportedDiagnostics,
            ignoredDiagnosticIds: Options.IgnoredDiagnostics);

        IEnumerable<AnalyzerAssembly> analyzerAssemblies = Options.AnalyzerAssemblies
            .SelectMany(path => AnalyzerAssemblyLoader.LoadFrom(path, loadFixers: false).Select(info => info.AnalyzerAssembly));

        CultureInfo culture = (Options.Culture is not null) ? CultureInfo.GetCultureInfo(Options.Culture) : null;

        var analyzerLoader = new AnalyzerLoader(analyzerAssemblies, codeAnalyzerOptions);

        analyzerLoader.AnalyzerAssemblyAdded += (sender, args) =>
        {
            AnalyzerAssembly analyzerAssembly = args.AnalyzerAssembly;

            if (analyzerAssembly.Name.EndsWith(".Analyzers")
                || analyzerAssembly.HasAnalyzers
                || analyzerAssembly.HasFixers)
            {
                WriteLine($"Loaded analyzer assembly '{analyzerAssembly.FullName}'", ConsoleColors.DarkGray, Verbosity.Detailed);
            }
        };

        var codeAnalyzer = new CodeAnalyzer(
            analyzerLoader: analyzerLoader,
            formatProvider: culture,
            options: codeAnalyzerOptions);

        ImmutableArray<ProjectAnalysisResult> results;

        if (projectOrSolution.IsProject)
        {
            Project project = projectOrSolution.AsProject();

            ProjectAnalysisResult result = await codeAnalyzer.AnalyzeProjectAsync(project, cancellationToken);

            results = ImmutableArray.Create(result);
        }
        else
        {
            Solution solution = projectOrSolution.AsSolution();

            results = await codeAnalyzer.AnalyzeSolutionAsync(solution, f => IsMatch(f), cancellationToken);
        }

        return new AnalyzeCommandResult(
            (results.Any(f => f.Diagnostics.Length > 0 || f.CompilerDiagnostics.Length > 0)) ? CommandStatus.NotSuccess : CommandStatus.Success,
            results);
    }

    protected override void ProcessResults(IList<AnalyzeCommandResult> results)
    {
        IEnumerable<ProjectAnalysisResult> analysisResults = results.SelectMany(f => f.AnalysisResults);

        if (results.Count > 1)
            WriteAnalysisResults(analysisResults);

        if (Options.Output is not null
            && analysisResults.Any(f => f.Diagnostics.Any() || f.CompilerDiagnostics.Any()))
        {
            CultureInfo culture = (Options.Culture is not null) ? CultureInfo.GetCultureInfo(Options.Culture) : null;
            if (!string.IsNullOrWhiteSpace(Options.OutputFormat) && Options.OutputFormat.Equals("gitlab", StringComparison.CurrentCultureIgnoreCase))
            {
                DiagnosticGitLabJsonSerializer.Serialize(analysisResults, Options.Output, culture);
            }
            else
            {
                // Default output format is xml
                DiagnosticXmlSerializer.Serialize(analysisResults, Options.Output, culture);
            }
        }
    }

    private static void WriteAnalysisResults(IEnumerable<ProjectAnalysisResult> results)
    {
        ImmutableDictionary<DiagnosticDescriptor, int> diagnostics = results
            .SelectMany(f => f.Diagnostics.Concat(f.CompilerDiagnostics))
            .GroupBy(f => f.Descriptor, DiagnosticDescriptorComparer.Id)
            .ToImmutableDictionary(f => f.Key, f => f.Count(), DiagnosticDescriptorComparer.Id);

        int totalCount = diagnostics.Sum(f => f.Value);

        if (totalCount > 0)
        {
            WriteLine(Verbosity.Normal);

            int maxCountLength = Math.Max(totalCount.ToString().Length, diagnostics.Max(f => f.Value.ToString().Length));
            int maxIdLength = diagnostics.Max(f => f.Key.Id.Length);

            foreach (KeyValuePair<DiagnosticDescriptor, int> kvp in diagnostics
                .OrderBy(f => f.Key.Id))
            {
                WriteLine($"{kvp.Value.ToString().PadLeft(maxCountLength)} {kvp.Key.Id.PadRight(maxIdLength)} {kvp.Key.Title}", Verbosity.Normal);
            }
        }

        WriteLine(Verbosity.Minimal);
        WriteLine($"{totalCount} {((totalCount == 1) ? "diagnostic" : "diagnostics")} found", ConsoleColors.Green, Verbosity.Minimal);
    }
}
