// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CodeFixes
{
    internal static class CodeFixerHelpers
    {
        public static Dictionary<string, ImmutableArray<CodeFixProvider>> GetFixersById(
            ImmutableArray<CodeFixProvider> fixers,
            CodeFixerOptions options)
        {
            return fixers
                .Where(f =>
                {
                    if (f.HasFixAllProvider(FixAllScope.Project))
                        return true;

                    if (options.DiagnosticIdsFixableOneByOne.Count > 0)
                    {
                        foreach (string diagnosticId in f.FixableDiagnosticIds)
                        {
                            if (options.DiagnosticIdsFixableOneByOne.Contains(diagnosticId))
                                return true;
                        }
                    }

                    return false;
                })
                .SelectMany(f => f.FixableDiagnosticIds.Select(id => (id, fixer: f)))
                .GroupBy(f => f.id)
                .ToDictionary(f => f.Key, g => g.Select(f => f.fixer).Distinct().ToImmutableArray());
        }

        public static Dictionary<string, ImmutableArray<DiagnosticAnalyzer>> GetAnalyzersById(
            ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            return analyzers
                .SelectMany(f => f.SupportedDiagnostics.Select(d => (id: d.Id, analyzer: f)))
                .GroupBy(f => f.id, f => f.analyzer)
                .ToDictionary(g => g.Key, g => g.Select(analyzer => analyzer).Distinct().ToImmutableArray());
        }
    }
}
