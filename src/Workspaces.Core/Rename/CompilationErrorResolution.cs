// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Rename;

/// <summary>
/// Specifies how to handle compilation errors that occur after renaming a symbol.
/// </summary>
public enum CompilationErrorResolution
{
    /// <summary>
    /// Ignore compilation errors.
    /// </summary>
    Ignore = 0,

    /// <summary>
    /// Throw an exception if renaming of a symbol causes compilation errors.
    /// </summary>
    Throw = 1,

    /// <summary>
    /// Skip renaming of a symbol if it causes compilation errors.
    /// </summary>
    Skip = 2,
}
