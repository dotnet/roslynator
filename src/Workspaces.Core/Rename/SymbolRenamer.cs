// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Roslynator.Rename;

/// <summary>
/// Provides a set of static methods for renaming symbols in a solution or a project.
/// </summary>
public static class SymbolRenamer
{
    /// <summary>
    /// Renames symbols in the specified solution.
    /// </summary>
    /// <param name="solution"></param>
    /// <param name="predicate"></param>
    /// <param name="getNewName"></param>
    /// <param name="options"></param>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    public static async Task RenameSymbolsAsync(
        Solution solution,
        Func<ISymbol, bool> predicate,
        Func<ISymbol, string> getNewName,
        SymbolRenamerOptions options = null,
        IProgress<SymbolRenameProgress> progress = null,
        CancellationToken cancellationToken = default)
    {
        var renamer = new SymbolRenameState(solution, predicate, getNewName, options, progress);

        await renamer.RenameSymbolsAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Renames symbols in the specified projects. All projects must be in the same solution.
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="predicate"></param>
    /// <param name="getNewName"></param>
    /// <param name="options"></param>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task RenameSymbolsAsync(
        IEnumerable<Project> projects,
        Func<ISymbol, bool> predicate,
        Func<ISymbol, string> getNewName,
        SymbolRenamerOptions options = null,
        IProgress<SymbolRenameProgress> progress = null,
        CancellationToken cancellationToken = default)
    {
        if (projects is null)
            throw new ArgumentNullException(nameof(projects));

        List<Project> projectList = EnumerateProjects().ToList();

        if (projectList.Count == 0)
            throw new ArgumentException("Sequence of projects contains no elements.", nameof(projects));

        var renamer = new SymbolRenameState(projectList[0].Solution, predicate, getNewName, options, progress);

        await renamer.RenameSymbolsAsync(projectList, cancellationToken).ConfigureAwait(false);

        IEnumerable<Project> EnumerateProjects()
        {
            using (IEnumerator<Project> en = projects.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    Solution solution = en.Current.Solution;

                    yield return en.Current;

                    while (en.MoveNext())
                    {
                        if (en.Current.Solution.Id == solution.Id)
                        {
                            yield return en.Current;
                        }
                        else
                        {
                            throw new InvalidOperationException("All projects must be from the same solution.");
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Renames symbols in the specified project.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="predicate"></param>
    /// <param name="getNewName"></param>
    /// <param name="options"></param>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    public static async Task RenameSymbolsAsync(
        Project project,
        Func<ISymbol, bool> predicate,
        Func<ISymbol, string> getNewName,
        SymbolRenamerOptions options = null,
        IProgress<SymbolRenameProgress> progress = null,
        CancellationToken cancellationToken = default)
    {
        var renamer = new SymbolRenameState(project.Solution, predicate, getNewName, options, progress);

        await renamer.RenameSymbolsAsync(project, cancellationToken).ConfigureAwait(false);
    }
}
