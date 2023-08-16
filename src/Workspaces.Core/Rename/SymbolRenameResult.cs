// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Rename;

internal readonly struct SymbolRenameResult
{
    public SymbolRenameResult(string newName, Solution newSolution)
    {
        NewName = newName;
        NewSolution = newSolution;
    }

    public string NewName { get; }

    public Solution NewSolution { get; }
}
