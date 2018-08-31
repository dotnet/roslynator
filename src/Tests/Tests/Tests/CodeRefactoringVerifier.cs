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
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Tests.Text;
using Xunit;
using static Roslynator.Tests.CompilerDiagnosticVerifier;

namespace Roslynator.Tests
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class CodeRefactoringVerifier : CodeVerifier
    {
        public abstract string RefactoringId { get; }

        public abstract CodeRefactoringProvider RefactoringProvider { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{RefactoringId} {RefactoringProvider.GetType().Name}"; }
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            string equivalenceKey = null,
            string[] additionalSources = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TextSpanParserResult result = SpanParser.GetSpans(source, reverse: true);

            await VerifyRefactoringAsync(
                source: result.Text,
                expected: expected,
                spans: result.Spans.Select(f => f.Span),
                equivalenceKey: equivalenceKey,
                additionalSources: additionalSources,
                options: options,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string theory,
            string fromData,
            string toData,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (TextSpan span, string source, string expected) = SpanParser.ReplaceEmptySpan(theory, fromData, toData);

            TextSpanParserResult result = SpanParser.GetSpans(source, reverse: true);

            if (result.Spans.Any())
            {
                await VerifyRefactoringAsync(
                    source: result.Text,
                    expected: expected,
                    spans: result.Spans.Select(f => f.Span),
                    equivalenceKey: equivalenceKey,
                    options: options,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await VerifyRefactoringAsync(
                    source: source,
                    expected: expected,
                    span: span,
                    equivalenceKey: equivalenceKey,
                    options: options,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            IEnumerable<TextSpan> spans,
            string equivalenceKey = null,
            string[] additionalSources = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (IEnumerator<TextSpan> en = spans.GetEnumerator())
            {
                if (!en.MoveNext())
                    throw new InvalidOperationException($"'{nameof(spans)}' contains no elements.");

                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await VerifyRefactoringAsync(
                        source: source,
                        expected: expected,
                        en.Current,
                        equivalenceKey: equivalenceKey,
                        additionalSources: additionalSources,
                        options: options,
                        cancellationToken: cancellationToken).ConfigureAwait(false);

                } while (en.MoveNext());
            }
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            TextSpan span,
            string equivalenceKey = null,
            string[] additionalSources = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Document document = CreateDocument(source, additionalSources ?? Array.Empty<string>());

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            if (options == null)
                options = Options;

            VerifyCompilerDiagnostics(compilerDiagnostics, options);
            CodeAction action = null;

            var context = new CodeRefactoringContext(
                document,
                span,
                a =>
                {
                    if (equivalenceKey == null
                        || string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                    {
                        if (action == null)
                            action = a;
                    }
                },
                CancellationToken.None);

            await RefactoringProvider.ComputeRefactoringsAsync(context).ConfigureAwait(false);

            Assert.True(action != null, "No code refactoring has been registered.");

            document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

            semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> newCompilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            VerifyCompilerDiagnostics(newCompilerDiagnostics, options);

            if (!options.AllowNewCompilerDiagnostics)
                VerifyNoNewCompilerDiagnostics(compilerDiagnostics, newCompilerDiagnostics, options);

            string actual = await document.ToFullStringAsync(simplify: true, format: true).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TextSpanParserResult result = SpanParser.GetSpans(source, reverse: true);

            await VerifyNoRefactoringAsync(
                source: result.Text,
                spans: result.Spans.Select(f => f.Span),
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            TextSpan span,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyNoRefactoringAsync(
                source,
                ImmutableArray.Create(span),
                equivalenceKey,
                options,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            IEnumerable<TextSpan> spans,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Document document = CreateDocument(source);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            if (options == null)
                options = Options;

            VerifyCompilerDiagnostics(compilerDiagnostics, options);

            using (IEnumerator<TextSpan> en = spans.GetEnumerator())
            {
                if (!en.MoveNext())
                    throw new InvalidOperationException($"'{nameof(spans)}' contains no elements.");

                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var context = new CodeRefactoringContext(
                        document,
                        en.Current,
                        a =>
                        {
                            if (equivalenceKey == null
                                || string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                            {
                                Assert.True(false, "No code refactoring expected.");
                            }
                        },
                        CancellationToken.None);

                    await RefactoringProvider.ComputeRefactoringsAsync(context).ConfigureAwait(false);

                } while (en.MoveNext());
            }
        }
    }
}
