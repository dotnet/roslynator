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
using Roslynator.Testing.Text;

namespace Roslynator.Testing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class RefactoringVerifier : CodeVerifier
    {
        internal RefactoringVerifier(WorkspaceFactory workspaceFactory, IAssert assert) : base(workspaceFactory, assert)
        {
        }

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
            IEnumerable<string> additionalSources = null,
            string title = null,
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
                title: title,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken);
        }

        public async Task VerifyRefactoringAsync(
            string source,
            string sourceData,
            string expectedData,
            string title = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            (TextSpan span, string source2, string expected) = TextParser.ReplaceEmptySpan(source, sourceData, expectedData);

            TextParserResult result = TextParser.GetSpans(source2, LinePositionSpanInfoComparer.IndexDescending);

            if (result.Spans.Any())
            {
                await VerifyRefactoringAsync(
                    source: result.Text,
                    expected: expected,
                    spans: result.Spans.Select(f => f.Span),
                    title: title,
                    equivalenceKey: equivalenceKey,
                    options: options,
                    cancellationToken: cancellationToken);
            }
            else
            {
                await VerifyRefactoringAsync(
                    source: source2,
                    expected: expected,
                    span: span,
                    title: title,
                    equivalenceKey: equivalenceKey,
                    options: options,
                    cancellationToken: cancellationToken);
            }
        }

        internal async Task VerifyRefactoringAsync(
            string source,
            string expected,
            IEnumerable<TextSpan> spans,
            IEnumerable<string> additionalSources = null,
            string title = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            using (IEnumerator<TextSpan> en = spans.GetEnumerator())
            {
                if (!en.MoveNext())
                    Assert.True(false, "Span on which a refactoring should be invoked was not found.");

                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await VerifyRefactoringAsync(
                        source: source,
                        expected: expected,
                        span: en.Current,
                        additionalSources: additionalSources,
                        title: title,
                        equivalenceKey: equivalenceKey,
                        options: options,
                        cancellationToken: cancellationToken);

                } while (en.MoveNext());
            }
        }

        internal async Task VerifyRefactoringAsync(
            string source,
            string expected,
            TextSpan span,
            IEnumerable<string> additionalSources = null,
            string title = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            options ??= Options;

            using (Workspace workspace = new AdhocWorkspace())
            {
                Document document = WorkspaceFactory.CreateDocument(workspace.CurrentSolution, source, additionalSources, options);

                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

                ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

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

                await RefactoringProvider.ComputeRefactoringsAsync(context);

                Assert.True(action != null, "No code refactoring has been registered.");

                document = await VerifyAndApplyCodeActionAsync(document, action, title);

                semanticModel = await document.GetSemanticModelAsync(cancellationToken);

                ImmutableArray<Diagnostic> newCompilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

                VerifyCompilerDiagnostics(newCompilerDiagnostics, options);

                VerifyNoNewCompilerDiagnostics(compilerDiagnostics, newCompilerDiagnostics, options);

                string actual = await document.ToFullStringAsync(simplify: true, format: true, cancellationToken);

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
                cancellationToken: cancellationToken);
        }

        internal async Task VerifyNoRefactoringAsync(
            string source,
            TextSpan span,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            await VerifyNoRefactoringAsync(
                source,
                ImmutableArray.Create(span),
                equivalenceKey: equivalenceKey,
                options,
                cancellationToken);
        }

        internal async Task VerifyNoRefactoringAsync(
            string source,
            IEnumerable<TextSpan> spans,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            options ??= Options;

            using (Workspace workspace = new AdhocWorkspace())
            {
                Document document = WorkspaceFactory.CreateDocument(workspace.CurrentSolution, source, options);

                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

                ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

                VerifyCompilerDiagnostics(compilerDiagnostics, options);

                using (IEnumerator<TextSpan> en = spans.GetEnumerator())
                {
                    if (!en.MoveNext())
                        Assert.True(false, "Span on which a refactoring should be invoked was not found.");

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

                        await RefactoringProvider.ComputeRefactoringsAsync(context);

                    } while (en.MoveNext());
                }
            }
        }
    }
}
