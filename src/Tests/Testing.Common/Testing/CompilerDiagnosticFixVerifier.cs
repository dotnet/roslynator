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
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class CompilerDiagnosticFixVerifier : CodeVerifier
    {
        private ImmutableArray<string> _fixableDiagnosticIds;

        internal CompilerDiagnosticFixVerifier(WorkspaceFactory workspaceFactory) : base(workspaceFactory)
        {
        }

        public abstract string DiagnosticId { get; }

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

        public async Task VerifyFixAsync(
            string source,
            string inlineSource,
            string inlineExpected,
            string equivalenceKey = null,
            CodeVerificationOptions options = null,
            CancellationToken cancellationToken = default)
        {
            (_, string source2, string expected) = TextParser.ReplaceEmptySpan(source, inlineSource, inlineExpected);

            await VerifyFixAsync(
                source: source2,
                expected: expected,
                equivalenceKey: equivalenceKey,
                options: options,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task VerifyFixAsync(
            string source,
            string expected,
            IEnumerable<(string source, string expected)> additionalData = null,
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
                Project project = WorkspaceFactory.AddProject(workspace.CurrentSolution, options);

                Document document = WorkspaceFactory.AddDocument(project, source);

                project = document.Project;

                ImmutableArray<ExpectedDocument> expectedDocuments = (additionalData != null)
                    ? WorkspaceFactory.AddAdditionalDocuments(additionalData, ref project)
                    : ImmutableArray<ExpectedDocument>.Empty;

                document = project.GetDocument(document.Id);

                ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;

                bool fixRegistered = false;

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

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

                    await FixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                    if (action == null)
                        break;

                    fixRegistered = true;

                    document = await document.ApplyCodeActionAsync(action).ConfigureAwait(false);

                    previousDiagnostics = diagnostics;
                }

                Assert.True(fixRegistered, "No code fix has been registered.");

                string actual = await document.ToFullStringAsync(simplify: true, format: true, cancellationToken).ConfigureAwait(false);

                Assert.Equal(expected, actual);

                if (expectedDocuments.Any())
                    await VerifyAdditionalDocumentsAsync(document.Project, expectedDocuments, cancellationToken).ConfigureAwait(false);
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

        [SuppressMessage("Redundancy", "RCS1163:Unused parameter.", Justification = "<Pending>")]
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
                Project project = WorkspaceFactory.AddProject(workspace.CurrentSolution, options);

                Document document = WorkspaceFactory.AddDocument(project, source);

                Compilation compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

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

                    await FixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
                }
            }
        }
    }
}
