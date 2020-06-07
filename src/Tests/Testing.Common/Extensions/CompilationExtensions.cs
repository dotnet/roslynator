// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    internal static class CompilationExtensions
    {
        public static Compilation EnsureEnabled(this Compilation compilation, DiagnosticDescriptor descriptor)
        {
            CompilationOptions compilationOptions = compilation.Options;

            ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions = compilationOptions.SpecificDiagnosticOptions;

            specificDiagnosticOptions = specificDiagnosticOptions.SetItem(
                descriptor.Id,
                descriptor.DefaultSeverity.ToReportDiagnostic());

            return compilation.WithOptions(compilationOptions.WithSpecificDiagnosticOptions(specificDiagnosticOptions));
        }

        public static Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(
            this Compilation compilation,
            DiagnosticAnalyzer analyzer,
            IComparer<Diagnostic> comparer = null,
            CancellationToken cancellationToken = default)
        {
            return GetAnalyzerDiagnosticsAsync(
                compilation,
                ImmutableArray.Create(analyzer),
                comparer,
                cancellationToken);
        }

        public static async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(
            this Compilation compilation,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            IComparer<Diagnostic> comparer = null,
            CancellationToken cancellationToken = default)
        {
            CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(analyzers, default(AnalyzerOptions), cancellationToken);

            ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken).ConfigureAwait(false);

            if (comparer != null)
                diagnostics = diagnostics.Sort(comparer);

            return diagnostics;
        }
    }
}
