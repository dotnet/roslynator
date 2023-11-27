// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Spelling;

internal class SpellcheckOptions
{
    public static SpellcheckOptions Default { get; } = new();

    public FileSystemFilter FileSystemFilter { get; init; }

    public SpellingScopeFilter ScopeFilter { get; init; } = SpellingScopeFilter.All;

    public VisibilityFilter SymbolVisibility { get; init; } = VisibilityFilter.All;

    public SplitMode SplitMode { get; init; } = SplitMode.CaseAndHyphen;

    public int MinWordLength { get; init; } = 3;

    public int MaxWordLength { get; init; } = int.MaxValue;

    public int CodeContext { get; init; }

    public bool IncludeGeneratedCode { get; init; }

    public bool Autofix { get; init; } = true;

    public bool Interactive { get; init; }

    public bool DryRun { get; init; }
}
