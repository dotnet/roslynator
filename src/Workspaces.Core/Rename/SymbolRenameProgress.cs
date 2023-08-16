// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Rename;

/// <summary>
/// Represents in information about renaming a symbol.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SymbolRenameProgress
{
    /// <summary>
    /// Initializes a new instance of <see cref="SymbolRenameProgress"/>.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="result"></param>
    /// <param name="newName"></param>
    /// <param name="exception"></param>
    internal SymbolRenameProgress(
        ISymbol symbol,
        SymbolRenameResult result,
        string newName = null,
        Exception exception = null)
    {
        Symbol = symbol;
        Result = result;
        NewName = newName;
        Exception = exception;
    }

    /// <summary>
    /// Symbols being renamed.
    /// </summary>
    public ISymbol Symbol { get; }

    public SymbolRenameResult Result { get; }

    /// <summary>
    /// New name of the symbol.
    /// </summary>
    public string NewName { get; }

    /// <summary>
    /// Exception that occurred during renaming. May be <c>null</c>.
    /// </summary>
    public Exception Exception { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => Symbol.ToDisplayString(SymbolDisplayFormats.FullName);
}
