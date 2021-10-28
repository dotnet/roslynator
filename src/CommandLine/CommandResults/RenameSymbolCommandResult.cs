// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Roslynator.Rename;

namespace Roslynator.CommandLine
{
    internal class RenameSymbolCommandResult : CommandResult
    {
        public RenameSymbolCommandResult(CommandStatus status, ImmutableArray<SymbolRenameResult> renameResults)
            : base(status)
        {
            RenameResults = renameResults;
        }

        public ImmutableArray<SymbolRenameResult> RenameResults { get; }
    }
}
