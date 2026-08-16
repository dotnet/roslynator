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
    public CompilerDiagnosticFixTestData(
        string source,
        IEnumerable<AdditionalFile>? additionalFiles = null,
        string? equivalenceKey = null,
        string? directoryPath = null,
        string? fileName = null)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        AdditionalFiles = additionalFiles?.ToImmutableArray() ?? ImmutableArray<AdditionalFile>.Empty;
        EquivalenceKey = equivalenceKey;

        FileSystemVerifier.VerifyDirectoryPath(directoryPath);
        DirectoryPath = directoryPath;

        FileSystemVerifier.VerifyFileName(fileName);
        FileName = fileName;
    }

    internal CompilerDiagnosticFixTestData(CompilerDiagnosticFixTestData other)
        : this(
            source: other.Source,
            additionalFiles: other.AdditionalFiles,
            equivalenceKey: other.EquivalenceKey,
            directoryPath: other.DirectoryPath,
            fileName: other.FileName)
    {
    }

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
    /// Gets the relative directory path.
    /// </summary>
    public string? DirectoryPath { get; }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string? FileName { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Source}";
}
