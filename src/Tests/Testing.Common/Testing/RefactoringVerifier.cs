// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

namespace Roslynator.Testing;

/// <summary>
/// Represents verifier for a code refactoring.
/// </summary>
public abstract class RefactoringVerifier<TRefactoringProvider> : CodeVerifier
    where TRefactoringProvider : CodeRefactoringProvider, new()
{
    internal RefactoringVerifier(IAssert assert) : base(assert)
    {
    }

    /// <summary>
    /// Verifies that refactoring will be applied correctly using specified <typeparamref name="TRefactoringProvider"/>.
    /// </summary>
    /// <param name="source">Source code where text selection is marked with <c>[|</c> and <c>|]</c> tokens.</param>
    public async Task VerifyRefactoringAsync(
        string source,
        string expectedSource,
        IEnumerable<string>? additionalFiles = null,
        string? equivalenceKey = null,
        TestOptions? options = null,
        IEnumerable<DiagnosticDescriptor>? additionalDiagnostics = null,
        CancellationToken cancellationToken = default)
    {
        var code = TestCode.Parse(source);

        var expected = ExpectedTestState.Parse(expectedSource);

        var data = new RefactoringTestData(
            code.Value,
            code.Spans.OrderByDescending(f => f.Start).ToImmutableArray(),
            AdditionalFile.CreateRange(additionalFiles),
            equivalenceKey: equivalenceKey,
            additionalDiagnostics: additionalDiagnostics);

        await VerifyRefactoringAsync(
            data,
            expected,
            options,
            cancellationToken: cancellationToken);
    }

    internal async Task VerifyRefactoringAsync(
        string source,
        string sourceData,
        string expectedData,
        IEnumerable<string>? additionalFiles = null,
        string? equivalenceKey = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var code = TestCode.Parse(source, sourceData, expectedData);

        var expected = ExpectedTestState.Parse(code.ExpectedValue!);

        var data = new RefactoringTestData(
            code.Value,
            code.Spans.OrderByDescending(f => f.Start).ToImmutableArray(),
            AdditionalFile.CreateRange(additionalFiles),
            equivalenceKey: equivalenceKey);

        await VerifyRefactoringAsync(
            data,
            expected,
            options,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Verifies that refactoring will be applied correctly using specified <typeparamref name="TRefactoringProvider"/>.
    /// </summary>
    public async Task VerifyRefactoringAsync(
        RefactoringTestData data,
        ExpectedTestState expected,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        if (expected is null)
            throw new ArgumentNullException(nameof(expected));

        if (data.Spans.IsEmpty)
            Fail("Span on which a refactoring should be invoked was not found.");

        options ??= Options;

        TRefactoringProvider refactoringProvider = Activator.CreateInstance<TRefactoringProvider>();

        foreach (TextSpan span in data.Spans)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (Workspace workspace = new AdhocWorkspace())
            {
                (Document document, ImmutableArray<ExpectedDocument> expectedDocuments) = CreateDocument(workspace.CurrentSolution, data.Source, data.AdditionalFiles, options, data.AdditionalDiagnostics);

                SemanticModel semanticModel = (await document.GetSemanticModelAsync(cancellationToken))!;

                ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

                VerifyCompilerDiagnostics(compilerDiagnostics, options);

                CodeAction? action = null;
                List<CodeAction>? candidateActions = null;

                var context = new CodeRefactoringContext(
                    document,
                    span,
                    a =>
                    {
                        ImmutableArray<CodeAction> nestedActions = a.GetNestedActions();

                        if (nestedActions.Any())
                        {
                            foreach (CodeAction nestedAction in nestedActions)
                            {
                                if (data.EquivalenceKey is null
                                    || string.Equals(nestedAction.EquivalenceKey, data.EquivalenceKey, StringComparison.Ordinal))
                                {
                                    if (action is not null)
                                        Fail($"Multiple refactorings registered by '{refactoringProvider.GetType().Name}'.", new CodeAction[] { action, a });

                                    action = nestedAction;
                                }
                                else
                                {
                                    (candidateActions ??= new List<CodeAction>()).Add(nestedAction);
                                }
                            }
                        }
                        else if (data.EquivalenceKey is null
                            || string.Equals(a.EquivalenceKey, data.EquivalenceKey, StringComparison.Ordinal))
                        {
                            if (action is not null)
                                Fail($"Multiple refactorings registered by '{refactoringProvider.GetType().Name}'.", new CodeAction[] { action, a });

                            action = a;
                        }
                        else
                        {
                            (candidateActions ??= new List<CodeAction>()).Add(a);
                        }
                    },
                    cancellationToken);

                await refactoringProvider.ComputeRefactoringsAsync(context);

                if (action is null)
                    Fail("No code refactoring has been registered.", candidateActions);

                document = await VerifyAndApplyCodeActionAsync(document, action!, expected.CodeActionTitle);
                semanticModel = (await document.GetSemanticModelAsync(cancellationToken))!;

                ImmutableArray<Diagnostic> newCompilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

                VerifyCompilerDiagnostics(newCompilerDiagnostics, options);
                VerifyNoNewCompilerDiagnostics(compilerDiagnostics, newCompilerDiagnostics, options);

                await VerifyExpectedDocument(expected, document, cancellationToken);

                if (expectedDocuments.Any())
                    await VerifyAdditionalDocumentsAsync(document.Project, expectedDocuments, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Verifies that refactoring will not be applied using specified <typeparamref name="TRefactoringProvider"/>.
    /// </summary>
    /// <param name="source">Source code where text selection is marked with <c>[|</c> and <c>|]</c> tokens.</param>
    public async Task VerifyNoRefactoringAsync(
        string source,
        string? equivalenceKey = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var code = TestCode.Parse(source);

        var data = new RefactoringTestData(
            code.Value,
            code.Spans,
            equivalenceKey: equivalenceKey);

        await VerifyNoRefactoringAsync(
            data,
            options,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Verifies that refactoring will not be applied using specified <typeparamref name="TRefactoringProvider"/>.
    /// </summary>
    public async Task VerifyNoRefactoringAsync(
        RefactoringTestData data,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        if (data.Spans.IsEmpty)
            Fail("Span on which a refactoring should be invoked was not found.");

        cancellationToken.ThrowIfCancellationRequested();

        options ??= Options;

        TRefactoringProvider refactoringProvider = Activator.CreateInstance<TRefactoringProvider>();

        using (Workspace workspace = new AdhocWorkspace())
        {
            (Document document, ImmutableArray<ExpectedDocument> _) = CreateDocument(workspace.CurrentSolution, data.Source, data.AdditionalFiles, options, data.AdditionalDiagnostics);

            SemanticModel semanticModel = (await document.GetSemanticModelAsync(cancellationToken))!;

            ImmutableArray<Diagnostic> compilerDiagnostics = semanticModel.GetDiagnostics(cancellationToken: cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics, options);

            foreach (TextSpan span in data.Spans)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var context = new CodeRefactoringContext(
                    document,
                    span,
                    a =>
                    {
                        ImmutableArray<CodeAction> nestedActions = a.GetNestedActions();

                        if (nestedActions.Any())
                        {
                            foreach (CodeAction nestedAction in nestedActions)
                            {
                                if (data.EquivalenceKey is null
                                    || string.Equals(nestedAction.EquivalenceKey, data.EquivalenceKey, StringComparison.Ordinal))
                                {
                                    Fail("No code refactoring expected.");
                                }
                            }
                        }
                        else if (data.EquivalenceKey is null
                            || string.Equals(a.EquivalenceKey, data.EquivalenceKey, StringComparison.Ordinal))
                        {
                            Fail("No code refactoring expected.");
                        }
                    },
                    cancellationToken);

                await refactoringProvider.ComputeRefactoringsAsync(context);
            }
        }
    }
}
