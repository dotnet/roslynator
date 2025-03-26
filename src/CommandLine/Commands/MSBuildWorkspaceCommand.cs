﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.FileSystemGlobbing;
using static Roslynator.Logger;

namespace Roslynator.CommandLine;

internal abstract class MSBuildWorkspaceCommand<TCommandResult> where TCommandResult : CommandResult
{
    protected MSBuildWorkspaceCommand(in ProjectFilter projectFilter, FileSystemFilter fileSystemFilter)
    {
        ProjectFilter = projectFilter;
        FileSystemFilter = fileSystemFilter;
    }

    public string Language
    {
        get { return ProjectFilter.Language; }
    }

    public ProjectFilter ProjectFilter { get; }

    public FileSystemFilter FileSystemFilter { get; }

    public abstract Task<TCommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default);

    public async Task<CommandStatus> ExecuteAsync(IEnumerable<PathInfo> paths, string msbuildPath = null, IEnumerable<string> properties = null)
    {
        if (paths is null)
            throw new ArgumentNullException(nameof(paths));

        if (!paths.Any())
            throw new ArgumentException("", nameof(paths));

        MSBuildWorkspace workspace = null;

        try
        {
            workspace = CreateMSBuildWorkspace(msbuildPath, properties);

            if (workspace is null)
                return CommandStatus.Fail;

            workspace.WorkspaceFailed += (sender, args) => WorkspaceFailed(sender, args);

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            CancellationToken cancellationToken = cts.Token;

            try
            {
                var status = CommandStatus.Success;
                var results = new List<TCommandResult>();

                foreach (PathInfo path in paths)
                {
                    if (path.Origin == PathOrigin.PipedInput)
                    {
                        Matcher matcher = (IsSolutionFile(path.Path))
                            ? ProjectFilter.SolutionMatcher
                            : ProjectFilter.Matcher;

                        if (matcher?.Match(path.Path).HasMatches == false)
                        {
                            WriteLine($"Skip '{path.Path}'", ConsoleColors.DarkGray, Verbosity.Normal);
                            continue;
                        }
                    }

                    TCommandResult result;
                    try
                    {
                        result = await ExecuteAsync(path.Path, workspace, cancellationToken);

                        if (result is null)
                        {
                            status = CommandStatus.Fail;
                            continue;
                        }
                    }
                    catch (ProjectOrSolutionLoadException ex)
                    {
                        WriteLine(ex.Message, Colors.Message_Warning, Verbosity.Minimal);
                        WriteError(ex.InnerException);
                        status = CommandStatus.Fail;
                        continue;
                    }

                    results.Add(result);

                    if (status == CommandStatus.Success)
                        status = result.Status;

                    if (status == CommandStatus.Canceled)
                        break;

                    workspace.CloseSolution();
                }

                ProcessResults(results);

                return status;
            }
            catch (OperationCanceledException ex)
            {
                OperationCanceled(ex);
            }
            catch (AggregateException ex)
            {
                OperationCanceledException operationCanceledException = ex.GetOperationCanceledException();

                if (operationCanceledException is not null)
                {
                    OperationCanceled(operationCanceledException);
                }
                else
                {
                    throw;
                }
            }
        }
        finally
        {
            workspace?.Dispose();
        }

        return CommandStatus.Canceled;
    }

    private async Task<TCommandResult> ExecuteAsync(string path, MSBuildWorkspace workspace, CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Project or solution file not found: {path}");

        ProjectOrSolution projectOrSolution = await OpenProjectOrSolutionAsync(path, workspace, ConsoleProgressReporter.Default, cancellationToken);

        Solution solution = projectOrSolution.AsSolution();

        if (solution is not null
            && !VerifyProjectNames(solution))
        {
            return null;
        }

        if (FileSystemFilter is not null)
            FileSystemFilter.RootDirectoryPath = Path.GetDirectoryName(path);

        return await ExecuteAsync(projectOrSolution, cancellationToken);
    }

    private bool VerifyProjectNames(Solution solution)
    {
        List<ProjectName> projectNames = solution.Projects.Select(f => ProjectName.Create(f.Name)).ToList();

        if (ShouldWrite(Verbosity.Detailed))
        {
            WriteLine("List of projects:", Verbosity.Detailed);

            foreach (IGrouping<string, ProjectName> grouping in projectNames
                .OrderBy(f => f.NameWithoutMoniker)
                .GroupBy(f => f.NameWithoutMoniker))
            {
                WriteLine($"  {grouping.Key}", Verbosity.Detailed);

                foreach (string moniker in grouping
                    .Select(f => f.Moniker)
                    .Where(f => f is not null)
#if NETFRAMEWORK
                    .OrderBy(f => f))
#else
                    .Order())
#endif
                {
                    WriteLine($"    {moniker}", Verbosity.Detailed);
                }
            }
        }

        ImmutableHashSet<ProjectName> values = (ProjectFilter.Names.Count > 0)
            ? ProjectFilter.Names
            : ProjectFilter.IgnoredNames;

        foreach (ProjectName value in values)
        {
            if (!projectNames.Any(f => string.Equals(f.Name, value.Name, StringComparison.Ordinal))
                && !projectNames.Any(f => string.Equals(f.NameWithoutMoniker, value.NameWithoutMoniker, StringComparison.Ordinal)))
            {
                WriteLine($"Project '{value.Name}' does not exist.", ConsoleColors.Yellow, Verbosity.Quiet);
                return false;
            }
        }

        return true;
    }

    protected virtual void ProcessResults(IList<TCommandResult> results)
    {
    }

    protected virtual void OperationCanceled(OperationCanceledException ex)
    {
        WriteLine("Operation was canceled.", Verbosity.Quiet);
    }

    protected virtual void WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
    {
        WriteLine($"  {e.Diagnostic.Message}", e.Diagnostic.Kind.GetColors(), Verbosity.Detailed);
    }

    private static async Task<ProjectOrSolution> OpenProjectOrSolutionAsync(
        string path,
        MSBuildWorkspace workspace,
        IProgress<ProjectLoadProgress> progress = null,
        CancellationToken cancellationToken = default)
    {
        bool isSolution = IsSolutionFile(path);

        WriteLine($"Loading {((isSolution) ? "solution" : "project")} '{path}'...", Verbosity.Minimal);

        ProjectOrSolution projectOrSolution;

        try
        {
            if (isSolution)
            {
                projectOrSolution = await workspace.OpenSolutionAsync(path, progress, cancellationToken);
            }
            else
            {
                projectOrSolution = await workspace.OpenProjectAsync(path, progress, cancellationToken);
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new ProjectOrSolutionLoadException($"Error occurred while loading {((isSolution) ? "solution" : "project")} '{path}'", ex);
        }

        return projectOrSolution;
    }

    private static MSBuildWorkspace CreateMSBuildWorkspace(string msbuildPath, IEnumerable<string> rawProperties)
    {
        if (msbuildPath is not null)
        {
            MSBuildLocator.RegisterMSBuildPath(msbuildPath);
        }
        else if (TryGetVisualStudioInstance(out VisualStudioInstance instance))
        {
            MSBuildLocator.RegisterInstance(instance);
            msbuildPath = instance.MSBuildPath;
        }
        else
        {
            return null;
        }

        WriteLine($"MSBuild location is '{msbuildPath}'", Verbosity.Detailed);

        if (!ParseHelpers.TryParseMSBuildProperties(rawProperties, out Dictionary<string, string> properties))
            return null;

        if (properties is null)
            properties = new Dictionary<string, string>();

        // https://github.com/Microsoft/MSBuildLocator/issues/16
        if (!properties.ContainsKey("AlwaysCompileMarkupFilesInSeparateDomain"))
            properties["AlwaysCompileMarkupFilesInSeparateDomain"] = bool.FalseString;

        return MSBuildWorkspace.Create(properties);
    }

    private static bool TryGetVisualStudioInstance(out VisualStudioInstance result)
    {
        List<VisualStudioInstance> instances = MSBuildLocator.QueryVisualStudioInstances()
            .Distinct(VisualStudioInstanceComparer.MSBuildPath)
            .ToList();

        if (instances.Count == 0)
        {
            WriteLine($"MSBuild location not found. Use option '-{OptionShortNames.MSBuildPath}, --{OptionNames.MSBuildPath}' to specify MSBuild location", Verbosity.Quiet);
            result = null;
            return false;
        }

        WriteLine("Available MSBuild locations:", Verbosity.Diagnostic);

        foreach (VisualStudioInstance instance in instances.OrderBy(f => f.Version))
            WriteLine($"  {instance.Name}, Version: {instance.Version}, Path: {instance.MSBuildPath}", Verbosity.Diagnostic);

        instances = instances
            .GroupBy(f => f.Version)
            .OrderByDescending(f => f.Key)
            .First()
            .ToList();

        if (instances.Count > 1)
        {
            WriteLine($"Cannot choose MSBuild location automatically. Use option '-{OptionShortNames.MSBuildPath}, --{OptionNames.MSBuildPath}' to specify MSBuild location", Verbosity.Quiet);
            result = null;
            return false;
        }

        result = instances[0];
        return true;
    }

    private protected IEnumerable<Project> FilterProjects(
        ProjectOrSolution projectOrSolution,
        Func<Solution, ImmutableArray<ProjectId>> getProjects = null)
    {
        if (projectOrSolution.IsProject)
        {
            yield return projectOrSolution.AsProject();
        }
        else if (projectOrSolution.IsSolution)
        {
            foreach (Project project in FilterProjects(projectOrSolution.AsSolution(), getProjects))
                yield return project;
        }
    }

    private protected IEnumerable<Project> FilterProjects(
        Solution solution,
        Func<Solution, ImmutableArray<ProjectId>> getProjects = null)
    {
        Workspace workspace = solution.Workspace;

        foreach (ProjectId projectId in (getProjects is not null) ? getProjects(solution) : solution.ProjectIds)
        {
            Project project = workspace.CurrentSolution.GetProject(projectId);

            if (IsMatch(project))
            {
                yield return project;
            }
            else
            {
                WriteLine($"  Skip '{project.Name}'", ConsoleColors.DarkGray, Verbosity.Normal);
            }
        }
    }

    private protected bool IsMatch(Project project)
    {
        return ProjectFilter.IsMatch(project);
    }

    private protected async Task<ImmutableArray<Compilation>> GetCompilationsAsync(
        ProjectOrSolution projectOrSolution,
        CancellationToken cancellationToken)
    {
        if (projectOrSolution.IsProject)
        {
            Project project = projectOrSolution.AsProject();

            WriteLine($"Compile '{project.Name}'", Verbosity.Minimal);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken);

            return ImmutableArray.Create(compilation);
        }
        else
        {
            ImmutableArray<Compilation>.Builder compilations = ImmutableArray.CreateBuilder<Compilation>();

            Solution solution = projectOrSolution.AsSolution();

            WriteLine($"Compiling solution '{solution.FilePath}'", Verbosity.Minimal);

            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (Project project in FilterProjects(
                solution,
                s => s
                    .GetProjectDependencyGraph()
                    .GetTopologicallySortedProjects(cancellationToken)
                    .ToImmutableArray()))
            {
                cancellationToken.ThrowIfCancellationRequested();

                WriteLine($"  Compile '{project.Name}'", Verbosity.Minimal);

                Compilation compilation = await project.GetCompilationAsync(cancellationToken);

                compilations.Add(compilation);
            }

            stopwatch.Stop();

            LogHelpers.WriteElapsedTime($"Compiled solution '{solution.FilePath}'", stopwatch.Elapsed, Verbosity.Minimal);

            return compilations.ToImmutableArray();
        }
    }

    private static bool IsSolutionFile(string path)
    {
        string extension = Path.GetExtension(path);

        return string.Equals(extension, ".sln", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".slnf", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".slnx", StringComparison.OrdinalIgnoreCase);
    }

    protected class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
    {
        public static ConsoleProgressReporter Default { get; } = new();

        public Dictionary<string, List<string>> Projects { get; }

        public ConsoleProgressReporter(bool shouldSaveProgress = false)
        {
            if (shouldSaveProgress)
                Projects = new Dictionary<string, List<string>>();
        }

        public void Report(ProjectLoadProgress value)
        {
            string text = Path.GetFileName(value.FilePath);

            ProjectLoadOperation operation = value.Operation;

            if (operation == ProjectLoadOperation.Resolve)
            {
                string targetFramework = value.TargetFramework;

                if (targetFramework is not null)
                    text += $" ({targetFramework})";

                if (Projects is not null)
                {
                    if (!Projects.TryGetValue(value.FilePath, out List<string> targetFrameworks))
                    {
                        if (targetFramework is not null)
                            targetFrameworks = new List<string>();

                        Projects[value.FilePath] = targetFrameworks;
                    }

                    if (targetFramework is not null)
                        targetFrameworks.Add(targetFramework);
                }
            }

            text = $"  {operation,-9} {value.ElapsedTime:mm\\:ss\\.ff}  {text}";

            if (operation == ProjectLoadOperation.Resolve)
            {
                WriteLine(text, Verbosity.Detailed);
            }
            else
            {
                WriteLine(text, ConsoleColors.DarkGray, Verbosity.Diagnostic);
            }
        }
    }
}
