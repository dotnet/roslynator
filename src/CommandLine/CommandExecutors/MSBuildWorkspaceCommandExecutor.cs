// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal abstract class MSBuildWorkspaceCommandExecutor
    {
        protected MSBuildWorkspaceCommandExecutor(string language)
        {
            Language = language;
        }

        public string Language { get; }

        public abstract Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default);

        public async Task<CommandResult> ExecuteAsync(string path, string msbuildPath = null, IEnumerable<string> properties = null)
        {
            MSBuildWorkspace workspace = null;

            try
            {
                workspace = CreateMSBuildWorkspace(msbuildPath, properties);

                if (workspace == null)
                    return CommandResult.Fail;

                workspace.WorkspaceFailed += WorkspaceFailed;

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                CancellationToken cancellationToken = cts.Token;

                try
                {
                    CommandResult result = await ExecuteAsync(path, workspace, ConsoleProgressReporter.Default, cancellationToken);

                    if (result.Kind != CommandResultKind.None)
                        return result;

                    ProjectOrSolution projectOrSolution = await OpenProjectOrSolutionAsync(path, workspace, ConsoleProgressReporter.Default, cancellationToken);

                    if (projectOrSolution != default)
                        return await ExecuteAsync(projectOrSolution, cancellationToken);
                }
                catch (OperationCanceledException ex)
                {
                    OperationCanceled(ex);
                }
                catch (AggregateException ex)
                {
                    OperationCanceledException operationCanceledException = ex.GetOperationCanceledException();

                    if (operationCanceledException != null)
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

            return CommandResult.Fail;
        }

        protected virtual void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Operation was canceled.", Verbosity.Quiet);
        }

        protected virtual void WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            WriteLine($"  {e.Diagnostic.Message}", e.Diagnostic.Kind.GetColor(), Verbosity.Detailed);
        }

        protected virtual Task<CommandResult> ExecuteAsync(
            string path,
            MSBuildWorkspace workspace,
            IProgress<ProjectLoadProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(default(CommandResult));
        }

        protected virtual async Task<ProjectOrSolution> OpenProjectOrSolutionAsync(
            string path,
            MSBuildWorkspace workspace,
            IProgress<ProjectLoadProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            bool isSolution = string.Equals(Path.GetExtension(path), ".sln", StringComparison.OrdinalIgnoreCase);

            WriteLine($"Load {((isSolution) ? "solution" : "project")} '{path}'", Verbosity.Minimal);

            try
            {
                ProjectOrSolution projectOrSolution;

                if (isSolution)
                {
                    projectOrSolution = await workspace.OpenSolutionAsync(path, progress, cancellationToken);
                }
                else
                {
                    projectOrSolution = await workspace.OpenProjectAsync(path, progress, cancellationToken);
                }

                WriteLine($"Done loading {((projectOrSolution.IsSolution) ? "solution" : "project")} '{projectOrSolution.FilePath}'", Verbosity.Minimal);

                return projectOrSolution;
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException
                    || ex is InvalidOperationException)
                {
                    WriteLine(ex.ToString(), ConsoleColor.Red, Verbosity.Minimal);
                    return default;
                }
                else
                {
                    throw;
                }
            }
        }

        private static MSBuildWorkspace CreateMSBuildWorkspace(string msbuildPath, IEnumerable<string> rawProperties)
        {
            if (msbuildPath != null)
            {
                MSBuildLocator.RegisterMSBuildPath(msbuildPath);
            }
            else
            {
                VisualStudioInstance instance;

                try
                {
                    instance = MSBuildLocator.RegisterDefaults();
                }
                catch (InvalidOperationException)
                {
                    WriteLine("MSBuild location not found. Use option '--msbuild-path' to specify MSBuild location", ConsoleColor.Red, Verbosity.Quiet);
                    return null;
                }

                msbuildPath = instance.MSBuildPath;
            }

            WriteLine($"MSBuild location is '{msbuildPath}'", Verbosity.Diagnostic);

            if (!ParseHelpers.TryParseMSBuildProperties(rawProperties, out Dictionary<string, string> properties))
                return null;

            if (properties == null)
                properties = new Dictionary<string, string>();

            // https://github.com/Microsoft/MSBuildLocator/issues/16
            if (!properties.ContainsKey("AlwaysCompileMarkupFilesInSeparateDomain"))
                properties["AlwaysCompileMarkupFilesInSeparateDomain"] = bool.FalseString;

            return MSBuildWorkspace.Create(properties);
        }

        private protected IEnumerable<Project> FilterProjects(
            Solution solution,
            MSBuildCommandLineOptions options,
            Func<Solution, ImmutableArray<ProjectId>> getProjects = null)
        {
            ImmutableHashSet<string> projectNames = options.GetProjectNames();

            ImmutableHashSet<string> ignoredProjectNames = options.GetIgnoredProjectNames();

            Workspace workspace = solution.Workspace;

            foreach (ProjectId projectId in (getProjects != null) ? getProjects(solution) : solution.ProjectIds)
            {
                Project project = workspace.CurrentSolution.GetProject(projectId);

                if (SyntaxFactsService.IsSupportedLanguage(project.Language)
                    && (Language == null || Language == project.Language)
                    && ((projectNames.Count > 0) ? projectNames.Contains(project.Name) : !ignoredProjectNames.Contains(project.Name)))
                {
                    yield return project;
                }
                else
                {
                    WriteLine($"  Skip '{project.Name}'", ConsoleColor.DarkGray, Verbosity.Normal);
                }
            }
        }

        protected class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public static ConsoleProgressReporter Default { get; } = new ConsoleProgressReporter();

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

                    if (targetFramework != null)
                        text += $" ({targetFramework})";

                    if (Projects != null)
                    {
                        if (!Projects.TryGetValue(value.FilePath, out List<string> targetFrameworks))
                        {
                            if (targetFramework != null)
                                targetFrameworks = new List<string>();

                            Projects[value.FilePath] = targetFrameworks;
                        }

                        if (targetFramework != null)
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
                    WriteLine(text, ConsoleColor.DarkGray, Verbosity.Diagnostic);
                }
            }
        }
    }
}
