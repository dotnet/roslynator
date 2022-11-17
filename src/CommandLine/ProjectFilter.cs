// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.CommandLine
{
    internal readonly struct ProjectFilter
    {
        public ProjectFilter(
            IEnumerable<string> names,
            IEnumerable<string> ignoredNames,
            string language)
        {
            if (names?.Any() == true
                && ignoredNames?.Any() == true)
            {
                throw new ArgumentException($"Cannot specify both '{nameof(names)}' and '{nameof(ignoredNames)}'.", nameof(names));
            }

            Names = names?.Select(f => ProjectName.Create(f)).ToImmutableHashSet() ?? ImmutableHashSet<ProjectName>.Empty;
            IgnoredNames = ignoredNames?.Select(f => ProjectName.Create(f)).ToImmutableHashSet() ?? ImmutableHashSet<ProjectName>.Empty;
            Language = language;
        }

        public ImmutableHashSet<ProjectName> Names { get; }

        public ImmutableHashSet<ProjectName> IgnoredNames { get; }

        public string Language { get; }

        public bool IsDefault
        {
            get
            {
                return Names == null
                    && IgnoredNames == null
                    && Language == null;
            }
        }

        public bool IsMatch(Project project)
        {
            if (Language != null
                && Language != project.Language)
            {
                return false;
            }

            if (Names?.Count > 0)
                return IsMatch(project.Name, Names);

            if (IgnoredNames?.Count > 0)
                return !IsMatch(project.Name, IgnoredNames);

            return true;
        }

        private static bool IsMatch(string name, ImmutableHashSet<ProjectName> projectNames)
        {
            ProjectName projectName = ProjectName.Create(name);

            foreach (ProjectName projectName2 in projectNames)
            {
                if (projectName2.Moniker != null)
                {
                    if (projectName.Moniker != null
                        && string.Equals(projectName.Name, projectName2.Name, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
                else if (string.Equals(projectName.NameWithoutMoniker, projectName2.NameWithoutMoniker, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
