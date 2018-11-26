// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

            bool hasFixAll2 = FixersById[y.Id].Any(f => f.HasFixAllProvider(FixAllScope.Project));

            if (FixersById[x.Id].Any(f => f.HasFixAllProvider(FixAllScope.Project)))
            {
                if (!hasFixAll2)
                {
                    return -1;
                }
            }
            else if (hasFixAll2)
            {
                return 1;
            }

            return DiagnosticCountByDescriptor[y].CompareTo(DiagnosticCountByDescriptor[x]);
        }
    }
}
