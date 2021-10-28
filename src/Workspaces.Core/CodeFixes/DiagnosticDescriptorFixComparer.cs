// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.CodeFixes
{
    internal class DiagnosticDescriptorFixComparer : IComparer<DiagnosticDescriptor>
    {
        public DiagnosticDescriptorFixComparer(
            Dictionary<DiagnosticDescriptor, int> diagnosticCountByDescriptor,
            Dictionary<string, ImmutableArray<CodeFixProvider>> fixersById)
        {
            DiagnosticCountByDescriptor = diagnosticCountByDescriptor;
            FixersById = fixersById;
        }

        private Dictionary<DiagnosticDescriptor, int> DiagnosticCountByDescriptor { get; }

        private Dictionary<string, ImmutableArray<CodeFixProvider>> FixersById { get; }

        public int Compare(DiagnosticDescriptor x, DiagnosticDescriptor y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (HasFixAll(x))
            {
                if (!HasFixAll(y))
                    return -1;
            }
            else if (HasFixAll(y))
            {
                return 1;
            }

            return DiagnosticCountByDescriptor[y].CompareTo(DiagnosticCountByDescriptor[x]);
        }

        private bool HasFixAll(DiagnosticDescriptor descriptor)
        {
            return FixersById[descriptor.Id].Any(f => f.HasFixAllProvider(FixAllScope.Project));
        }
    }
}
