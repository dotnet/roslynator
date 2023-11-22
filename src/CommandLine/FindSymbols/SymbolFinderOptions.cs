// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.FindSymbols;

internal class SymbolFinderOptions : SymbolFilterOptions
{
    internal SymbolFinderOptions(
        FileSystemFilter fileSystemFilter = null,
        VisibilityFilter visibility = VisibilityFilter.All,
        SymbolGroupFilter symbolGroups = SymbolGroupFilter.TypeOrMember,
        IEnumerable<SymbolFilterRule> rules = null,
        IEnumerable<AttributeFilterRule> attributeRules = null,
        bool ignoreGeneratedCode = false,
        bool unused = false) : base(fileSystemFilter, visibility, symbolGroups, rules, attributeRules)
    {
        IgnoreGeneratedCode = ignoreGeneratedCode;
        Unused = unused;
    }

    new public static SymbolFinderOptions Default { get; } = new();

    public bool IgnoreGeneratedCode { get; }

    public bool Unused { get; }
}
