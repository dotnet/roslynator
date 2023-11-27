// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Roslynator.Rename;

#pragma warning disable RCS1223 // Mark publicly visible type with DebuggerDisplay attribute.

/// <summary>
/// Represents options for <see cref="SymbolRenamer"/>.
/// </summary>
public class SymbolRenamerOptions
{
    /// <summary>
    /// Do not rename type symbols (classes, structs, interfaces etc.).
    /// </summary>
    public bool SkipTypes { get; set; }

    /// <summary>
    /// Do not rename member symbols (methods, properties, fields etc.).
    /// </summary>
    public bool SkipMembers { get; set; }

    /// <summary>
    /// Do not rename local symbols (like local variables).
    /// </summary>
    public bool SkipLocals { get; set; }

    public CompilationErrorResolution CompilationErrorResolution { get; set; } = CompilationErrorResolution.Throw;

    /// <summary>
    /// A list of compiler diagnostic IDs that should be ignored.
    /// </summary>
    public HashSet<string> IgnoredCompilerDiagnosticIds { get; } = new();

    /// <summary>
    /// Include symbols that are part of generated code.
    /// </summary>
    public bool IncludeGeneratedCode { get; set; }

    /// <summary>
    /// Do not save changes to disk.
    /// </summary>
    public bool DryRun { get; set; }

    /// <summary>
    /// If the symbol is a method rename its overloads as well.
    /// </summary>
    public bool RenameOverloads { get; set; }

    /// <summary>
    /// Rename identifiers in string literals that match the name of the symbol.
    /// </summary>
    public bool RenameInStrings { get; set; }

    /// <summary>
    /// Rename identifiers in comments that match the name of the symbol.
    /// </summary>
    public bool RenameInComments { get; set; }

    /// <summary>
    /// If the symbol is a type renames the file containing the type declaration as well.
    /// </summary>
    public bool RenameFile { get; set; }

    //TODO: SymbolRenameOptions.FileSystemMatcher
    internal Matcher FileSystemMatcher { get; set; }
}
