// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.CodeMetrics;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal abstract class AbstractLinesOfCodeCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        protected AbstractLinesOfCodeCommandExecutor(string language) : base(language)
        {
        }

        internal static void WriteLinesOfCode(Solution solution, ImmutableDictionary<ProjectId, CodeMetricsInfo> projectsMetrics)
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
}
