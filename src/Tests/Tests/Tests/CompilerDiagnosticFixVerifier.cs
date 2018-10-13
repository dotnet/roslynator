// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Xunit;
using static Roslynator.Tests.CompilerDiagnosticVerifier;

namespace Roslynator.Tests
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class CompilerDiagnosticFixVerifier : CodeVerifier
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
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (TextSpan span, string source, string expected) = SpanParser.ReplaceEmptySpan(theory, fromData, toData);

            await VerifyFixAsync(
                source: source,
                expected: expected,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyFixAsync(
            string source,
            string expected,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!FixProvider.FixableDiagnosticIds.Contains(DiagnosticId))
                Assert.True(false, $"Code fix provider '{FixProvider.GetType().Name}' cannot fix diagnostic '{DiagnosticId}'.");

            Document document = CreateDocument(source);

            Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(cancellationToken: cancellationToken);

            diagnostics = diagnostics.Sort((x, y) => -DiagnosticComparer.SpanStart.Compare(x, y));

            bool fixRegistered = false;

            while (diagnostics.Length > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

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

                if (options == null)
                    options = Options;

                if (!options.AllowNewCompilerDiagnostics)
                    VerifyNoNewCompilerDiagnostics(diagnostics, newDiagnostics, options);

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

        [SuppressMessage("Redundancy", "RCS1163:Unused parameter.", Justification = "<Pending>")]
        public async Task VerifyNoFixAsync(
            string source,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Document document = CreateDocument(source);

            Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<string> fixableDiagnosticIds = FixProvider.FixableDiagnosticIds;

            foreach (Diagnostic diagnostic in compilation.GetDiagnostics(cancellationToken: cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

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

                        Assert.True(false, "No code fix expected.");
                    },
                    CancellationToken.None);

                await FixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            }
        }
    }
}
