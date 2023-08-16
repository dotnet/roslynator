// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Rename;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SymbolRenameProgress
{
    internal SymbolRenameProgress(
        ISymbol symbol,
        SymbolRenameProgressKind kind,
        string newName = null,
        Exception exception = null)
    {
        Symbol = symbol;
        Kind = kind;
        NewName = newName;
        Exception = exception;
    }

    public ISymbol Symbol { get; }

    public SymbolRenameProgressKind Kind { get; }

    public string NewName { get; }

    public Exception Exception { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => Symbol.ToDisplayString(SymbolDisplayFormats.FullName);
}
