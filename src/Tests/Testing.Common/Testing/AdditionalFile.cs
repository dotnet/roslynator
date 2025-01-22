// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Roslynator.Testing;

/// <summary>
/// Represents additional code file.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct AdditionalFile
{
    /// <summary>
    /// Initializes a new instance of <see cref="AdditionalFile"/>
    /// </summary>
    public AdditionalFile(string source, string? expectedSource = null, string? path = null)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        ExpectedSource = expectedSource;

        FilePathVerifier.VerifyFilePath(path);
        Path = path;
    }

    /// <summary>
    /// Gets a source code.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Gets expected source code.
    /// </summary>
    public string? ExpectedSource { get; }

    /// <summary>
    /// Gets the relative path a source file.
    /// </summary>
    public string? Path { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => Source;

    internal static ImmutableArray<AdditionalFile> CreateRange(IEnumerable<string>? additionalFiles)
    {
        return additionalFiles?.Select(f => new AdditionalFile(f)).ToImmutableArray()
            ?? ImmutableArray<AdditionalFile>.Empty;
    }

    internal static ImmutableArray<AdditionalFile> CreateRange(IEnumerable<(string source, string expectedSource)>? additionalFiles)
    {
        return additionalFiles?.Select(f => new AdditionalFile(f.source, f.expectedSource)).ToImmutableArray()
            ?? ImmutableArray<AdditionalFile>.Empty;
    }
}
