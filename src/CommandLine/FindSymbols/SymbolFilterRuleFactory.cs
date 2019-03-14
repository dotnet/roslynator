// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.FindSymbols
{
    internal static class SymbolFilterRuleFactory
    {
        public static IEnumerable<SymbolFilterRule> FromFlags(SymbolFlags flags, bool invert = false)
        {
            if ((flags & SymbolFlags.Const) != 0)
                yield return (invert) ? IsNotConst : IsConst;

            if ((flags & SymbolFlags.Static) != 0)
                yield return (invert) ? IsNotStatic : IsStatic;

            if ((flags & SymbolFlags.Virtual) != 0)
                yield return (invert) ? IsNotVirtual : IsVirtual;

            if ((flags & SymbolFlags.Sealed) != 0)
                yield return (invert) ? IsNotSealed : IsSealed;

            if ((flags & SymbolFlags.Override) != 0)
                yield return (invert) ? IsNotOverride : IsOverride;

            if ((flags & SymbolFlags.Abstract) != 0)
                yield return (invert) ? IsNotAbstract : IsAbstract;

            if ((flags & SymbolFlags.ReadOnly) != 0)
                yield return (invert) ? IsNotReadOnly : IsReadOnly;

            if ((flags & SymbolFlags.Extern) != 0)
                yield return (invert) ? IsNotExtern : IsExtern;

            if ((flags & SymbolFlags.Async) != 0)
                yield return (invert) ? IsNotAsync : IsAsync;

            if ((flags & SymbolFlags.Extension) != 0)
                yield return (invert) ? IsNotExtension : IsExtension;
        }

        public static PredicateSymbolFilterRule IsConst { get; } = new PredicateSymbolFilterRule(f => ((IFieldSymbol)f).IsConst, f => f.IsKind(SymbolKind.Field), SymbolFilterReason.Other);
        public static PredicateSymbolFilterRule IsNotConst { get; } = IsConst.Invert();

        public static PredicateSymbolFilterRule IsStatic { get; } = new PredicateSymbolFilterRule(f => f.IsStatic, f => !f.IsKind(SymbolKind.Namespace), SymbolFilterReason.Other);
        public static PredicateSymbolFilterRule IsNotStatic { get; } = IsStatic.Invert();

        public static PredicateSymbolFilterRule IsVirtual { get; } = new PredicateSymbolFilterRule(f => f.IsVirtual, f => f.IsKind(SymbolKind.Event, SymbolKind.Method, SymbolKind.Property), SymbolFilterReason.Other);
        public static PredicateSymbolFilterRule IsNotVirtual { get; } = IsVirtual.Invert();

        public static PredicateSymbolFilterRule IsSealed { get; } = new PredicateSymbolFilterRule(f => f.IsSealed, f => !f.IsKind(SymbolKind.Namespace), SymbolFilterReason.Other);
        public static PredicateSymbolFilterRule IsNotSealed { get; } = IsSealed.Invert();

        public static PredicateSymbolFilterRule IsOverride { get; } = new PredicateSymbolFilterRule(f => f.IsOverride, f => f.IsKind(SymbolKind.Event, SymbolKind.Method, SymbolKind.Property), SymbolFilterReason.Other);
        public static PredicateSymbolFilterRule IsNotOverride { get; } = IsOverride.Invert();

        public static PredicateSymbolFilterRule IsAbstract { get; } = new PredicateSymbolFilterRule(f => f.IsAbstract, f => !f.IsKind(SymbolKind.Namespace), SymbolFilterReason.Other);
        public static PredicateSymbolFilterRule IsNotAbstract { get; } = IsAbstract.Invert();

        public static PredicateSymbolFilterRule IsReadOnly { get; } = new PredicateSymbolFilterRule(symbol =>
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Field:
                    return ((IFieldSymbol)symbol).IsReadOnly;
                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).IsReadOnly;
                default:
                    return false;
            }
        },
        f => f.IsKind(SymbolKind.Field, SymbolKind.Property),
        SymbolFilterReason.Other);

        public static PredicateSymbolFilterRule IsNotReadOnly { get; } = IsReadOnly.Invert();

        public static PredicateSymbolFilterRule IsExtern { get; } = new PredicateSymbolFilterRule(f => f.IsExtern, f => f.IsKind(SymbolKind.Event, SymbolKind.Method, SymbolKind.Property), SymbolFilterReason.Other);
        public static PredicateSymbolFilterRule IsNotExtern { get; } = IsExtern.Invert();

        public static PredicateSymbolFilterRule IsAsync { get; } = new PredicateSymbolFilterRule(f => ((IMethodSymbol)f).IsAsync, f => f.IsKind(SymbolKind.Method), SymbolFilterReason.Other);
        public static PredicateSymbolFilterRule IsNotAsync { get; } = IsAsync.Invert();

        public static PredicateSymbolFilterRule IsExtension { get; } = new PredicateSymbolFilterRule(f => ((IMethodSymbol)f).IsExtensionMethod, f => f.IsKind(SymbolKind.Method), SymbolFilterReason.Other);
        public static PredicateSymbolFilterRule IsNotExtension { get; } = IsExtension.Invert();
    }
}
