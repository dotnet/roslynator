// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
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
    public abstract class CompilerCodeFixVerifier : CodeVerifier
    {
        public abstract string DiagnosticId { get; }

        public abstract CodeFixProvider FixProvider { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{DiagnosticId} {FixProvider.GetType().Name}"; }
        }

        public async Task VerifyFixAsync(
            string theory,
            string fromData,
            string toData,
            string equivalenceKey)
        {
            (string source, string expected, TextSpan span) = TestSourceText.ReplaceSpan(theory, fromData, toData);

            await VerifyFixAsync(
                source: source,
                expected: expected,
                equivalenceKey: equivalenceKey).ConfigureAwait(false);
        }

        public async Task VerifyFixAsync(
            string source,
            string expected,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Assert.True(FixProvider.FixableDiagnosticIds.Contains(DiagnosticId), $"Code fix provider '{FixProvider.GetType().Name}' cannot fix diagnostic '{DiagnosticId}'.");

            Document document = WorkspaceFactory.Document(source);

            Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(cancellationToken: cancellationToken);

            diagnostics = diagnostics.Sort((x, y) => -DiagnosticComparer.SpanStart.Compare(x, y));

            bool fixRegistered = false;

            while (diagnostics.Length > 0)
            {
                Diagnostic diagnostic = FindDiagnostic();

                if (diagnostic == null)
                    break;

                CodeAction action = null;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, d) =>
                    {
                        if (action != null)
                            return;

                        if (!d.Contains(diagnostic))
                            return;

                        if (equivalenceKey != null
                            && !string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                        {
                            return;
                        }

                        action = a;
                    },
                    CancellationToken.None);

                await FixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                if (action == null)
                    break;

                fixRegistered = true;

                document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

                compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<Diagnostic> newDiagnostics = compilation.GetDiagnostics(cancellationToken: cancellationToken);

                if (!Options.AllowNewCompilerDiagnostics)
                    VerifyNoNewCompilerDiagnostics(diagnostics, newDiagnostics);

                diagnostics = newDiagnostics;
            }

            Assert.True(fixRegistered, "No code fix has been registered.");

            string actual = await document.ToFullStringAsync(simplify: true, format: true).ConfigureAwait(false);

            Assert.Equal(expected, actual);

            Diagnostic FindDiagnostic()
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (string.Equals(diagnostic.Id, DiagnosticId, StringComparison.Ordinal))
                        return diagnostic;
                }

                return null;
            }
        }

        public async Task VerifyNoFixAsync(
            string source,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Document document = WorkspaceFactory.Document(source);

            Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<string> fixableDiagnosticIds = FixProvider.FixableDiagnosticIds;

            foreach (Diagnostic diagnostic in compilation.GetDiagnostics(cancellationToken: cancellationToken))
            {
                if (!fixableDiagnosticIds.Contains(diagnostic.Id))
                    continue;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, d) =>
                    {
                        if (!d.Contains(diagnostic))
                            return;

                        if (equivalenceKey != null
                            && !string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                        {
                            return;
                        }

                        Assert.True(false, "Expected no code fix.");
                    },
                    CancellationToken.None);

                await FixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            }
        }
    }
}
