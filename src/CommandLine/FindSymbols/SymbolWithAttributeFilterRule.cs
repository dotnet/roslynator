// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.FindSymbols;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal class SymbolWithAttributeFilterRule : SymbolFilterRule
{
    public SymbolWithAttributeFilterRule(IEnumerable<MetadataName> attributeNames)
    {
        AttributeNames = new MetadataNameSet(attributeNames);
    }

    public override SymbolFilterReason Reason => SymbolFilterReason.WithAttribute;

    public MetadataNameSet AttributeNames { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Reason} {string.Join(" ", AttributeNames.Values)}";

    public override bool IsApplicable(ISymbol value)
    {
        return !value.IsKind(SymbolKind.Namespace);
    }

    public override bool IsMatch(ISymbol value)
    {
        foreach (AttributeData attribute in value.GetAttributes())
        {
            if (attribute.AttributeClass is not null
                && AttributeNames.Contains(attribute.AttributeClass))
            {
                return true;
            }
        }

        return false;
    }
}
