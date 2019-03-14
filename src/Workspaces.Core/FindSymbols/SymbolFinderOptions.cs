// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Roslynator.FindSymbols;

namespace Roslynator.FindSymbols
{
    internal class SymbolFinderOptions : SymbolFilterOptions
    {
        internal SymbolFinderOptions(
            VisibilityFilter visibility = VisibilityFilter.All,
            SymbolGroupFilter symbolGroups = SymbolGroupFilter.TypeOrMember,
            IEnumerable<SymbolFilterRule> rules = null,
            IEnumerable<AttributeFilterRule> attributeRules = null,
            bool ignoreGeneratedCode = false,
            bool unusedOnly = false) : base(visibility, symbolGroups, rules, attributeRules)
        {
            IgnoreGeneratedCode = ignoreGeneratedCode;
            UnusedOnly = unusedOnly;
        }

        new public static SymbolFinderOptions Default { get; } = new SymbolFinderOptions();

        public bool IgnoreGeneratedCode { get; }

        public bool UnusedOnly { get; }
    }
}
