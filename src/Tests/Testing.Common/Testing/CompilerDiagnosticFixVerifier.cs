// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.Testing
{
    /// <summary>
    /// Represents a verifier for compiler diagnostics (i.e CS1234 for C#).
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class CompilerDiagnosticFixVerifier : CodeVerifier
    {
        private ImmutableArray<string> _fixableDiagnosticIds;

        internal CompilerDiagnosticFixVerifier(WorkspaceFactory workspaceFactory, IAssert assert) : base(workspaceFactory, assert)
        {
        }

        /// <summary>
        /// Gets an ID of a diagnostic to verify.
        /// </summary>
        public abstract string DiagnosticId { get; }

        /// <summary>
        /// Gets an <see cref="CodeFixProvider"/> that should fix a diagnostic.
        /// </summary>
        public abstract CodeFixProvider FixProvider { get; }

        internal ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                if (_fixableDiagnosticIds.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _fixableDiagnosticIds, FixProvider.FixableDiagnosticIds);

                return _fixableDiagnosticIds;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{DiagnosticId} {FixProvider.GetType().Name}"; }
        }

        /// <summary>
        /// Verifies that specified source will produce compiler diagnostic with ID specified in <see cref="DiagnosticId"/>.
        /// </summary>
        /// <param name="source">Source text that contains placeholder [||] to be replaced with <paramref name="sourceData"/> and <paramref name="expectedData"/>.</param>
        /// <param name="sourceData"></param>
        /// <param name="expectedData"></param>
        /// <param name="title">Code action's title.</param>
        /// <param name="equivalenceKey">Code action's equivalence key.</param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        public async Task VerifyFixAsync(
            string source,
            string sourceData,
            string expectedData,
            string title = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            (_, string source2, string expected) = TextParser.ReplaceEmptySpan(source, sourceData, expectedData);

            await VerifyFixAsync(
                source: source2,
                expected: expected,
                title: title,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Verifies that specified source will produce compiler diagnostic with ID specified in <see cref="DiagnosticId"/>.
        /// </summary>
        /// <param name="source">A source code that should be tested. Tokens [| and |] represents start and end of selection respectively.</param>
        /// <param name="expected"></param>
        /// <param name="additionalData"></param>
        /// <param name="title">Code action's title.</param>
        /// <param name="equivalenceKey">Code action's equivalence key.</param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        public async Task VerifyFixAsync(
            string source,
            string expected,
            IEnumerable<(string source, string expected)> additionalData = null,
            string title = null,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            options ??= Options;

            if (!FixableDiagnosticIds.Contains(DiagnosticId))
                Assert.True(false, $"Code fix provider '{FixProvider.GetType().Name}' cannot fix diagnostic '{DiagnosticId}'.");

            using (Workspace workspace = new AdhocWorkspace())
            {
                Document document = WorkspaceFactory.CreateDocument(workspace.CurrentSolution, source, options);

                Project project = document.Project;

                ImmutableArray<ExpectedDocument> expectedDocuments = (additionalData != null)
                    ? WorkspaceFactory.AddAdditionalDocuments(additionalData, ref project)
                    : ImmutableArray<ExpectedDocument>.Empty;

                document = project.GetDocument(document.Id);

                ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;

                var fixRegistered = false;

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken);

                    ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(cancellationToken: cancellationToken);

                    int length = diagnostics.Length;

                    if (length == 0)
                        break;

                    if (previousDiagnostics.Any())
                        VerifyNoNewCompilerDiagnostics(previousDiagnostics, diagnostics, options);

                    if (length == previousDiagnostics.Length
                        && !diagnostics.Except(previousDiagnostics, DiagnosticDeepEqualityComparer.Instance).Any())
                    {
                        Assert.True(false, "Same diagnostics returned before and after the fix was applied.");
                    }

                    Diagnostic diagnostic = FindDiagnostic(diagnostics);

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

                    await FixProvider.RegisterCodeFixesAsync(context);

                    if (action == null)
                        break;

                    fixRegistered = true;

                    document = await VerifyAndApplyCodeActionAsync(document, action, title);

                    previousDiagnostics = diagnostics;
                }

                Assert.True(fixRegistered, "No code fix has been registered.");

                string actual = await document.ToFullStringAsync(simplify: true, format: true, cancellationToken);

                Assert.Equal(expected, actual);

                if (expectedDocuments.Any())
                    await VerifyAdditionalDocumentsAsync(document.Project, expectedDocuments, cancellationToken);
            }

            Diagnostic FindDiagnostic(ImmutableArray<Diagnostic> diagnostics)
            {
                Diagnostic match = null;

                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (string.Equals(diagnostic.Id, DiagnosticId, StringComparison.Ordinal))
                    {
                        if (match == null
                            || diagnostic.Location.SourceSpan.Start > match.Location.SourceSpan.Start)
                        {
                            match = diagnostic;
                        }
                    }
                }

                return match;
            }
        }

        /// <summary>
        /// Verifies that specified source will not produce compiler diagnostic with ID specified in <see cref="DiagnosticId"/>.
        /// </summary>
        /// <param name="source">A source code that should be tested. Tokens [| and |] represents start and end of selection respectively.</param>
        /// <param name="equivalenceKey">Code action's equivalence key.</param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        public async Task VerifyNoFixAsync(
            string source,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            options ??= Options;

            using (Workspace workspace = new AdhocWorkspace())
            {
                Document document = WorkspaceFactory.CreateDocument(workspace.CurrentSolution, source, options);

                Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken);

                foreach (Diagnostic diagnostic in compilation.GetDiagnostics(cancellationToken: cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!FixableDiagnosticIds.Contains(diagnostic.Id))
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

                    await FixProvider.RegisterCodeFixesAsync(context);
                }
            }
        }
    }
}
