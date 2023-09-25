// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Rename;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly struct SymbolData
{
    public SymbolData(ISymbol symbol, string id, DocumentId documentId)
    {
        Symbol = symbol;
        Id = id;
        DocumentId = documentId;
    }

    public ISymbol Symbol { get; }

    public string Id { get; }

    public DocumentId DocumentId { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => (Symbol is not null) ? $"{Symbol.Name}  {Id}" : "Uninitialized";
}
