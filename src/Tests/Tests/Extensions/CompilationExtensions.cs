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
        public static Compilation EnableDiagnosticsDisabledByDefault(this Compilation compilation, DiagnosticAnalyzer analyzer)
        {
            return EnableDiagnosticsDisabledByDefault(compilation, analyzer.SupportedDiagnostics);
        }

        public static Compilation EnableDiagnosticsDisabledByDefault(this Compilation compilation, ImmutableArray<DiagnosticDescriptor> diagnosticDescriptors)
        {
            CompilationOptions options = compilation.Options;
            ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions = options.SpecificDiagnosticOptions;

            int count = specificDiagnosticOptions.Count;

            foreach (DiagnosticDescriptor descriptor in diagnosticDescriptors)
            {
                if (!descriptor.IsEnabledByDefault)
                {
                    specificDiagnosticOptions = specificDiagnosticOptions.Add(
                        descriptor.Id,
                        descriptor.DefaultSeverity.ToReportDiagnostic());
                }
            }

            if (specificDiagnosticOptions.Count != count)
            {
                options = options.WithSpecificDiagnosticOptions(specificDiagnosticOptions);

                compilation = compilation.WithOptions(options);
            }

            return compilation;
        }

        public static async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(
            this Compilation compilation,
            DiagnosticAnalyzer analyzer,
            IComparer<Diagnostic> comparer = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzer), default(AnalyzerOptions), cancellationToken);

            ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken).ConfigureAwait(false);

            if (comparer != null)
                diagnostics = diagnostics.Sort(comparer);

            return diagnostics;
        }
    }
}
