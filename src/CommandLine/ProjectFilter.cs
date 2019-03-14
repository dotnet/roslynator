// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

            Names = names?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            IgnoredNames = ignoredNames?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            Language = language;
        }

        public ImmutableHashSet<string> Names { get; }

        public ImmutableHashSet<string> IgnoredNames { get; }

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
                return Names.Contains(project.Name);

            if (IgnoredNames?.Count > 0)
                return !IgnoredNames.Contains(project.Name);

            return true;
        }
    }
}
