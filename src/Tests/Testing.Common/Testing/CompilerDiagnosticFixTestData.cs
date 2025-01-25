// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Roslynator.Testing;

/// <summary>
/// Represents test data for a compiler diagnostic fix.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class CompilerDiagnosticFixTestData
{
    /// <summary>
    /// Initializes a new instance of <see cref="CompilerDiagnosticFixTestData"/>
    /// </summary>
    [Obsolete("This constructor is obsolete and will be removed in future versions.")]
    public CompilerDiagnosticFixTestData(
        string diagnosticId,
        string source,
        IEnumerable<AdditionalFile>? additionalFiles = null,
        string? equivalenceKey = null,
        string? path = null)
    {
        DiagnosticId = diagnosticId ?? throw new ArgumentNullException(nameof(diagnosticId));
        Source = source ?? throw new ArgumentNullException(nameof(source));
        AdditionalFiles = additionalFiles?.ToImmutableArray() ?? ImmutableArray<AdditionalFile>.Empty;
        EquivalenceKey = equivalenceKey;
        Path = path;
    }

#pragma warning disable CS0618 // Type or member is obsolete
    /// <summary>
    /// Initializes a new instance of <see cref="CompilerDiagnosticFixTestData"/>
    /// </summary>
    public CompilerDiagnosticFixTestData(
        string source,
        IEnumerable<AdditionalFile>? additionalFiles = null,
        string? equivalenceKey = null,
        string? path = null)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        AdditionalFiles = additionalFiles?.ToImmutableArray() ?? ImmutableArray<AdditionalFile>.Empty;
        EquivalenceKey = equivalenceKey;
        Path = path;
        DiagnosticId = null!;
    }

    internal CompilerDiagnosticFixTestData(CompilerDiagnosticFixTestData other)
        : this(
            diagnosticId: other.DiagnosticId,
            source: other.Source,
            additionalFiles: other.AdditionalFiles,
            equivalenceKey: other.EquivalenceKey,
            path: other.Path)
    {
    }
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>
    /// Gets compiler diagnostic ID to be fixed.
    /// </summary>
    [Obsolete("This property is obsolete and will be removed in future versions.")]
    public string DiagnosticId { get; }

    /// <summary>
    /// Gets a source code that will report specified compiler diagnostic.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Gets additional source files.
    /// </summary>
    public ImmutableArray<AdditionalFile> AdditionalFiles { get; }

    /// <summary>
    /// Gets code action's equivalence key.
    /// </summary>
    public string? EquivalenceKey { get; }

    /// <summary>
    /// Gets source file path.
    /// </summary>
    public string? Path { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Source}";

    /// <summary>
    /// Creates and return new instance of <see cref="CompilerDiagnosticFixTestData"/> updated with specified values.
    /// </summary>
    [Obsolete("This method is obsolete and will be removed in future version.")]
    public CompilerDiagnosticFixTestData Update(
        string diagnosticId,
        string source,
        IEnumerable<AdditionalFile> additionalFiles,
        string equivalenceKey,
        string path)
    {
        return new(
            diagnosticId: diagnosticId,
            source: source,
            additionalFiles: additionalFiles,
            equivalenceKey: equivalenceKey,
            path: path);
    }
}
