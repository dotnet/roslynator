// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;

namespace Roslynator.Rename;

#pragma warning disable RCS1223 // Mark publicly visible type with DebuggerDisplay attribute.

public class SymbolRenamerOptions
{
    internal static SymbolRenamerOptions Default { get; } = new();

    public bool SkipTypes { get; init; }

    public bool SkipMembers { get; init; }

    public bool SkipLocals { get; init; }

    internal VisibilityFilter VisibilityFilter { get; init; } = VisibilityFilter.All;

    public CompilationErrorResolution ErrorResolution { get; init; } = CompilationErrorResolution.Throw;

    public ImmutableHashSet<string> IgnoredCompilerDiagnosticIds { get; init; } = ImmutableHashSet<string>.Empty;

    public bool IncludeGeneratedCode { get; init; }

    public bool DryRun { get; init; }

    public bool RenameOverloads { get; init; }

    public bool RenameInStrings { get; init; }

    public bool RenameInComments { get; init; }

    public bool RenameFile { get; init; }
}
