// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CommandLine.Rename;
using Roslynator.Rename;
using static Roslynator.Logger;

namespace Roslynator.CommandLine;

internal class RenameSymbolCommand : MSBuildWorkspaceCommand<RenameSymbolCommandResult>
{
    public RenameSymbolCommand(
        RenameSymbolCommandLineOptions options,
        in ProjectFilter projectFilter,
        FileSystemFilter fileSystemFilter,
        RenameScopeFilter scopeFilter,
        CliCompilationErrorResolution errorResolution,
        IEnumerable<string> ignoredCompilerDiagnostics,
        int codeContext,
        Func<ISymbol, bool> predicate,
        Func<ISymbol, string> getNewName) : base(projectFilter, fileSystemFilter)
    {
        Options = options;
        ScopeFilter = scopeFilter;
        ErrorResolution = errorResolution;
        IgnoredCompilerDiagnostics = ignoredCompilerDiagnostics;
        CodeContext = codeContext;
        Predicate = predicate;
        GetNewName = getNewName;
    }

    public RenameSymbolCommandLineOptions Options { get; }

    public RenameScopeFilter ScopeFilter { get; }

    public CliCompilationErrorResolution ErrorResolution { get; }

    public IEnumerable<string> IgnoredCompilerDiagnostics { get; }

    public int CodeContext { get; }

    public Func<ISymbol, bool> Predicate { get; }

    public Func<ISymbol, string> GetNewName { get; }

    public override async Task<RenameSymbolCommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
    {
        AssemblyResolver.Register();

        SymbolRenameState renamer = null;

        if (projectOrSolution.IsProject)
        {
            Project project = projectOrSolution.AsProject();

            Solution solution = project.Solution;

            renamer = GetSymbolRenamer(solution);

            WriteLine($"Fix '{project.Name}'", ConsoleColors.Cyan, Verbosity.Minimal);

            Stopwatch stopwatch = Stopwatch.StartNew();

            await renamer.RenameSymbolsAsync(project, cancellationToken);

            stopwatch.Stop();

            WriteLine($"Done fixing project '{project.FilePath}' in {stopwatch.Elapsed:mm\\:ss\\.ff}", Verbosity.Minimal);
        }
        else
        {
            Solution solution = projectOrSolution.AsSolution();

            renamer = GetSymbolRenamer(solution);

            await renamer.RenameSymbolsAsync(solution.Projects.Where(p => IsMatch(p)), cancellationToken);
        }

        return new RenameSymbolCommandResult(CommandStatus.Success);

        SymbolRenameState GetSymbolRenamer(Solution solution)
        {
            var options = new SymbolRenamerOptions()
            {
                SkipTypes = (ScopeFilter & RenameScopeFilter.Type) != 0,
                SkipMembers = (ScopeFilter & RenameScopeFilter.Member) != 0,
                SkipLocals = (ScopeFilter & RenameScopeFilter.Local) != 0,
                IncludeGeneratedCode = Options.IncludeGeneratedCode,
                DryRun = Options.DryRun,
                FileSystemMatcher = FileSystemFilter?.Matcher,
            };

            if (IgnoredCompilerDiagnostics is not null)
            {
                foreach (string id in IgnoredCompilerDiagnostics)
                    options.IgnoredCompilerDiagnosticIds.Add(id);
            }

            return new CliSymbolRenameState(
                solution,
                predicate: Predicate,
                getNewName: GetNewName,
                ask: Options.Ask,
                interactive: Options.Interactive,
                codeContext: -1,
                errorResolution: ErrorResolution,
                options: options);
        }
    }

    protected override void OperationCanceled(OperationCanceledException ex)
    {
        WriteLine("Renaming was canceled.", Verbosity.Minimal);
    }
}
