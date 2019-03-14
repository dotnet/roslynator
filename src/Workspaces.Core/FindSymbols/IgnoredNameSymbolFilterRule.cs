// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.FindSymbols
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class IgnoredNameSymbolFilterRule : SymbolFilterRule
    {
        public override SymbolFilterReason Reason => SymbolFilterReason.Ignored;

        public MetadataNameSet Names { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Reason} {string.Join(" ", Names.Values)}";

        public IgnoredNameSymbolFilterRule(IEnumerable<MetadataName> values)
        {
            Names = new MetadataNameSet(values);
        }

        public override bool IsApplicable(ISymbol value)
        {
            return true;
        }

        public override bool IsMatch(ISymbol value)
        {
            if (Names.Contains(value.ContainingNamespace))
                return false;

            switch (value.Kind)
            {
                case SymbolKind.Namespace:
                case SymbolKind.NamedType:
                    {
                        if (Names.Contains(value))
                            return false;

                        break;
                    }
            }

            return true;
        }
    }
}
