// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.FindSymbols
{
    internal class SymbolFilterOptions
    {
        public static SymbolFilterOptions Default { get; } = new SymbolFilterOptions();

        internal SymbolFilterOptions(
            VisibilityFilter visibility = VisibilityFilter.All,
            SymbolGroupFilter symbolGroups = SymbolGroupFilter.TypeOrMember,
            IEnumerable<SymbolFilterRule> rules = null,
            IEnumerable<AttributeFilterRule> attributeRules = null)
        {
            Visibility = visibility;
            SymbolGroups = symbolGroups;

            Rules = rules?.ToImmutableArray() ?? ImmutableArray<SymbolFilterRule>.Empty;
            AttributeRules = attributeRules?.ToImmutableArray() ?? ImmutableArray<AttributeFilterRule>.Empty;
        }

        public VisibilityFilter Visibility { get; }

        public SymbolGroupFilter SymbolGroups { get; }

        public ImmutableArray<SymbolFilterRule> Rules { get; }

        public ImmutableArray<AttributeFilterRule> AttributeRules { get; }

        public bool Includes(SymbolGroupFilter symbolGroupFilter)
        {
            return (SymbolGroups & symbolGroupFilter) == symbolGroupFilter;
        }

        public bool IsMatch(ISymbol symbol)
        {
            return GetReason(symbol) == SymbolFilterReason.None;
        }

        public bool IsMatch(INamespaceSymbol namespaceSymbol)
        {
            return GetReason(namespaceSymbol) == SymbolFilterReason.None;
        }

        public bool IsMatch(INamedTypeSymbol typeSymbol)
        {
            return GetReason(typeSymbol) == SymbolFilterReason.None;
        }

        public bool IsMatch(IEventSymbol symbol)
        {
            return GetReason(symbol) == SymbolFilterReason.None;
        }

        public bool IsMatch(IFieldSymbol symbol)
        {
            return GetReason(symbol) == SymbolFilterReason.None;
        }

        public bool IsMatch(IPropertySymbol symbol)
        {
            return GetReason(symbol) == SymbolFilterReason.None;
        }

        public bool IsMatch(IMethodSymbol symbol)
        {
            return GetReason(symbol) == SymbolFilterReason.None;
        }

        public bool IsMatch(ISymbol symbol, AttributeData attribute)
        {
            return GetReason(symbol, attribute) == SymbolFilterReason.None;
        }

        public SymbolFilterReason GetReason(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Namespace:
                    return GetReason((INamespaceSymbol)symbol);
                case SymbolKind.NamedType:
                    return GetReason((INamedTypeSymbol)symbol);
                case SymbolKind.Event:
                    return GetReason((IEventSymbol)symbol);
                case SymbolKind.Field:
                    return GetReason((IFieldSymbol)symbol);
                case SymbolKind.Property:
                    return GetReason((IPropertySymbol)symbol);
                case SymbolKind.Method:
                    return GetReason((IMethodSymbol)symbol);
                default:
                    throw new ArgumentException("", nameof(symbol));
            }
        }

        public virtual SymbolFilterReason GetReason(INamespaceSymbol namespaceSymbol)
        {
            return GetRulesReason(namespaceSymbol);
        }

        public virtual SymbolFilterReason GetReason(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.IsImplicitlyDeclared)
                return SymbolFilterReason.ImplicitlyDeclared;

            if (!Includes(typeSymbol.TypeKind.ToSymbolGroupFilter()))
                return SymbolFilterReason.SymbolGroup;

            if (!typeSymbol.IsVisible(Visibility))
                return SymbolFilterReason.Visibility;

            return GetRulesReason(typeSymbol);
        }

        public virtual SymbolFilterReason GetReason(IEventSymbol symbol)
        {
            if (symbol.IsImplicitlyDeclared)
                return SymbolFilterReason.ImplicitlyDeclared;

            if (!Includes(SymbolGroupFilter.Event))
                return SymbolFilterReason.SymbolGroup;

            if (!symbol.IsVisible(Visibility))
                return SymbolFilterReason.Visibility;

            return GetRulesReason(symbol);
        }

        public virtual SymbolFilterReason GetReason(IFieldSymbol symbol)
        {
            if (symbol.IsImplicitlyDeclared)
                return SymbolFilterReason.ImplicitlyDeclared;

            var group = SymbolGroupFilter.None;

            if (symbol.IsConst)
            {
                group = (symbol.ContainingType.TypeKind == TypeKind.Enum) ? SymbolGroupFilter.EnumField : SymbolGroupFilter.Const;
            }
            else
            {
                group = SymbolGroupFilter.Field;
            }

            if (!Includes(group))
                return SymbolFilterReason.SymbolGroup;

            if (!symbol.IsVisible(Visibility))
                return SymbolFilterReason.Visibility;

            return GetRulesReason(symbol);
        }

        public virtual SymbolFilterReason GetReason(IPropertySymbol symbol)
        {
            if (symbol.IsImplicitlyDeclared)
                return SymbolFilterReason.ImplicitlyDeclared;

            if (!Includes((symbol.IsIndexer) ? SymbolGroupFilter.Indexer : SymbolGroupFilter.Property))
                return SymbolFilterReason.SymbolGroup;

            if (!symbol.IsVisible(Visibility))
                return SymbolFilterReason.Visibility;

            return GetRulesReason(symbol);
        }

        public virtual SymbolFilterReason GetReason(IMethodSymbol symbol)
        {
            bool canBeImplicitlyDeclared = false;

            if (!Includes(SymbolGroupFilter.Method))
                return SymbolFilterReason.SymbolGroup;

            switch (symbol.MethodKind)
            {
                case MethodKind.Constructor:
                    {
                        TypeKind typeKind = symbol.ContainingType.TypeKind;

                        Debug.Assert(typeKind.Is(TypeKind.Class, TypeKind.Struct, TypeKind.Enum), symbol.ToDisplayString(SymbolDisplayFormats.Test));

                        if (typeKind == TypeKind.Class
                            && !symbol.Parameters.Any())
                        {
                            canBeImplicitlyDeclared = true;
                        }

                        break;
                    }
                case MethodKind.Conversion:
                case MethodKind.UserDefinedOperator:
                case MethodKind.Ordinary:
                case MethodKind.StaticConstructor:
                case MethodKind.Destructor:
                case MethodKind.ExplicitInterfaceImplementation:
                    {
                        break;
                    }
                default:
                    {
                        return SymbolFilterReason.Other;
                    }
            }

            if (!canBeImplicitlyDeclared && symbol.IsImplicitlyDeclared)
                return SymbolFilterReason.ImplicitlyDeclared;

            if (!symbol.IsVisible(Visibility))
                return SymbolFilterReason.Visibility;

            return GetRulesReason(symbol);
        }

        private SymbolFilterReason GetRulesReason(ISymbol symbol)
        {
            foreach (SymbolFilterRule rule in Rules)
            {
                if (rule.IsApplicable(symbol)
                    && !rule.IsMatch(symbol))
                {
                    return rule.Reason;
                }
            }

            return SymbolFilterReason.None;
        }

        public virtual SymbolFilterReason GetReason(ISymbol symbol, AttributeData attribute)
        {
            foreach (AttributeFilterRule rule in AttributeRules)
            {
                if (rule.IsApplicable(attribute)
                    && !rule.IsMatch(attribute))
                {
                    return rule.Reason;
                }
            }

            return SymbolFilterReason.None;
        }
    }
}
