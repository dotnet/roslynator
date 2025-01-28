// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Testing;

/// <summary>
/// Gets test data for a code refactoring.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class RefactoringTestData
{
    /// <summary>
    /// Initializes a new instance of <see cref="RefactoringTestData"/>.
    /// </summary>
    public RefactoringTestData(
        string source,
        IEnumerable<TextSpan> spans,
        IEnumerable<AdditionalFile>? additionalFiles = null,
        string? equivalenceKey = null,
        string? directoryPath = null,
        string? fileName = null)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Spans = spans?.ToImmutableArray() ?? ImmutableArray<TextSpan>.Empty;
        AdditionalFiles = additionalFiles?.ToImmutableArray() ?? ImmutableArray<AdditionalFile>.Empty;
        EquivalenceKey = equivalenceKey;

        FileSystemVerifier.VerifyDirectoryPath(directoryPath);
        DirectoryPath = directoryPath;

        FileSystemVerifier.VerifyFileName(fileName);
        FileName = fileName;
    }

    /// <summary>
    /// Gets a source code to be refactored.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Gets text spans on which a code refactoring will be applied.
    /// </summary>
    public ImmutableArray<TextSpan> Spans { get; }

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
    private string DebuggerDisplay => Source;

    internal RefactoringTestData(RefactoringTestData other)
        : this(
            source: other.Source,
            spans: other.Spans,
            additionalFiles: other.AdditionalFiles,
            equivalenceKey: other.EquivalenceKey,
            directoryPath: other.DirectoryPath,
            fileName: other.FileName)
    {
    }

    /// <summary>
    /// Creates and return new instance of <see cref="RefactoringTestData"/> updated with specified values.
    /// </summary>
    [Obsolete("This method is obsolete and will be removed in future version.")]
    public RefactoringTestData Update(
        string source,
        IEnumerable<TextSpan> spans,
        IEnumerable<AdditionalFile> additionalFiles,
        string equivalenceKey,
        string directoryPath,
        string fileName)
    {
        return new(
            source: source,
            spans: spans,
            additionalFiles: additionalFiles,
            equivalenceKey: equivalenceKey,
            directoryPath: directoryPath,
            fileName: fileName);
    }
}
