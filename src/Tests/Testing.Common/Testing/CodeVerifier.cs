﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;
using static System.Environment;

namespace Roslynator.Testing;

/// <summary>
/// Represents base type for verifying a diagnostic, a code fix and a refactoring.
/// </summary>
public abstract class CodeVerifier
{
    internal CodeVerifier(IAssert assert)
    {
        Assert = assert ?? throw new ArgumentNullException(nameof(assert));
    }

    /// <summary>
    /// Gets a common code verification options.
    /// </summary>
    protected abstract TestOptions CommonOptions { get; }

    /// <summary>
    /// Gets a code verification options.
    /// </summary>
    public TestOptions Options => CommonOptions;

    internal IAssert Assert { get; }

    internal void Fail(string userMessage)
    {
        Assert.True(false, userMessage);
    }

    internal void Fail(string userMessage, IEnumerable<Diagnostic> diagnostics)
    {
        string s = string.Join(NewLine, diagnostics.Select(d => d.ToString()));

        if (s.Length == 0)
            s = "-";

        Fail(userMessage + $"{NewLine}{NewLine}Diagnostics:{NewLine}{s}{NewLine}");
    }

    internal void Fail(string userMessage, IEnumerable<CodeAction>? codeActions)
    {
        var s = "";

        if (codeActions is not null)
            s = string.Join(NewLine, codeActions.Select(a => $"\"{a.Title}\", EquivalenceKey: {a.EquivalenceKey}"));

        if (s.Length == 0)
            s = "-";

        Fail(userMessage + $"{NewLine}{NewLine}Candidate actions:{NewLine}{s}{NewLine}");
    }

    internal void VerifyCompilerDiagnostics(
        ImmutableArray<Diagnostic> diagnostics,
        TestOptions options)
    {
        foreach (Diagnostic diagnostic in diagnostics)
        {
            if (!options.IsAllowedCompilerDiagnostic(diagnostic))
            {
                Fail($"No compiler diagnostics with severity higher than '{options.AllowedCompilerDiagnosticSeverity}' expected.",
                    diagnostics.Where(d => !options.IsAllowedCompilerDiagnostic(d)));
            }
        }
    }

    internal void VerifyNoNewCompilerDiagnostics(
        ImmutableArray<Diagnostic> diagnostics,
        ImmutableArray<Diagnostic> newDiagnostics,
        TestOptions options)
    {
        foreach (Diagnostic newDiagnostic in newDiagnostics)
        {
            if (!options.IsAllowedCompilerDiagnostic(newDiagnostic)
                && IsNewCompilerDiagnostic(newDiagnostic))
            {
                IEnumerable<Diagnostic> diff = newDiagnostics
                    .Where(diagnostic => !options.IsAllowedCompilerDiagnostic(diagnostic))
                    .Except(diagnostics, DiagnosticDeepEqualityComparer.Instance);

                var message = "Code fix introduced new compiler diagnostic";

                if (diff.Count() > 1)
                    message += "s";

                message += ".";

                Fail(message, diff);
            }
        }

        bool IsNewCompilerDiagnostic(Diagnostic newDiagnostic)
        {
            foreach (Diagnostic diagnostic in diagnostics)
            {
                if (DiagnosticDeepEqualityComparer.Instance.Equals(diagnostic, newDiagnostic))
                    return false;
            }

            return true;
        }
    }

    internal async Task VerifyAdditionalDocumentsAsync(
        Project project,
        ImmutableArray<ExpectedDocument> expectedDocuments,
        CancellationToken cancellationToken = default)
    {
        foreach (ExpectedDocument expectedDocument in expectedDocuments)
        {
            Document? document = project.GetDocument(expectedDocument.Id);

            Assert.NotNull(document);

            SyntaxNode? root = await document!.GetSyntaxRootAsync(simplify: true, format: true, cancellationToken);

            Assert.NotNull(root);

            string actual = root!.ToFullString();

            Assert.Equal(expectedDocument.Text, actual);
        }
    }

