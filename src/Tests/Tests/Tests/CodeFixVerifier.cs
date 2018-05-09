// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Tests.Text;
using Xunit;

namespace Roslynator.Tests
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class CodeFixVerifier : DiagnosticVerifier
    {
        public abstract CodeFixProvider FixProvider { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{Descriptor.Id} {Analyzer.GetType().Name} {FixProvider.GetType().Name}"; }
        }

        public async Task VerifyDiagnosticAndFixAsync(
            string source,
            string expected,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TestSourceTextAnalysis analysis = TestSourceText.GetSpans(source);

            IEnumerable<Diagnostic> diagnostics = analysis.Spans.Select(f => CreateDiagnostic(f.Span, f.LineSpan));

            await VerifyDiagnosticAsync(analysis.Source, diagnostics, cancellationToken).ConfigureAwait(false);

            await VerifyFixAsync(analysis.Source, expected, cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyDiagnosticAndFixAsync(
            string theory,
            string fromData,
            string toData,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string source, string expected, TextSpan span) = TestSourceText.ReplaceSpan(theory, fromData, toData);

            TestSourceTextAnalysis analysis = TestSourceText.GetSpans(source);

            if (analysis.Spans.Any())
            {
                IEnumerable<Diagnostic> diagnostics = analysis.Spans.Select(f => CreateDiagnostic(f.Span, f.LineSpan));

                await VerifyDiagnosticAsync(analysis.Source, diagnostics, cancellationToken).ConfigureAwait(false);

                await VerifyFixAsync(analysis.Source, expected, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await VerifyDiagnosticAsync(source, span, cancellationToken).ConfigureAwait(false);

                await VerifyFixAsync(source, expected, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task VerifyFixAsync(
            string source,
            string expected,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Assert.True(FixProvider.CanFixAny(Analyzer.SupportedDiagnostics), $"Code fix provider '{FixProvider.GetType().Name}' cannot fix any diagnostic supported by analyzer '{Analyzer}'.");

            Document document = WorkspaceFactory.Document(source);

            Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics);

            if (Options.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(Analyzer);

            ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(Analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);

            ImmutableArray<string> fixableDiagnosticIds = FixProvider.FixableDiagnosticIds;

            bool fixRegistered = false;

            while (diagnostics.Length > 0)
            {
                Diagnostic diagnostic = FindFirstFixableDiagnostic();

                if (diagnostic == null)
                    break;

                CodeAction action = null;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, d) =>
                    {
                        if (d.Contains(diagnostic)
                            && action == null)
                        {
                            action = a;
                        }
                    },
                    CancellationToken.None);

                await FixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                if (action == null)
                    break;

                fixRegistered = true;

                document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

                compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> newCompilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

                VerifyCompilerDiagnostics(newCompilerDiagnostics);

                if (!Options.AllowNewCompilerDiagnostics)
                    VerifyNoNewCompilerDiagnostics(compilerDiagnostics, newCompilerDiagnostics);

                if (Options.EnableDiagnosticsDisabledByDefault)
                    compilation = compilation.EnableDiagnosticsDisabledByDefault(Analyzer);

                diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(Analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);
            }

            Assert.True(fixRegistered, "No code fix has been registered.");

            string actual = await document.ToFullStringAsync(simplify: true, format: true).ConfigureAwait(false);

            Assert.Equal(expected, actual);

            Diagnostic FindFirstFixableDiagnostic()
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (fixableDiagnosticIds.Contains(diagnostic.Id))
                        return diagnostic;
                }

                return null;
            }
        }

        public async Task VerifyNoFixAsync(string source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Document document = WorkspaceFactory.Document(source);

            Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics);

            if (Options.EnableDiagnosticsDisabledByDefault)
                compilation = compilation.EnableDiagnosticsDisabledByDefault(Analyzer);

            ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync(Analyzer, DiagnosticComparer.SpanStart, cancellationToken).ConfigureAwait(false);

            ImmutableArray<string> fixableDiagnosticIds = FixProvider.FixableDiagnosticIds;

            foreach (Diagnostic diagnostic in diagnostics)
            {
                if (!string.Equals(diagnostic.Id, Descriptor.Id, StringComparison.Ordinal))
                    continue;

                if (!fixableDiagnosticIds.Contains(diagnostic.Id))
                    continue;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (_, d) => Assert.True(!d.Contains(diagnostic), "Expected no code fix."),
                    CancellationToken.None);

                await FixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            }
        }
    }
}
