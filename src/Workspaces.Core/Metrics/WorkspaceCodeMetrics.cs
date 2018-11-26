// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CodeMetrics
{
    internal static class WorkspaceCodeMetrics
    {
        public static ImmutableDictionary<ProjectId, CodeMetricsInfo> CountLinesInParallel(
            IEnumerable<Project> projects,
            Func<string, CodeMetricsCounter> counterFactory,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            var codeMetrics = new ConcurrentBag<(ProjectId projectId, CodeMetricsInfo codeMetrics)>();

            Parallel.ForEach(projects, project =>
            {
                CodeMetricsCounter counter = counterFactory(project.Language);

                CodeMetricsInfo projectMetrics = (counter != null)
                    ? CountLinesAsync(project, counter, options, cancellationToken).Result
                    : CodeMetricsInfo.NotAvailable;

                codeMetrics.Add((project.Id, codeMetrics: projectMetrics));
            });

            return codeMetrics.ToImmutableDictionary(f => f.projectId, f => f.codeMetrics);
        }

        public static async Task<ImmutableDictionary<ProjectId, CodeMetricsInfo>> CountLinesAsync(
            IEnumerable<Project> projects,
            Func<string, CodeMetricsCounter> counterFactory,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            ImmutableDictionary<ProjectId, CodeMetricsInfo>.Builder builder = ImmutableDictionary.CreateBuilder<ProjectId, CodeMetricsInfo>();

            foreach (Project project in projects)
            {
                CodeMetricsCounter counter = counterFactory(project.Language);

                CodeMetricsInfo projectMetrics = (counter != null)
                    ? await CountLinesAsync(project, counter, options, cancellationToken).ConfigureAwait(false)
                    : CodeMetricsInfo.NotAvailable;

                builder.Add(project.Id, projectMetrics);
            }

            return builder.ToImmutableDictionary();
        }

        public static async Task<CodeMetricsInfo> CountLinesAsync(
            Project project,
            CodeMetricsCounter counter,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            CodeMetricsInfo codeMetrics = default;

            foreach (Document document in project.Documents)
            {
                if (!document.SupportsSyntaxTree)
                    continue;

                CodeMetricsInfo documentMetrics = await CountLinesAsync(document, counter, options, cancellationToken).ConfigureAwait(false);

                codeMetrics = codeMetrics.Add(documentMetrics);
            }

            return codeMetrics;
        }

        public static async Task<CodeMetricsInfo> CountLinesAsync(
            Document document,
            CodeMetricsCounter counter,
            CodeMetricsOptions options = null,
            CancellationToken cancellationToken = default)
        {
            SyntaxTree tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);

            if (tree == null)
                return default;

            if (!options.IncludeGeneratedCode
                && GeneratedCodeUtility.IsGeneratedCode(tree, counter.SyntaxFacts.IsComment, cancellationToken))
            {
                return default;
            }

            SyntaxNode root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            return counter.CountLines(root, sourceText, options, cancellationToken);
        }
    }
}
