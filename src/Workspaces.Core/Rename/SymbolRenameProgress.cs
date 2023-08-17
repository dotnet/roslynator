// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Rename;

/// <summary>
/// Represents in information about renaming a symbol.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct SymbolRenameProgress
{
    /// <summary>
    /// Initializes a new instance of <see cref="SymbolRenameProgress"/>.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="newName"></param>
    /// <param name="result"></param>
    /// <param name="exception"></param>
    internal SymbolRenameProgress(
        ISymbol symbol,
        string newName,
        SymbolRenameResult result,
        Exception exception = null)
    {
        Symbol = symbol;
        NewName = newName;
        Result = result;
        Exception = exception;
    }

    /// <summary>
    /// Symbols being renamed.
    /// </summary>
    public ISymbol Symbol { get; }

    /// <summary>
    /// New name of the symbol.
    /// </summary>
    public string NewName { get; }

    /// <summary>
    /// /// Specifies if the rename operation succeeded or not.
    /// </summary>
    public SymbolRenameResult Result { get; }

    /// <summary>
    /// Exception that occurred during renaming. May be <c>null</c>.
    /// </summary>
    public Exception Exception { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => Symbol.ToDisplayString(SymbolDisplayFormats.FullName);
}