    internal async Task<Document> VerifyAndApplyCodeActionAsync(
        Document document,
        CodeAction codeAction,
        string? title)
    {
        if (title is not null)
            Assert.Equal(title, codeAction.Title);

        ImmutableArray<CodeActionOperation> operations = await codeAction.GetOperationsAsync(CancellationToken.None);

        Document? newDocument = operations
            .OfType<ApplyChangesOperation>()
            .Single()
            .ChangedSolution
            .GetDocument(document.Id);

        Assert.NotNull(newDocument);

        return newDocument!;
    }

    internal void VerifySupportedDiagnostics(
        DiagnosticAnalyzer analyzer,
        ImmutableArray<Diagnostic> diagnostics)
    {
        foreach (Diagnostic diagnostic in diagnostics)
            VerifySupportedDiagnostics(analyzer, diagnostic);
    }

    internal void VerifySupportedDiagnostics(DiagnosticAnalyzer analyzer, Diagnostic diagnostic)
    {
        if (analyzer.SupportedDiagnostics.IndexOf(diagnostic.Descriptor, DiagnosticDescriptorComparer.Id) == -1)
            Fail($"Diagnostic \"{diagnostic.Id}\" is not supported by '{analyzer.GetType().Name}'.");
    }

    internal void VerifyFixableDiagnostics(CodeFixProvider fixProvider, string diagnosticId)
    {
        ImmutableArray<string> fixableDiagnosticIds = fixProvider.FixableDiagnosticIds;

        if (!fixableDiagnosticIds.Contains(diagnosticId))
            Fail($"Diagnostic '{diagnosticId}' is not fixable by '{fixProvider.GetType().Name}'.");
    }

    internal async Task VerifyExpectedDocument(
        ExpectedTestState expected,
        Document document,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(simplify: true, format: true, cancellationToken);

        Assert.NotNull(root);

        string actual = root!.ToFullString();

        Assert.Equal(expected.Source, actual);

        if (!expected.Annotations.IsEmpty
            || !expected.AlwaysVerifyAnnotations.IsEmpty)
        {
            VerifyAnnotations(expected, root, actual);
        }
    }

    private void VerifyAnnotations(
        ExpectedTestState expected,
        SyntaxNode root,
        string source)
    {
        foreach (KeyValuePair<string, ImmutableArray<TextSpan>> kvp in expected.AnnotationsByKind)
        {
            string kind = GetAnnotationKind(kvp.Key);
            ImmutableArray<TextSpan> spans = kvp.Value;

            VerifyAnnotations(root, source, kind, spans);
        }

        foreach (string kind in expected.AlwaysVerifyAnnotations)
        {
            if (!expected.AnnotationsByKind.ContainsKey(kind))
            {
                ImmutableArray<TextSpan> spans = expected.AnnotationsByKind.GetValueOrDefault(kind, ImmutableArray<TextSpan>.Empty);

                VerifyAnnotations(root, source, kind, spans);
            }
        }

        static string GetAnnotationKind(string value)
        {
            if (string.Equals(value, "n", StringComparison.OrdinalIgnoreCase))
                return "CodeAction_Navigation";

            if (string.Equals(value, "r", StringComparison.OrdinalIgnoreCase))
                return RenameAnnotation.Kind;

            return value;
        }
    }

    private void VerifyAnnotations(
        SyntaxNode root,
        string source,
        string kind,
        ImmutableArray<TextSpan> spans)
    {
        ImmutableArray<SyntaxToken> tokens = root.GetAnnotatedTokens(kind).OrderBy(f => f.SpanStart).ToImmutableArray();

        if (spans.Length != tokens.Length)
            Fail($"{spans.Length} '{kind}' annotation(s) expected, actual: {tokens.Length}");

        for (int i = 0; i < spans.Length; i++)
        {
            TextSpan expectedSpan = spans[i];
            TextSpan actualSpan = tokens[i].Span;

            if (expectedSpan != actualSpan)
            {
                string? message = VerifyLinePositionSpan(
                    expectedSpan.ToLinePositionSpan(source),
                    actualSpan.ToLinePositionSpan(source));

                if (message is not null)
                    Fail($"Annotation '{kind}'{message}");
            }
        }
    }

    internal static string? VerifyLinePositionSpan(LinePositionSpan expected, LinePositionSpan actual)
    {
        return VerifyLinePosition(expected.Start, actual.Start, "start")
            ?? VerifyLinePosition(expected.End, actual.End, "end");
    }

