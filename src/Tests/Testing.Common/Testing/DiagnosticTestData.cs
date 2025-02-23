// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Testing;

/// <summary>
/// Represents test data for a diagnostic and its fix.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class DiagnosticTestData
{
    /// <summary>
    /// Initializes a new instance of <see cref="DiagnosticTestData"/>.
    /// </summary>
    [Obsolete("This constructor is obsolete and will be removed in future versions.")]
    public DiagnosticTestData(
        DiagnosticDescriptor descriptor,
        string source,
        IEnumerable<TextSpan>? spans,
        IEnumerable<TextSpan>? additionalSpans = null,
        IEnumerable<AdditionalFile>? additionalFiles = null,
        string? diagnosticMessage = null,
        IFormatProvider? formatProvider = null,
        string? equivalenceKey = null,
        bool alwaysVerifyAdditionalLocations = false,
        string? directoryPath = null,
        string? fileName = null)
    {
        Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Spans = spans?.ToImmutableArray() ?? ImmutableArray<TextSpan>.Empty;
        AdditionalSpans = additionalSpans?.ToImmutableArray() ?? ImmutableArray<TextSpan>.Empty;
        AdditionalFiles = additionalFiles?.ToImmutableArray() ?? ImmutableArray<AdditionalFile>.Empty;
        DiagnosticMessage = diagnosticMessage;
        FormatProvider = formatProvider;
        EquivalenceKey = equivalenceKey;
        AlwaysVerifyAdditionalLocations = alwaysVerifyAdditionalLocations;

        FileSystemVerifier.VerifyDirectoryPath(directoryPath);
        DirectoryPath = directoryPath;

        FileSystemVerifier.VerifyFileName(fileName);
        FileName = fileName;

        if (Spans.Length > 1
            && !AdditionalSpans.IsEmpty)
        {
            throw new ArgumentException("", nameof(additionalSpans));
        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    /// <summary>
    /// Initializes a new instance of <see cref="DiagnosticTestData"/>.
    /// </summary>
    public DiagnosticTestData(
        string source,
        IEnumerable<TextSpan>? spans,
        IEnumerable<TextSpan>? additionalSpans = null,
        IEnumerable<AdditionalFile>? additionalFiles = null,
        string? diagnosticMessage = null,
        IFormatProvider? formatProvider = null,
        string? equivalenceKey = null,
        bool alwaysVerifyAdditionalLocations = false,
        string? directoryPath = null,
        string? fileName = null)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Spans = spans?.ToImmutableArray() ?? ImmutableArray<TextSpan>.Empty;
        AdditionalSpans = additionalSpans?.ToImmutableArray() ?? ImmutableArray<TextSpan>.Empty;
        AdditionalFiles = additionalFiles?.ToImmutableArray() ?? ImmutableArray<AdditionalFile>.Empty;
        DiagnosticMessage = diagnosticMessage;
        FormatProvider = formatProvider;
        EquivalenceKey = equivalenceKey;
        AlwaysVerifyAdditionalLocations = alwaysVerifyAdditionalLocations;

        FileSystemVerifier.VerifyDirectoryPath(directoryPath);
        DirectoryPath = directoryPath;

        FileSystemVerifier.VerifyFileName(fileName);
        FileName = fileName;

        Descriptor = null!;

        if (Spans.Length > 1
            && !AdditionalSpans.IsEmpty)
        {
            throw new ArgumentException("", nameof(additionalSpans));
        }
    }

    internal DiagnosticTestData(DiagnosticTestData other)
        : this(
            descriptor: other.Descriptor,
            source: other.Source,
            spans: other.Spans,
            additionalSpans: other.AdditionalSpans,
            additionalFiles: other.AdditionalFiles,
            diagnosticMessage: other.DiagnosticMessage,
            formatProvider: other.FormatProvider,
            equivalenceKey: other.EquivalenceKey,
            alwaysVerifyAdditionalLocations: other.AlwaysVerifyAdditionalLocations,
            directoryPath: other.DirectoryPath,
            fileName: other.FileName)
    {
    }
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>
    /// Gets diagnostic's descriptor.
    /// </summary>
    [Obsolete("This property is obsolete and will be removed in future versions.")]
    public DiagnosticDescriptor Descriptor { get; }

    /// <summary>
    /// Gets source that will report specified diagnostic.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Gets diagnostic's locations.
    /// </summary>
    public ImmutableArray<TextSpan> Spans { get; }

    /// <summary>
    /// Gets diagnostic's additional locations.
    /// </summary>
    public ImmutableArray<TextSpan> AdditionalSpans { get; }

    /// <summary>
    /// Gets additional source files.
    /// </summary>
    public ImmutableArray<AdditionalFile> AdditionalFiles { get; }

    /// <summary>
    /// Gets diagnostic's message
    /// </summary>
    public string? DiagnosticMessage { get; }

    /// <summary>
    /// Gets format provider to be used to format diagnostic's message.
    /// </summary>
    public IFormatProvider? FormatProvider { get; }

    /// <summary>
    /// Gets code action's equivalence key.
    /// </summary>
    public string? EquivalenceKey { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Source}";

    /// <summary>
    /// True if additional locations should be always verified.
    /// </summary>
    public bool AlwaysVerifyAdditionalLocations { get; }

    /// <summary>
    /// Gets the relative directory path.
    /// </summary>
    public string? DirectoryPath { get; }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string? FileName { get; }

    internal ImmutableArray<Diagnostic> GetDiagnostics(DiagnosticDescriptor descriptor, SyntaxTree tree)
    {
        if (Spans.IsEmpty)
        {
            return ImmutableArray.Create(Diagnostic.Create(descriptor, Location.None));
        }
        else
        {
            return ImmutableArray.CreateRange(
                Spans,
                span => Diagnostic.Create(
                    descriptor,
                    Location.Create(tree, span),
                    additionalLocations: AdditionalSpans.Select(span => Location.Create(tree, span)).ToImmutableArray()));
        }
    }

    /// <summary>
    /// Creates and return new instance of <see cref="DiagnosticTestData"/> updated with specified values.
    /// </summary>
    [Obsolete("This method is obsolete and will be removed in future version.")]
    public DiagnosticTestData Update(
        DiagnosticDescriptor descriptor,
        string source,
        IEnumerable<TextSpan> spans,
        IEnumerable<TextSpan> additionalSpans,
        IEnumerable<AdditionalFile> additionalFiles,
        string diagnosticMessage,
        IFormatProvider formatProvider,
        string equivalenceKey,
        bool alwaysVerifyAdditionalLocations)
    {
        return new(
            descriptor: descriptor,
            source: source,
            spans: spans,
            additionalSpans: additionalSpans,
            additionalFiles: additionalFiles,
            diagnosticMessage: diagnosticMessage,
            formatProvider: formatProvider,
            equivalenceKey: equivalenceKey,
            alwaysVerifyAdditionalLocations: alwaysVerifyAdditionalLocations);
    }
}
