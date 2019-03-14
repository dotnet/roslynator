// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.FindSymbols
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class IgnoredAttributeNameFilterRule : AttributeFilterRule
    {
        public MetadataNameSet AttributeNames { get; }

        public IgnoredAttributeNameFilterRule(IEnumerable<MetadataName> values)
        {
            AttributeNames = new MetadataNameSet(values);
        }

        public override bool IsApplicable(AttributeData value)
        {
            return true;
        }

        public override SymbolFilterReason Reason => SymbolFilterReason.Ignored;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Reason} {string.Join(" ", AttributeNames.Values)}";

        public override bool IsMatch(AttributeData value)
        {
            return !AttributeNames.Contains(value.AttributeClass);
        }
    }
}
