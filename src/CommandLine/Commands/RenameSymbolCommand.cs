// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Rename;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class RenameSymbolCommand : MSBuildWorkspaceCommand<RenameSymbolCommandResult>
    {
        public RenameSymbolCommand(
            RenameSymbolCommandLineOptions options,
            in ProjectFilter projectFilter,
            RenameScopeFilter scopeFilter,
            Visibility visibility,
            RenameErrorResolution errorResolution,
            IEnumerable<string> ignoredCompilerDiagnostics,
            int codeContext,
            Func<ISymbol, bool> predicate,
            Func<ISymbol, string> getNewName) : base(projectFilter)
        {
            Options = options;
            ScopeFilter = scopeFilter;
            Visibility = visibility;
            ErrorResolution = errorResolution;
            IgnoredCompilerDiagnostics = ignoredCompilerDiagnostics;
            CodeContext = codeContext;
            Predicate = predicate;
            GetNewName = getNewName;
        }

        public RenameSymbolCommandLineOptions Options { get; }

        public RenameScopeFilter ScopeFilter { get; }

        public Visibility Visibility { get; }

        public RenameErrorResolution ErrorResolution { get; }

        public IEnumerable<string> IgnoredCompilerDiagnostics { get; }

        public int CodeContext { get; }

        public Func<ISymbol, bool> Predicate { get; }

        public Func<ISymbol, string> GetNewName { get; }

        public override async Task<RenameSymbolCommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            var projectFilter = new ProjectFilter(Options.Projects, Options.IgnoredProjects, Language);

            SymbolRenamer renamer = null;
            ImmutableArray<SymbolRenameResult> results = default;

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                Solution solution = project.Solution;

                renamer = GetSymbolRenamer(solution);

                WriteLine($"Fix '{project.Name}'", ConsoleColors.Cyan, Verbosity.Minimal);

                Stopwatch stopwatch = Stopwatch.StartNew();

                results = await renamer.AnalyzeProjectAsync(project, cancellationToken);

                stopwatch.Stop();

                WriteLine($"Done fixing project '{project.FilePath}' in {stopwatch.Elapsed:mm\\:ss\\.ff}", Verbosity.Minimal);
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                renamer = GetSymbolRenamer(solution);

                results = await renamer.AnalyzeSolutionAsync(f => projectFilter.IsMatch(f), cancellationToken);
            }

            return new RenameSymbolCommandResult(CommandStatus.Success, results);

            SymbolRenamer GetSymbolRenamer(Solution solution)
            {
                VisibilityFilter visibilityFilter = Visibility switch
                {
                    Visibility.Public => VisibilityFilter.All,
                    Visibility.Internal => VisibilityFilter.Internal | VisibilityFilter.Private,
                    Visibility.Private => VisibilityFilter.Private,
                    _ => throw new InvalidOperationException()
                };

                var options = new SymbolRenamerOptions(
                    scopeFilter: ScopeFilter,
                    visibilityFilter: visibilityFilter,
                    errorResolution: ErrorResolution,
                    ignoredCompilerDiagnosticIds: IgnoredCompilerDiagnostics,
                    codeContext: CodeContext,
                    includeGeneratedCode: Options.IncludeGeneratedCode,
                    ask: Options.Ask,
                    dryRun: Options.DryRun,
                    interactive: Options.Interactive);

                return new SymbolRenamer(
                    solution,
                    predicate: Predicate,
                    getNewName: GetNewName,
                    userDialog: new ConsoleDialog(ConsoleDialogDefinition.Default, "    "),
                    options: options);
            }
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Renaming was canceled.", Verbosity.Minimal);
        }
    }
}
