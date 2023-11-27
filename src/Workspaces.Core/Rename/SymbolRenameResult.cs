// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Rename;

/// <summary>
/// Specifies the result of renaming a symbol.
/// </summary>
public enum SymbolRenameResult
{
    /// <summary>
    /// Symbol was renamed successfully.
    /// </summary>
    Success,

    /// <summary>
    /// <see cref="Microsoft.CodeAnalysis.Rename.Renamer"/> throws an exception.
    /// </summary>
    Error,

    /// <summary>
    /// Renaming of a symbol caused compilation errors.
    /// </summary>
    CompilationError,
}
