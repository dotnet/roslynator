// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation
{
    internal class DocumentationFilterOptions : SymbolFilterOptions
    {
        public static DocumentationFilterOptions Instance { get; } = new(
            visibility: VisibilityFilter.Public,
            symbolGroups: SymbolGroupFilter.TypeOrMember,
            rules: null,
            attributeRules: ImmutableArray.Create<AttributeFilterRule>(IgnoredAttributeNameFilterRule.Default));

        internal DocumentationFilterOptions(
            VisibilityFilter visibility = VisibilityFilter.All,
            SymbolGroupFilter symbolGroups = SymbolGroupFilter.TypeOrMember,
            IEnumerable<SymbolFilterRule> rules = null,
            IEnumerable<AttributeFilterRule> attributeRules = null) : base(visibility, symbolGroups, rules, attributeRules)
        {
        }
    }
}
