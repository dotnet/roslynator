// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.FindSymbols;

internal static class SymbolFilterRuleFactory
{
    public static IEnumerable<SymbolFilterRule> FromModifiers(SymbolModifier modifiers, bool invert = false)
    {
        if ((modifiers & SymbolModifier.Const) != 0)
            yield return (invert) ? IsNotConst : IsConst;

        if ((modifiers & SymbolModifier.Static) != 0)
            yield return (invert) ? IsNotStatic : IsStatic;

        if ((modifiers & SymbolModifier.Virtual) != 0)
            yield return (invert) ? IsNotVirtual : IsVirtual;

        if ((modifiers & SymbolModifier.Sealed) != 0)
            yield return (invert) ? IsNotSealed : IsSealed;

        if ((modifiers & SymbolModifier.Override) != 0)
            yield return (invert) ? IsNotOverride : IsOverride;

        if ((modifiers & SymbolModifier.Abstract) != 0)
            yield return (invert) ? IsNotAbstract : IsAbstract;

        if ((modifiers & SymbolModifier.ReadOnly) != 0)
            yield return (invert) ? IsNotReadOnly : IsReadOnly;

        if ((modifiers & SymbolModifier.Async) != 0)
            yield return (invert) ? IsNotAsync : IsAsync;
    }

    public static PredicateSymbolFilterRule IsConst { get; } = new(f => ((IFieldSymbol)f).IsConst, f => f.IsKind(SymbolKind.Field), SymbolFilterReason.Other);
    public static PredicateSymbolFilterRule IsNotConst { get; } = IsConst.Invert();

    public static PredicateSymbolFilterRule IsStatic { get; } = new(f => f.IsStatic, f => !f.IsKind(SymbolKind.Namespace), SymbolFilterReason.Other);
    public static PredicateSymbolFilterRule IsNotStatic { get; } = IsStatic.Invert();

    public static PredicateSymbolFilterRule IsVirtual { get; } = new(f => f.IsVirtual, f => f.IsKind(SymbolKind.Event, SymbolKind.Method, SymbolKind.Property), SymbolFilterReason.Other);
    public static PredicateSymbolFilterRule IsNotVirtual { get; } = IsVirtual.Invert();

    public static PredicateSymbolFilterRule IsSealed { get; } = new(f => f.IsSealed, f => !f.IsKind(SymbolKind.Namespace), SymbolFilterReason.Other);
    public static PredicateSymbolFilterRule IsNotSealed { get; } = IsSealed.Invert();

    public static PredicateSymbolFilterRule IsOverride { get; } = new(f => f.IsOverride, f => f.IsKind(SymbolKind.Event, SymbolKind.Method, SymbolKind.Property), SymbolFilterReason.Other);
    public static PredicateSymbolFilterRule IsNotOverride { get; } = IsOverride.Invert();

    public static PredicateSymbolFilterRule IsAbstract { get; } = new(f => f.IsAbstract, f => !f.IsKind(SymbolKind.Namespace), SymbolFilterReason.Other);
    public static PredicateSymbolFilterRule IsNotAbstract { get; } = IsAbstract.Invert();

    public static PredicateSymbolFilterRule IsReadOnly { get; } = new(
        symbol =>
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

    public static PredicateSymbolFilterRule IsAsync { get; } = new(f => ((IMethodSymbol)f).IsAsync, f => f.IsKind(SymbolKind.Method), SymbolFilterReason.Other);
    public static PredicateSymbolFilterRule IsNotAsync { get; } = IsAsync.Invert();
}
