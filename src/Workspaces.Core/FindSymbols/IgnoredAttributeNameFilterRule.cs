// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.FindSymbols;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal class IgnoredAttributeNameFilterRule : AttributeFilterRule
{
    public static IgnoredAttributeNameFilterRule Default { get; } = DefaultIgnoredAttributeNameFilterRule.Instance;

    public MetadataNameSet AttributeNames { get; }

    public IgnoredAttributeNameFilterRule(IEnumerable<MetadataName> values)
    {
        AttributeNames = new MetadataNameSet(values);
    }

    public override bool IsApplicable(AttributeInfo value)
    {
        return true;
    }

    public override SymbolFilterReason Reason => SymbolFilterReason.Ignored;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{Reason} {string.Join(" ", AttributeNames.Values)}";

    public override bool IsMatch(AttributeInfo attributeInfo)
    {
        return attributeInfo.AttributeClass is null
            || !AttributeNames.Contains(attributeInfo.AttributeClass);
    }

    private class DefaultIgnoredAttributeNameFilterRule : IgnoredAttributeNameFilterRule
    {
        private static readonly MetadataName _defaultMemberAttribute = MetadataName.Parse("System.Reflection.DefaultMemberAttribute");

        public static DefaultIgnoredAttributeNameFilterRule Instance { get; } = new(GetIgnoredAttributes().Select(f => MetadataName.Parse(f)).ToImmutableArray());

        public DefaultIgnoredAttributeNameFilterRule(IEnumerable<MetadataName> values)
            : base(values)
        {
        }

        public override bool IsMatch(AttributeInfo attributeInfo)
        {
            INamedTypeSymbol? attributeClass = attributeInfo.AttributeClass;

            if (attributeClass is null)
                return true;

            if (AttributeNames.Contains(attributeClass))
                return false;

            ISymbol symbol = attributeInfo.Target;

            if (symbol.IsKind(SymbolKind.NamedType)
                && attributeClass.HasMetadataName(_defaultMemberAttribute))
            {
                var namedType = (INamedTypeSymbol)symbol;

                if (namedType.GetMembers().Any(f => f.IsKind(SymbolKind.Property) && ((IPropertySymbol)f).IsIndexer))
                    return false;
            }

            return true;
        }

        private static string[] GetIgnoredAttributes()
        {
            return new[]
            {
                "System.Diagnostics.CodeAnalysis.SuppressMessageAttribute",
                "System.Diagnostics.ConditionalAttribute",
                "System.Diagnostics.DebuggableAttribute",
                "System.Diagnostics.DebuggerBrowsableAttribute",
                "System.Diagnostics.DebuggerDisplayAttribute",
                "System.Diagnostics.DebuggerHiddenAttribute",
                "System.Diagnostics.DebuggerNonUserCodeAttribute",
                "System.Diagnostics.DebuggerStepperBoundaryAttribute",
                "System.Diagnostics.DebuggerStepThroughAttribute",
                "System.Diagnostics.DebuggerTypeProxyAttribute",
                "System.Diagnostics.DebuggerVisualizerAttribute",
                "System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute",
                "System.Runtime.CompilerServices.AsyncStateMachineAttribute",
                "System.Runtime.CompilerServices.CompilationRelaxationsAttribute",
                "System.Runtime.CompilerServices.CompilerGeneratedAttribute",
                "System.Runtime.CompilerServices.IsReadOnlyAttribute",
                "System.Runtime.CompilerServices.IteratorStateMachineAttribute",
                "System.Runtime.CompilerServices.MethodImplAttribute",
                "System.Runtime.CompilerServices.StateMachineAttribute",
                "System.Runtime.CompilerServices.TupleElementNamesAttribute",
                "System.Runtime.CompilerServices.TypeForwardedFromAttribute",
                "System.Runtime.CompilerServices.TypeForwardedToAttribute",
            };
        }
    }
}
