// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CodeFixes;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class FixCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        private static ImmutableArray<string> _roslynatorAnalyzersAssemblies;
        private static ImmutableArray<string> _roslynatorCodeFixesAssemblies;

        public FixCommandExecutor(
            FixCommandLineOptions options,
            DiagnosticSeverity severityLevel,
            ImmutableDictionary<string, string> diagnosticFixMap,
            ImmutableDictionary<string, string> diagnosticFixerMap,
            string language) : base(language)
        {
            Options = options;
            SeverityLevel = severityLevel;
            DiagnosticFixMap = diagnosticFixMap;
            DiagnosticFixerMap = diagnosticFixerMap;
        }

        public static ImmutableArray<string> RoslynatorAnalyzersAssemblies
        {
            get
            {
                if (_roslynatorAnalyzersAssemblies.IsDefault)
                {
                    _roslynatorAnalyzersAssemblies = ImmutableArray.CreateRange(new string[]
                    {
                        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Roslynator.CSharp.Analyzers.dll"),
                        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Roslynator.CSharp.Analyzers.CodeFixes.dll"),
                    });
                }

                return _roslynatorAnalyzersAssemblies;
            }
        }

        public static ImmutableArray<string> RoslynatorCodeFixesAssemblies
        {
            get
            {
                if (_roslynatorCodeFixesAssemblies.IsDefault)
                {
                    _roslynatorCodeFixesAssemblies = ImmutableArray.CreateRange(new string[]
                    {
                        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Roslynator.CSharp.CodeFixes.dll"),
                    });
                }

                return _roslynatorCodeFixesAssemblies;
            }
        }

        public FixCommandLineOptions Options { get; }

        public DiagnosticSeverity SeverityLevel { get; }

        public ImmutableDictionary<string, string> DiagnosticFixMap { get; }

        public ImmutableDictionary<string, string> DiagnosticFixerMap { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            var codeFixerOptions = new CodeFixerOptions(
                severityLevel: SeverityLevel,
                ignoreCompilerErrors: Options.IgnoreCompilerErrors,
                ignoreAnalyzerReferences: Options.IgnoreAnalyzerReferences,
                supportedDiagnosticIds: Options.SupportedDiagnostics,
                ignoredDiagnosticIds: Options.IgnoredDiagnostics,
                ignoredCompilerDiagnosticIds: Options.IgnoredCompilerDiagnostics,
                projectNames: Options.Projects,
                ignoredProjectNames: Options.IgnoredProjects,
                diagnosticIdsFixableOneByOne: Options.DiagnosticsFixableOneByOne,
                diagnosticFixMap: DiagnosticFixMap,
                diagnosticFixerMap: DiagnosticFixerMap,
                fileBanner: Options.FileBanner,
                language: Language,
                maxIterations: Options.MaxIterations,
                batchSize: Options.BatchSize,
                format: Options.Format);

            IEnumerable<string> analyzerAssemblies = Options.AnalyzerAssemblies;

            if (Options.UseRoslynatorAnalyzers)
                analyzerAssemblies = analyzerAssemblies.Concat(RoslynatorAnalyzersAssemblies);

            if (Options.UseRoslynatorCodeFixes)
                analyzerAssemblies = analyzerAssemblies.Concat(RoslynatorCodeFixesAssemblies);

            CultureInfo culture = (Options.Culture != null) ? CultureInfo.GetCultureInfo(Options.Culture) : null;

            return await FixAsync(projectOrSolution, analyzerAssemblies, codeFixerOptions, culture, cancellationToken);
        }

        internal static async Task<CommandResult> FixAsync(
            ProjectOrSolution projectOrSolution,
            IEnumerable<string> analyzerAssemblies,
            CodeFixerOptions codeFixerOptions,
            IFormatProvider formatProvider = null,
            CancellationToken cancellationToken = default)
        {
            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                Solution solution = project.Solution;

                var codeFixer = new CodeFixer(solution, SyntaxFactsServiceFactory.Instance, analyzerAssemblies: analyzerAssemblies, formatProvider: formatProvider, options: codeFixerOptions);

                WriteLine($"Fix '{project.Name}'", ConsoleColor.Cyan, Verbosity.Minimal);

                await codeFixer.FixProjectAsync(project, cancellationToken);
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                var codeFixer = new CodeFixer(solution, SyntaxFactsServiceFactory.Instance, analyzerAssemblies: analyzerAssemblies, options: codeFixerOptions);

                await codeFixer.FixSolutionAsync(cancellationToken);
            }

            return CommandResult.Success;
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Fixing was canceled.", Verbosity.Quiet);
        }
    }
}
