// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CodeMetrics;
using Roslynator.Host.Mef;
using static Roslynator.Logger;

namespace Roslynator.CommandLine;

internal abstract class AbstractLinesOfCodeCommand<TResult> : MSBuildWorkspaceCommand<LinesOfCodeCommandResult>
{
    protected AbstractLinesOfCodeCommand(in ProjectFilter projectFilter, FileSystemFilter fileSystemFilter) : base(projectFilter, fileSystemFilter)
    {
    }

    public async Task<ImmutableDictionary<ProjectId, CodeMetricsInfo>> CountLinesAsync(
        IEnumerable<Project> projects,
        LinesOfCodeKind kind,
        CodeMetricsOptions options = null,
        CancellationToken cancellationToken = default)
    {
        var codeMetrics = new ConcurrentBag<(ProjectId projectId, CodeMetricsInfo codeMetrics)>();

#if NETFRAMEWORK
        await Task.CompletedTask;

        Parallel.ForEach(
            projects,
            project =>
            {
                ICodeMetricsService service = MefWorkspaceServices.Default.GetService<ICodeMetricsService>(project.Language);

                CodeMetricsInfo projectMetrics = (service is not null)
                    ? service.CountLinesAsync(project, kind, FileSystemFilter, options, cancellationToken).Result
                    : CodeMetricsInfo.NotAvailable;

                codeMetrics.Add((project.Id, codeMetrics: projectMetrics));
            });
#else
        await Parallel.ForEachAsync(
            projects,
            cancellationToken,
            async (project, cancellationToken) =>
            {
                ICodeMetricsService service = MefWorkspaceServices.Default.GetService<ICodeMetricsService>(project.Language);

                CodeMetricsInfo projectMetrics = (service is not null)
                    ? await service.CountLinesAsync(project, kind, FileSystemFilter, options, cancellationToken)
                    : CodeMetricsInfo.NotAvailable;

                codeMetrics.Add((project.Id, codeMetrics: projectMetrics));
            });
#endif
        return codeMetrics.ToImmutableDictionary(f => f.projectId, f => f.codeMetrics);
    }

    protected static void WriteLinesOfCode(Solution solution, ImmutableDictionary<ProjectId, CodeMetricsInfo> projectsMetrics)
    {
        int maxDigits = projectsMetrics.Max(f => f.Value.CodeLineCount).ToString("n0").Length;
        int maxNameLength = projectsMetrics.Max(f => solution.GetProject(f.Key).Name.Length);

        foreach (KeyValuePair<ProjectId, CodeMetricsInfo> kvp in projectsMetrics
            .OrderByDescending(f => f.Value.CodeLineCount)
            .ThenBy(f => solution.GetProject(f.Key).Name))
        {
            Project project = solution.GetProject(kvp.Key);
            CodeMetricsInfo codeMetrics = kvp.Value;

            string count = (codeMetrics.CodeLineCount >= 0)
                ? codeMetrics.CodeLineCount.ToString("n0").PadLeft(maxDigits)
                : "-";

            WriteLine($"{count} {project.Name.PadRight(maxNameLength)} {project.Language}", Verbosity.Normal);
        }
    }
}
