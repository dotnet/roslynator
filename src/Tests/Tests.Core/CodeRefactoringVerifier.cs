// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Roslynator.Tests
{
    public abstract class CodeRefactoringVerifier : CodeVerifier
    {
        public abstract string RefactoringId { get; }

        public abstract CodeRefactoringProvider RefactoringProvider { get; }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            string equivalenceKey,
            string[] additionalSources = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TestSourceTextAnalysis analysis = TestSourceText.GetSpans(source, reverse: true);

            await VerifyRefactoringAsync(
                source: analysis.Source,
                expected: expected,
                spans: analysis.Spans.Select(f => f.Span),
                equivalenceKey: equivalenceKey,
                additionalSources: additionalSources,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string theory,
            string beforeData,
            string afterData,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            (string source, string expected, TextSpan span) = TestSourceText.ReplaceSpan(theory, beforeData, afterData);

            TestSourceTextAnalysis analysis = TestSourceText.GetSpans(source, reverse: true);

            if (analysis.Spans.Any())
            {
                await VerifyRefactoringAsync(
                    source: analysis.Source,
                    expected: expected,
                    spans: analysis.Spans.Select(f => f.Span),
                    equivalenceKey: equivalenceKey,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await VerifyRefactoringAsync(
                    source: source,
                    expected: expected,
                    span: span,
                    equivalenceKey: equivalenceKey,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            TextSpan span,
            string equivalenceKey,
            string[] additionalSources = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyRefactoringAsync(
                source: source,
                expected: expected,
                spans: ImmutableArray.Create(span),
                equivalenceKey: equivalenceKey,
                additionalSources: additionalSources,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            IEnumerable<TextSpan> spans,
            string equivalenceKey,
            string[] additionalSources = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Document document = WorkspaceFactory.Document(Language, source, additionalSources ?? Array.Empty<string>());

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics);

            foreach (TextSpan span in spans)
            {
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

                VerifyCompilerDiagnostics(newCompilerDiagnostics);

                if (!Options.AllowNewCompilerDiagnostics)
                    VerifyNoNewCompilerDiagnostics(compilerDiagnostics, newCompilerDiagnostics);
            }

            string actual = await document.ToFullStringAsync(simplify: true, format: true).ConfigureAwait(false);

            Assert.Equal(expected, actual);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TestSourceTextAnalysis analysis = TestSourceText.GetSpans(source, reverse: true);

            await VerifyNoRefactoringAsync(
                source: analysis.Source,
                spans: analysis.Spans.Select(f => f.Span),
                equivalenceKey: equivalenceKey,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            TextSpan span,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await VerifyNoRefactoringAsync(
                source,
                ImmutableArray.Create(span),
                equivalenceKey,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            IEnumerable<TextSpan> spans,
            string equivalenceKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Document document = WorkspaceFactory.Document(Language, source);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics);

            foreach (TextSpan span in spans)
            {
                var context = new CodeRefactoringContext(
                    document,
                    span,
                    a =>
                    {
                        if (equivalenceKey == null
                            || string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                        {
                            Assert.True(false, "Expected no code refactoring.");
                        }
                    },
                    CancellationToken.None);

                await RefactoringProvider.ComputeRefactoringsAsync(context).ConfigureAwait(false);
            }
        }
    }
}