    private static string? VerifyLinePosition(
        LinePosition expected,
        LinePosition actual,
        string startOrEnd)
    {
        int expectedLine = expected.Line;
        int actualLine = actual.Line;

        if (expectedLine != actualLine)
            return $" expected to {startOrEnd} on line {expectedLine + 1}, actual: {actualLine + 1}";

        int expectedCharacter = expected.Character;
        int actualCharacter = actual.Character;

        if (expectedCharacter != actualCharacter)
            return $" expected to {startOrEnd} at column {expectedCharacter + 1}, actual: {actualCharacter + 1}";

        return null;
    }

    internal static (Document document, ImmutableArray<ExpectedDocument> expectedDocuments)
        CreateDocument(Solution solution, string source, string? directoryPath, string? fileName, ImmutableArray<AdditionalFile> additionalFiles, TestOptions options, DiagnosticDescriptor? descriptor = null)
    {
        const string DefaultProjectName = "TestProject";

        ProjectId projectId = ProjectId.CreateNewId();

        ProjectInfo projectInfo = ProjectInfo.Create(
            id: projectId,
            version: VersionStamp.Default,
            name: DefaultProjectName,
            assemblyName: DefaultProjectName,
            language: options.Language,
            compilationOptions: options.CompilationOptions,
            parseOptions: options.ParseOptions,
            metadataReferences: options.MetadataReferences);

        Project project = solution
            .AddProject(projectInfo)
            .GetProject(projectId)!;

        string baseDirectoryPath = (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            ? "z:"
            : "/";

        if (options.ConfigOptions.Count > 0)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            sb.AppendLine("root = true");
            sb.AppendLine();
            sb.AppendLine("[*.*]");
            foreach (KeyValuePair<string, string> configOption in options.ConfigOptions)
            {
                sb.Append(configOption.Key);
                sb.Append(" = ");
                sb.AppendLine(configOption.Value);
            }

            TextDocument configFile = project.AddAnalyzerConfigDocument(
                ".editorconfig",
                SourceText.From(StringBuilderCache.GetStringAndFree(sb)),
                filePath: Path.Combine(baseDirectoryPath, ".editorconfig"));

            project = configFile.Project;
        }

        if (descriptor is not null)
        {
            CompilationOptions newCompilationOptions = project.CompilationOptions!.EnsureDiagnosticEnabled(descriptor);

            project = project.WithCompilationOptions(newCompilationOptions);
        }

        string documentName = fileName ?? options.DocumentName;

        Document document = project.AddDocument(
            documentName,
            SourceText.From(source),
            filePath: Path.Combine(baseDirectoryPath, (directoryPath is not null) ? Path.Combine(directoryPath, documentName) : documentName));

        ImmutableArray<ExpectedDocument>.Builder? expectedDocuments = null;

        if (!additionalFiles.IsEmpty)
        {
            expectedDocuments = ImmutableArray.CreateBuilder<ExpectedDocument>();
            project = document.Project;

            for (int i = 0; i < additionalFiles.Length; i++)
            {
                string additionalDocumentName = additionalFiles[i].Name ?? AppendNumberToFileName(options.DocumentName, i + 2);

                Document additionalDocument = project.AddDocument(
                    additionalDocumentName,
                    SourceText.From(additionalFiles[i].Source),
                    filePath: Path.Combine(baseDirectoryPath, (directoryPath is not null) ? Path.Combine(directoryPath, additionalDocumentName) : additionalDocumentName));

                string? expectedSource = additionalFiles[i].ExpectedSource;

                if (expectedSource is not null)
                    expectedDocuments.Add(new ExpectedDocument(additionalDocument.Id, expectedSource));

                project = additionalDocument.Project;
            }

            document = project.GetDocument(document.Id)!;
        }

        return (document, expectedDocuments?.ToImmutableArray() ?? ImmutableArray<ExpectedDocument>.Empty);

        static string AppendNumberToFileName(string fileName, int number)
        {
            int index = fileName.LastIndexOf(".");

            return fileName.Insert(index, number.ToString(CultureInfo.InvariantCulture));
        }
    }
}
