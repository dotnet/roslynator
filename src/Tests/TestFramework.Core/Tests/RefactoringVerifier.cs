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
using static Roslynator.Tests.CodeVerifierHelpers;

namespace Roslynator.Tests
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class RefactoringVerifier : CodeVerifier
    {
        public abstract string RefactoringId { get; }

        public abstract CodeRefactoringProvider RefactoringProvider { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return (!string.IsNullOrEmpty(RefactoringId))
                    ? $"{RefactoringId} {RefactoringProvider.GetType().Name}"
                    : $"{RefactoringProvider.GetType().Name}";
            }
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            string[] additionalSources = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            TextParserResult result = TextParser.GetSpans(source, LinePositionSpanInfoComparer.IndexDescending);

            await VerifyRefactoringAsync(
                source: result.Text,
                expected: expected,
                spans: result.Spans.Select(f => f.Span),
                additionalSources: additionalSources,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyRefactoringAsync(
            string theory,
            string fromData,
            string toData,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            (TextSpan span, string source, string expected) = TextParser.ReplaceEmptySpan(theory, fromData, toData);

            TextParserResult result = TextParser.GetSpans(source, LinePositionSpanInfoComparer.IndexDescending);

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
            string[] additionalSources = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
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
                        span: en.Current,
                        additionalSources: additionalSources,
                        equivalenceKey: equivalenceKey,
                        options: options,
                        cancellationToken: cancellationToken).ConfigureAwait(false);

                } while (en.MoveNext());
            }
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string expected,
            TextSpan span,
            string[] additionalSources = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (Workspace workspace = new AdhocWorkspace())
            {
                Project project = WorkspaceFactory.AddProject(workspace.CurrentSolution, options);

                Document document = WorkspaceFactory.AddDocument(project, source, additionalSources ?? Array.Empty<string>());

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
        }

        public async Task VerifyNoRefactoringAsync(
            string source,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            TextParserResult result = TextParser.GetSpans(source, LinePositionSpanInfoComparer.IndexDescending);

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
            CancellationToken cancellationToken = default)
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
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (Workspace workspace = new AdhocWorkspace())
            {
                Project project = WorkspaceFactory.AddProject(workspace.CurrentSolution, options);

                Document document = WorkspaceFactory.AddDocument(project, source);

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
}
