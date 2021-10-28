// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Roslynator.Rename
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct SymbolRenameResult
    {
        public SymbolRenameResult(string oldName, string newName, string symbolId)
        {
            OldName = oldName;
            NewName = newName;
            SymbolId = symbolId;
        }

        public string OldName { get; }

        public string NewName { get; }

        public string SymbolId { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => (OldName != null) ? $"{OldName}  {NewName}  {SymbolId}" : "Uninitialized";
    }
}
