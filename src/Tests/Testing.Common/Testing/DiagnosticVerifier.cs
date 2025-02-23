﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Testing;

/// <summary>
/// Represents verifier for a diagnostic that is produced by <see cref="DiagnosticAnalyzer"/>.
/// </summary>
public abstract class DiagnosticVerifier<TAnalyzer, TFixProvider> : CodeVerifier
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TFixProvider : CodeFixProvider, new()
{
    internal DiagnosticVerifier(IAssert assert) : base(assert)
    {
    }

    public abstract DiagnosticDescriptor Descriptor { get; }

    /// <summary>
    /// Verifies that specified source will produce specified diagnostic(s).
    /// </summary>
    /// <param name="source">Source code where diagnostic's location is marked with <c>[|</c> and <c>|]</c> tokens.</param>
    public async Task VerifyDiagnosticAsync(
        string source,
        IEnumerable<string>? additionalFiles = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var code = TestCode.Parse(source);

        var data = new DiagnosticTestData(
            code.Value,
            code.Spans,
            code.AdditionalSpans,
            additionalFiles: AdditionalFile.CreateRange(additionalFiles));

        await VerifyDiagnosticAsync(
            data,
            options: options,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Verifies that specified source will produce specified diagnostic(s).
    /// </summary>
    /// <param name="file">Source file where diagnostic's location is marked with <c>[|</c> and <c>|]</c> tokens.</param>
    public async Task VerifyDiagnosticAsync(
        TestFile file,
        IEnumerable<AdditionalFile>? additionalFiles = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (file is null)
            throw new ArgumentNullException(nameof(file));

        var code = TestCode.Parse(file.Source);

        var data = new DiagnosticTestData(
            code.Value,
            code.Spans,
            code.AdditionalSpans,
            additionalFiles: additionalFiles,
            directoryPath: file.DirectoryPath,
            fileName: file.Name);

        await VerifyDiagnosticAsync(
            data,
            options: options,
            cancellationToken: cancellationToken);
    }

    internal async Task VerifyDiagnosticAsync(
        string source,
        string sourceData,
        IEnumerable<string>? additionalFiles = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var code = TestCode.Parse(source, sourceData);

        var data = new DiagnosticTestData(
            source,
            code.Spans,
            code.AdditionalSpans,
            additionalFiles: AdditionalFile.CreateRange(additionalFiles));

        await VerifyDiagnosticAsync(
            data,
            options: options,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Verifies that specified source will produce specified diagnostic(s).
    /// </summary>
    public async Task VerifyDiagnosticAsync(
        DiagnosticTestData data,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        cancellationToken.ThrowIfCancellationRequested();

        options ??= Options;

        TAnalyzer analyzer = Activator.CreateInstance<TAnalyzer>();
        ImmutableArray<DiagnosticDescriptor> supportedDiagnostics = analyzer.SupportedDiagnostics;

        using (Workspace workspace = new AdhocWorkspace())
        {
            (Document document, ImmutableArray<ExpectedDocument> expectedDocuments) = CreateDocument(workspace.CurrentSolution, data.Source, data.DirectoryPath, data.FileName, data.AdditionalFiles, options, Descriptor);

            SyntaxTree tree = (await document.GetSyntaxTreeAsync(cancellationToken))!;

            ImmutableArray<Diagnostic> expectedDiagnostics = data.GetDiagnostics(Descriptor, tree);

            VerifySupportedDiagnostics(analyzer, expectedDiagnostics);

            Compilation compilation = (await document.Project.GetCompilationAsync(cancellationToken))!;

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics, options);

            ImmutableArray<Diagnostic> diagnostics = await GetAnalyzerDiagnosticsAsync(compilation, analyzer, document.Project.AnalyzerOptions, DiagnosticComparer.SpanStart, cancellationToken);

            if (diagnostics.Length > 0
                && supportedDiagnostics.Length > 1)
            {
                VerifyDiagnostics(data, analyzer, expectedDiagnostics, FilterDiagnostics(diagnostics, expectedDiagnostics), cancellationToken);
            }
            else
            {
                VerifyDiagnostics(data, analyzer, expectedDiagnostics, diagnostics, cancellationToken);
            }

            if (expectedDocuments.Any())
                await VerifyAdditionalDocumentsAsync(document.Project, expectedDocuments, cancellationToken);
        }

        static IEnumerable<Diagnostic> FilterDiagnostics(
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<Diagnostic> expectedDiagnostics)
        {
            foreach (Diagnostic diagnostic in diagnostics)
            {
                var success = false;
                foreach (Diagnostic expectedDiagnostic in expectedDiagnostics)
                {
                    if (DiagnosticComparer.Id.Equals(diagnostic, expectedDiagnostic))
                    {
                        success = true;
                        break;
                    }
                }

                if (success)
                    yield return diagnostic;
            }
        }
    }

    internal async Task VerifyNoDiagnosticAsync(
        string source,
        string sourceData,
        IEnumerable<string>? additionalFiles = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var code = TestCode.Parse(source, sourceData);

        var data = new DiagnosticTestData(
            code.Value,
            spans: null,
            code.AdditionalSpans,
            AdditionalFile.CreateRange(additionalFiles));

        await VerifyNoDiagnosticAsync(
            data,
            options: options,
            cancellationToken);
    }

    /// <summary>
    /// Verifies that specified source will not produce specified diagnostic.
    /// </summary>
    public async Task VerifyNoDiagnosticAsync(
        string source,
        IEnumerable<string>? additionalFiles = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var data = new DiagnosticTestData(
            source,
            spans: null,
            additionalFiles: AdditionalFile.CreateRange(additionalFiles));

        await VerifyNoDiagnosticAsync(
            data,
            options: options,
            cancellationToken);
    }

    /// <summary>
    /// Verifies that specified source will not produce specified diagnostic.
    /// </summary>
    public async Task VerifyNoDiagnosticAsync(
        TestFile file,
        IEnumerable<AdditionalFile>? additionalFiles = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (file is null)
            throw new ArgumentNullException(nameof(file));

        var code = TestCode.Parse(file.Source);

        var data = new DiagnosticTestData(
            code.Value,
            code.Spans,
            code.AdditionalSpans,
            additionalFiles: additionalFiles,
            directoryPath: file.DirectoryPath,
            fileName: file.Name);

        await VerifyNoDiagnosticAsync(
            data,
            options: options,
            cancellationToken);
    }

    /// <summary>
    /// Verifies that specified source will not produce specified diagnostic.
    /// </summary>
    public async Task VerifyNoDiagnosticAsync(
        DiagnosticTestData data,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        cancellationToken.ThrowIfCancellationRequested();

        options ??= Options;

        TAnalyzer analyzer = Activator.CreateInstance<TAnalyzer>();

        using (Workspace workspace = new AdhocWorkspace())
        {
            (Document document, ImmutableArray<ExpectedDocument> _) = CreateDocument(workspace.CurrentSolution, data.Source, data.DirectoryPath, data.FileName, data.AdditionalFiles, options, Descriptor);

            SyntaxTree tree = (await document.GetSyntaxTreeAsync(cancellationToken))!;

            ImmutableArray<Diagnostic> expectedDiagnostics = data.GetDiagnostics(Descriptor, tree);

            VerifySupportedDiagnostics(analyzer, expectedDiagnostics);

            Compilation compilation = (await document.Project.GetCompilationAsync(cancellationToken))!;

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics, options);

            ImmutableArray<Diagnostic> actualDiagnostics = await GetAnalyzerDiagnosticsAsync(compilation, analyzer, document.Project.AnalyzerOptions, DiagnosticComparer.SpanStart, cancellationToken);

            actualDiagnostics = actualDiagnostics
                .Where(diagnostic => string.Equals(diagnostic.Id, Descriptor.Id))
                .ToImmutableArray();

            if (!actualDiagnostics.IsEmpty)
                Fail("No diagnostic expected.", actualDiagnostics);
        }
    }

    /// <summary>
    /// Verifies that specified source will produce specified diagnostic and that the diagnostic will be fixed.
    /// </summary>
    /// <param name="source">Source code where diagnostic's location is marked with <c>[|</c> and <c>|]</c> tokens.</param>
    public async Task VerifyDiagnosticAndFixAsync(
        string source,
        string expectedSource,
        IEnumerable<(string source, string expectedSource)>? additionalFiles = null,
        string? equivalenceKey = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var code = TestCode.Parse(source);

        var expected = ExpectedTestState.Parse(expectedSource);

        var data = new DiagnosticTestData(
            code.Value,
            code.Spans,
            additionalSpans: code.AdditionalSpans,
            additionalFiles: AdditionalFile.CreateRange(additionalFiles),
            equivalenceKey: equivalenceKey);

        await VerifyDiagnosticAndFixAsync(data, expected, options, cancellationToken);
    }

    /// <summary>
    /// Verifies that specified source will produce specified diagnostic and that the diagnostic will be fixed.
    /// </summary>
    /// <param name="file">Source file where diagnostic's location is marked with <c>[|</c> and <c>|]</c> tokens.</param>
    public async Task VerifyDiagnosticAndFixAsync(
        TestFile file,
        IEnumerable<AdditionalFile>? additionalFiles = null,
        string? equivalenceKey = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (file is null)
            throw new ArgumentNullException(nameof(file));

        if (file.ExpectedSource is null)
            throw new ArgumentException("Expected source is required.", nameof(file));

        var code = TestCode.Parse(file.Source);
        var expected = ExpectedTestState.Parse(file.ExpectedSource);

        var data = new DiagnosticTestData(
            code.Value,
            code.Spans,
            additionalSpans: code.AdditionalSpans,
            additionalFiles: additionalFiles,
            equivalenceKey: equivalenceKey);

        await VerifyDiagnosticAndFixAsync(data, expected, options, cancellationToken);
    }

    internal async Task VerifyDiagnosticAndFixAsync(
        string source,
        string sourceData,
        string expectedData,
        IEnumerable<(string source, string expectedSource)>? additionalFiles = null,
        string? equivalenceKey = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var code = TestCode.Parse(source, sourceData, expectedData);

        var expected = ExpectedTestState.Parse(code.ExpectedValue!);

        var data = new DiagnosticTestData(
            code.Value,
            code.Spans,
            code.AdditionalSpans,
            AdditionalFile.CreateRange(additionalFiles),
            equivalenceKey: equivalenceKey);

        await VerifyDiagnosticAndFixAsync(data, expected, options, cancellationToken);
    }

    /// <summary>
    /// Verifies that specified source will produce specified diagnostic and that the diagnostic will be fixed.
    /// </summary>
    public async Task VerifyDiagnosticAndFixAsync(
        DiagnosticTestData data,
        ExpectedTestState expected,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await VerifyDiagnosticAsync(data, options, cancellationToken);
        await VerifyFixAsync(data, expected, options, cancellationToken);
    }

    /// <summary>
    /// Verifies that specified source will produce specified diagnostic and that the diagnostic will not be fixed.
    /// </summary>
    /// <param name="source">Source code where diagnostic's location is marked with <c>[|</c> and <c>|]</c> tokens.</param>
    public async Task VerifyDiagnosticAndNoFixAsync(
        string source,
        IEnumerable<string>? additionalFiles = null,
        string? equivalenceKey = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var code = TestCode.Parse(source);

        var data = new DiagnosticTestData(
            code.Value,
            code.Spans,
            additionalSpans: code.AdditionalSpans,
            additionalFiles: AdditionalFile.CreateRange(additionalFiles),
            equivalenceKey: equivalenceKey);

        await VerifyDiagnosticAndNoFixAsync(data, options, cancellationToken);
    }

    /// <summary>
    /// Verifies that specified source will produce specified diagnostic and that the diagnostic will not be fixed.
    /// </summary>
    /// <param name="file">Source file where diagnostic's location is marked with <c>[|</c> and <c>|]</c> tokens.</param>
    public async Task VerifyDiagnosticAndNoFixAsync(
        TestFile file,
        IEnumerable<AdditionalFile>? additionalFiles = null,
        string? equivalenceKey = null,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (file is null)
            throw new ArgumentNullException(nameof(file));

        var code = TestCode.Parse(file.Source);

        var data = new DiagnosticTestData(
            code.Value,
            code.Spans,
            additionalSpans: code.AdditionalSpans,
            additionalFiles: additionalFiles,
            equivalenceKey: equivalenceKey);

        await VerifyDiagnosticAndNoFixAsync(data, options, cancellationToken);
    }

    /// <summary>
    /// Verifies that specified source will produce specified diagnostic and that the diagnostic will not be fixed.
    /// </summary>
    public async Task VerifyDiagnosticAndNoFixAsync(
        DiagnosticTestData data,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await VerifyDiagnosticAsync(data, options, cancellationToken);
        await VerifyNoFixAsync(data, options, cancellationToken);
    }

    private async Task VerifyFixAsync(
        DiagnosticTestData data,
        ExpectedTestState expected,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        if (expected is null)
            throw new ArgumentNullException(nameof(expected));

        cancellationToken.ThrowIfCancellationRequested();

        options ??= Options;

        TAnalyzer analyzer = Activator.CreateInstance<TAnalyzer>();
        TFixProvider fixProvider = Activator.CreateInstance<TFixProvider>();

        ImmutableArray<DiagnosticDescriptor> supportedDiagnostics = analyzer.SupportedDiagnostics;

        using (Workspace workspace = new AdhocWorkspace())
        {
            (Document document, ImmutableArray<ExpectedDocument> expectedDocuments) = CreateDocument(workspace.CurrentSolution, data.Source, data.DirectoryPath, data.FileName, data.AdditionalFiles, options, Descriptor);

            Project project = document.Project;

            SyntaxTree tree = (await document.GetSyntaxTreeAsync(cancellationToken))!;

            ImmutableArray<Diagnostic> expectedDiagnostics = data.GetDiagnostics(Descriptor, tree);

            foreach (Diagnostic diagnostic in expectedDiagnostics)
                VerifyFixableDiagnostics(fixProvider, diagnostic.Id);

            VerifySupportedDiagnostics(analyzer, expectedDiagnostics);

            Compilation compilation = (await project.GetCompilationAsync(cancellationToken))!;

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics, options);

            ImmutableArray<Diagnostic> previousDiagnostics = ImmutableArray<Diagnostic>.Empty;

            ImmutableArray<Diagnostic> previousPreviousDiagnostics = ImmutableArray<Diagnostic>.Empty;

            var fixRegistered = false;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ImmutableArray<Diagnostic> diagnostics = await GetAnalyzerDiagnosticsAsync(compilation, analyzer, document.Project.AnalyzerOptions, DiagnosticComparer.SpanStart, cancellationToken);

                int length = diagnostics.Length;

                if (length == 0)
                {
                    if (!fixRegistered)
                        Fail("No diagnostic found.");

                    break;
                }

                if (DiagnosticDeepEqualityComparer.Equals(diagnostics, previousDiagnostics))
                    Fail("Same diagnostics returned before and after the fix was applied.", diagnostics);

                if (DiagnosticDeepEqualityComparer.Equals(diagnostics, previousPreviousDiagnostics))
                    Fail("Infinite loop detected: Reported diagnostics have been previously fixed.", diagnostics);

                Diagnostic? diagnostic = FindDiagnosticToFix(diagnostics, expectedDiagnostics);

                static Diagnostic? FindDiagnosticToFix(
                    ImmutableArray<Diagnostic> diagnostics,
                    ImmutableArray<Diagnostic> expectedDiagnostics)
                {
                    foreach (Diagnostic diagnostic in diagnostics)
                    {
                        foreach (Diagnostic diagnostic2 in expectedDiagnostics)
                        {
                            if (diagnostic.Id == diagnostic2.Id)
                                return diagnostic;
                        }
                    }

                    return null;
                }

                if (diagnostic is null)
                {
                    if (!fixRegistered)
                        Fail($"No diagnostic with ID '{Descriptor.Id}' found.", diagnostics);

                    break;
                }

                CodeAction? action = null;
                List<CodeAction>? candidateActions = null;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, d) =>
                    {
                        ImmutableArray<CodeAction> nestedActions = a.GetNestedActions();

                        if (nestedActions.Any())
                        {
                            foreach (CodeAction nestedAction in nestedActions)
                            {
                                if ((data.EquivalenceKey is null
                                    || string.Equals(data.EquivalenceKey, nestedAction.EquivalenceKey, StringComparison.Ordinal))
                                    && d.Contains(diagnostic))
                                {
                                    if (action is not null)
                                        Fail($"Multiple fixes registered by '{fixProvider.GetType().Name}'.", new CodeAction[] { action, nestedAction });

                                    action = nestedAction;
                                }
                                else
                                {
                                    (candidateActions ??= new List<CodeAction>()).Add(nestedAction);
                                }
                            }
                        }
                        else if ((data.EquivalenceKey is null
                            || string.Equals(data.EquivalenceKey, a.EquivalenceKey, StringComparison.Ordinal))
                            && d.Contains(diagnostic))
                        {
                            if (action is not null)
                                Fail($"Multiple fixes registered by '{fixProvider.GetType().Name}'.", new CodeAction[] { action, a });

                            action = a;
                        }
                        else
                        {
                            (candidateActions ??= new List<CodeAction>()).Add(a);
                        }
                    },
                    cancellationToken);

                await fixProvider.RegisterCodeFixesAsync(context);

                if (action is null)
                    Fail("No code fix has been registered.", candidateActions);

                fixRegistered = true;

                document = await VerifyAndApplyCodeActionAsync(document, action!, expected.CodeActionTitle);
                compilation = (await document.Project.GetCompilationAsync(cancellationToken))!;

                ImmutableArray<Diagnostic> newCompilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

                VerifyCompilerDiagnostics(newCompilerDiagnostics, options);
                VerifyNoNewCompilerDiagnostics(compilerDiagnostics, newCompilerDiagnostics, options);

                previousPreviousDiagnostics = previousDiagnostics;
                previousDiagnostics = diagnostics;
            }

            await VerifyExpectedDocument(expected, document, cancellationToken);

            if (expectedDocuments.Any())
                await VerifyAdditionalDocumentsAsync(document.Project, expectedDocuments, cancellationToken);
        }
    }

    private async Task VerifyNoFixAsync(
        DiagnosticTestData data,
        TestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        cancellationToken.ThrowIfCancellationRequested();

        options ??= Options;

        TAnalyzer analyzer = Activator.CreateInstance<TAnalyzer>();
        TFixProvider fixProvider = Activator.CreateInstance<TFixProvider>();

        ImmutableArray<DiagnosticDescriptor> supportedDiagnostics = analyzer.SupportedDiagnostics;
        ImmutableArray<string> fixableDiagnosticIds = fixProvider.FixableDiagnosticIds;

        using (Workspace workspace = new AdhocWorkspace())
        {
            (Document document, ImmutableArray<ExpectedDocument> _) = CreateDocument(workspace.CurrentSolution, data.Source, data.DirectoryPath, data.FileName, data.AdditionalFiles, options, Descriptor);

            Compilation compilation = (await document.Project.GetCompilationAsync(cancellationToken))!;

            SyntaxTree tree = (await document.GetSyntaxTreeAsync(cancellationToken))!;

            ImmutableArray<Diagnostic> expectedDiagnostics = data.GetDiagnostics(Descriptor, tree);

            VerifySupportedDiagnostics(analyzer, expectedDiagnostics);

            ImmutableArray<Diagnostic> compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);

            VerifyCompilerDiagnostics(compilerDiagnostics, options);

            ImmutableArray<Diagnostic> diagnostics = await GetAnalyzerDiagnosticsAsync(compilation, analyzer, document.Project.AnalyzerOptions, DiagnosticComparer.SpanStart, cancellationToken);

            foreach (Diagnostic diagnostic in diagnostics)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (expectedDiagnostics.IndexOf(diagnostic, DiagnosticComparer.Id) >= 0
                    && fixableDiagnosticIds.Contains(diagnostic.Id))
                {
                    var context = new CodeFixContext(
                        document,
                        diagnostic,
                        (a, d) =>
                        {
                            ImmutableArray<CodeAction> nestedActions = a.GetNestedActions();

                            if (nestedActions.Any())
                            {
                                foreach (CodeAction nestedAction in nestedActions)
                                {
                                    if ((data.EquivalenceKey is null
                                        || string.Equals(nestedAction.EquivalenceKey, data.EquivalenceKey, StringComparison.Ordinal))
                                        && d.Contains(diagnostic))
                                    {
                                        Fail("No code fix expected.");
                                    }
                                }
                            }
                            else if ((data.EquivalenceKey is null
                                || string.Equals(a.EquivalenceKey, data.EquivalenceKey, StringComparison.Ordinal))
                                && d.Contains(diagnostic))
                            {
                                Fail("No code fix expected.");
                            }
                        },
                        cancellationToken);

                    await fixProvider.RegisterCodeFixesAsync(context);
                }
            }
        }
    }

    private void VerifyDiagnostics(
        DiagnosticTestData data,
        TAnalyzer analyzer,
        IEnumerable<Diagnostic> expectedDiagnostics,
        IEnumerable<Diagnostic> actualDiagnostics,
        CancellationToken cancellationToken = default)
    {
        int expectedCount = 0;
        int actualCount = 0;

        using (IEnumerator<Diagnostic> expectedEnumerator = expectedDiagnostics.OrderBy(f => f, DiagnosticComparer.SpanStart).GetEnumerator())
        using (IEnumerator<Diagnostic> actualEnumerator = actualDiagnostics.OrderBy(f => f, DiagnosticComparer.SpanStart).GetEnumerator())
        {
            if (!expectedEnumerator.MoveNext())
                Fail("Diagnostic's location not found in a source text.");

            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                expectedCount++;

                Diagnostic expectedDiagnostic = expectedEnumerator.Current;

                VerifySupportedDiagnostics(analyzer, expectedDiagnostic);

                if (actualEnumerator.MoveNext())
                {
                    actualCount++;

                    VerifyDiagnostic(
                        expectedDiagnostic,
                        actualEnumerator.Current,
                        data.DiagnosticMessage,
                        data.FormatProvider,
                        verifyAdditionalLocations: data.AlwaysVerifyAdditionalLocations || !data.AdditionalSpans.IsEmpty);
                }
                else
                {
                    while (expectedEnumerator.MoveNext())
                        expectedCount++;

                    ReportMismatch(actualDiagnostics, actualCount, expectedCount);
                }
            }
            while (expectedEnumerator.MoveNext());

            if (actualEnumerator.MoveNext())
            {
                actualCount++;

                while (actualEnumerator.MoveNext())
                    actualCount++;

                ReportMismatch(actualDiagnostics, actualCount, expectedCount);
            }
        }

        void ReportMismatch(IEnumerable<Diagnostic> actualDiagnostics, int actualCount, int expectedCount)
        {
            if (actualCount == 0)
            {
                Fail($"No diagnostic found, expected: {expectedCount}.");
            }
            else
            {
                Fail($"Mismatch between number of diagnostics, expected: {expectedCount} actual: {actualCount}.", actualDiagnostics);
            }
        }
    }

    private void VerifyDiagnostic(
        Diagnostic expectedDiagnostic,
        Diagnostic actualDiagnostic,
        string? message,
        IFormatProvider? formatProvider,
        bool verifyAdditionalLocations = false)
    {
        if (expectedDiagnostic.Id != actualDiagnostic.Id)
            Fail($"Diagnostic's ID expected to be \"{expectedDiagnostic.Id}\", actual: \"{actualDiagnostic.Id}\"{GetMessage()}");

        VerifyLocation(expectedDiagnostic.Location, actualDiagnostic.Location);

        if (verifyAdditionalLocations)
            VerifyAdditionalLocations(expectedDiagnostic.AdditionalLocations, actualDiagnostic.AdditionalLocations);

        if (message is not null)
            Assert.Equal(message, actualDiagnostic.GetMessage(formatProvider));

        void VerifyLocation(
            Location expectedLocation,
            Location actualLocation)
        {
            if (object.ReferenceEquals(expectedLocation, Location.None))
            {
                if (object.ReferenceEquals(actualLocation, Location.None))
                {
                    return;
                }
                else
                {
                    LinePosition linePosition = actualLocation.GetLineSpan().StartLinePosition;

                    Fail($"Diagnostic expected to have no location, actual location start on line {linePosition.Line + 1} at column {linePosition.Character + 1}");
                }
            }
            else if (object.ReferenceEquals(actualLocation, Location.None))
            {
                LinePosition linePosition = expectedLocation.GetLineSpan().StartLinePosition;

                Fail($"Diagnostic's location expected to start on line {linePosition.Line + 1} at column {linePosition.Character + 1}, actual diagnostic has no location");
            }

            VerifyFileLinePositionSpan(expectedLocation.GetLineSpan(), actualLocation.GetLineSpan());
        }

        void VerifyAdditionalLocations(
            IReadOnlyList<Location> expected,
            IReadOnlyList<Location> actual)
        {
            int expectedCount = expected.Count;
            int actualCount = actual.Count;

            if (expectedCount != actualCount)
                Fail($"{expectedCount} additional location(s) expected, actual: {actualCount}{GetMessage()}");

            for (int j = 0; j < actualCount; j++)
                VerifyLocation(expected[j], actual[j]);
        }

        void VerifyFileLinePositionSpan(
            FileLinePositionSpan expected,
            FileLinePositionSpan actual)
        {
            if (expected.Path != actual.Path)
                Fail($"Diagnostic expected to be in file \"{expected.Path}\", actual: \"{actual.Path}\"{GetMessage()}");

            string? message = VerifyLinePositionSpan(expected.Span, actual.Span);

            if (message is not null)
                Fail($"Diagnostic{message}{GetMessage()}");
        }

        string GetMessage()
        {
            return $"\r\n\r\nExpected diagnostic:\r\n{expectedDiagnostic}\r\n\r\nActual diagnostic:\r\n{actualDiagnostic}\r\n";
        }
    }

    private Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(
        Compilation compilation,
        DiagnosticAnalyzer analyzer,
        AnalyzerOptions options,
        IComparer<Diagnostic>? comparer = null,
        CancellationToken cancellationToken = default)
    {
        return GetAnalyzerDiagnosticsAsync(
            compilation,
            ImmutableArray.Create(analyzer),
            options,
            comparer,
            cancellationToken);
    }

    private async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(
        Compilation compilation,
        ImmutableArray<DiagnosticAnalyzer> analyzers,
        AnalyzerOptions options,
        IComparer<Diagnostic>? comparer = null,
        CancellationToken cancellationToken = default)
    {
        Exception? exception = null;
        DiagnosticAnalyzer? analyzer = null;

        var compilationWithAnalyzersOptions = new CompilationWithAnalyzersOptions(
            options: options,
            onAnalyzerException: (e, a, _) =>
            {
                exception = e;
                analyzer = a;
            },
            concurrentAnalysis: true,
            logAnalyzerExecutionTime: false,
            reportSuppressedDiagnostics: false,
            analyzerExceptionFilter: null);

        CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(analyzers, compilationWithAnalyzersOptions);

        ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken);

        if (exception is not null)
        {
            string message = (analyzer is not null)
                ? $"An exception occurred in analyzer '{analyzer.GetType()}'.{Environment.NewLine}{exception}"
                : exception.ToString();

            Fail(message);
        }

        return (comparer is not null)
            ? diagnostics.Sort(comparer)
            : diagnostics;
    }
}
