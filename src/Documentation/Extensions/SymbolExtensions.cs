﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation;

internal static class SymbolExtensions
{
    internal static IEnumerable<INamespaceSymbol> GetContainingNamespaces(this ISymbol symbol)
    {
        INamespaceSymbol n = symbol.ContainingNamespace;

        while (n?.IsGlobalNamespace == false)
        {
            yield return n;

            n = n.ContainingNamespace;
        }
    }

    internal static IEnumerable<INamespaceSymbol> GetContainingNamespacesAndSelf(this INamespaceSymbol namespaceSymbol)
    {
        do
        {
            yield return namespaceSymbol;

            namespaceSymbol = namespaceSymbol.ContainingNamespace;
        }
        while (namespaceSymbol?.IsGlobalNamespace == false);
    }

    public static ImmutableArray<ISymbol> GetMembers(this INamedTypeSymbol typeSymbol, Func<ISymbol, bool> predicate, bool includeInherited = false)
    {
        if (includeInherited)
        {
            if (typeSymbol.TypeKind == TypeKind.Interface)
            {
                return GetInterfaceMembersIncludingInherited();
            }
            else
            {
                return GetMembersIncludingInherited();
            }
        }
        else if (predicate is not null)
        {
            return typeSymbol
                .GetMembers()
                .Where(predicate)
                .ToImmutableArray();
        }

        return typeSymbol.GetMembers();

        ImmutableArray<ISymbol> GetMembersIncludingInherited()
        {
            var symbols = new HashSet<ISymbol>(MemberSymbolEqualityComparer.Instance);

            HashSet<ISymbol> overriddenSymbols = null;

            foreach (ISymbol symbol in GetMembers(typeSymbol, predicate: predicate))
            {
                ISymbol overriddenSymbol = symbol.OverriddenSymbol();

                if (overriddenSymbol is not null)
                {
                    (overriddenSymbols ??= new HashSet<ISymbol>()).Add(overriddenSymbol);
                }

                symbols.Add(symbol);
            }

            INamedTypeSymbol baseType = typeSymbol.BaseType;

            while (baseType is not null)
            {
                bool areInternalsVisible = typeSymbol.ContainingAssembly.Identity.Equals(baseType.ContainingAssembly.Identity)
                    || baseType.ContainingAssembly.GivesAccessTo(typeSymbol.ContainingAssembly);

                foreach (ISymbol symbol in baseType.GetMembers())
                {
                    if (!symbol.IsStatic
                        && (predicate is null || predicate(symbol))
                        && (symbol.DeclaredAccessibility != Accessibility.Internal || areInternalsVisible))
                    {
                        if (overriddenSymbols?.Remove(symbol) != true)
                            symbols.Add(symbol);

                        ISymbol overriddenSymbol = symbol.OverriddenSymbol();

                        if (overriddenSymbol is not null)
                        {
                            (overriddenSymbols ??= new HashSet<ISymbol>()).Add(overriddenSymbol);
                        }
                    }
                }

                baseType = baseType.BaseType;
            }

            return symbols.ToImmutableArray();
        }

        ImmutableArray<ISymbol> GetInterfaceMembersIncludingInherited()
        {
            var symbols = new HashSet<ISymbol>(MemberSymbolEqualityComparer.Instance);

            foreach (ISymbol symbol in GetMembers(typeSymbol, predicate: predicate))
                symbols.Add(symbol);

            foreach (INamedTypeSymbol interfaceSymbol in typeSymbol.AllInterfaces)
            {
                foreach (ISymbol symbol in GetMembers(interfaceSymbol, predicate: predicate))
                    symbols.Add(symbol);
            }

            return symbols.ToImmutableArray();
        }
    }

    public static int GetArity(this ISymbol symbol)
    {
        switch (symbol.Kind)
        {
            case SymbolKind.Method:
                return ((IMethodSymbol)symbol).Arity;
            case SymbolKind.NamedType:
                return ((INamedTypeSymbol)symbol).Arity;
        }

        return 0;
    }

    public static ImmutableArray<ITypeParameterSymbol> GetTypeParameters(this ISymbol symbol)
    {
        switch (symbol.Kind)
        {
            case SymbolKind.Method:
                return ((IMethodSymbol)symbol).TypeParameters;
            case SymbolKind.NamedType:
                return ((INamedTypeSymbol)symbol).TypeParameters;
        }

        return ImmutableArray<ITypeParameterSymbol>.Empty;
    }

    public static ISymbol GetFirstExplicitInterfaceImplementation(this ISymbol symbol)
    {
        switch (symbol.Kind)
        {
            case SymbolKind.Event:
                return ((IEventSymbol)symbol).ExplicitInterfaceImplementations.FirstOrDefault();
            case SymbolKind.Method:
                return ((IMethodSymbol)symbol).ExplicitInterfaceImplementations.FirstOrDefault();
            case SymbolKind.Property:
                return ((IPropertySymbol)symbol).ExplicitInterfaceImplementations.FirstOrDefault();
        }

        return null;
    }

    public static string ToDisplayString(this ISymbol symbol, SymbolDisplayFormat format, SymbolDisplayAdditionalMemberOptions additionalOptions)
    {
        return symbol.ToDisplayParts(format, additionalOptions).ToDisplayString();
    }

    public static ImmutableArray<SymbolDisplayPart> ToDisplayParts(this ISymbol symbol, SymbolDisplayFormat format, SymbolDisplayAdditionalMemberOptions additionalOptions)
    {
        if (additionalOptions == SymbolDisplayAdditionalMemberOptions.None)
            return symbol.ToDisplayParts(format);

        ImmutableArray<SymbolDisplayPart> parts = symbol.ToDisplayParts(format);
        int length = parts.Length;

        for (int i = 0; i < length; i++)
        {
            SymbolDisplayPart part = parts[i];

            switch (part.Kind)
            {
                case SymbolDisplayPartKind.Keyword:
                {
                    switch (part.ToString())
                    {
                        case "this":
                        {
                            if ((additionalOptions & SymbolDisplayAdditionalMemberOptions.UseItemPropertyName) != 0
                                && symbol is IPropertySymbol { IsIndexer: true })
                            {
                                parts = parts.Replace(part, SymbolDisplayPartFactory.PropertyName("Item", part.Symbol));
                            }

                            break;
                        }
                        case "operator":
                        {
                            if ((additionalOptions & SymbolDisplayAdditionalMemberOptions.UseOperatorName) != 0
                                && symbol is IMethodSymbol methodSymbol
                                && methodSymbol.MethodKind == MethodKind.UserDefinedOperator)
                            {
                                string name = methodSymbol.Name;

                                Debug.Assert(name.StartsWith("op_", StringComparison.Ordinal), name);

                                if (name.StartsWith("op_", StringComparison.Ordinal)
                                    && i < length - 2
                                    && parts[i + 1].IsSpace()
                                    && parts[i + 2].Kind == SymbolDisplayPartKind.Operator
                                    && parts[i + 2].Symbol is IMethodSymbol { MethodKind: MethodKind.UserDefinedOperator })
                                {
                                    parts = parts.Replace(parts[i + 2], SymbolDisplayPartFactory.MethodName(name.Substring(3), parts[i + 2].Symbol));
                                    parts = parts.RemoveRange(i, 2);
                                    length -= 2;
                                }
                            }

                            break;
                        }
                        case "implicit":
                        case "explicit":
                        {
                            if ((additionalOptions & SymbolDisplayAdditionalMemberOptions.UseOperatorName) != 0
                                && symbol is IMethodSymbol methodSymbol
                                && methodSymbol.MethodKind == MethodKind.Conversion)
                            {
                                string name = methodSymbol.Name;

                                Debug.Assert(name.StartsWith("op_", StringComparison.Ordinal), name);

                                if (name.StartsWith("op_", StringComparison.Ordinal)
                                    && i < length - 2
                                    && parts[i + 1].IsSpace()
                                    && parts[i + 2].IsKeyword("operator"))
                                {
                                    List<SymbolDisplayPart> list = parts.ToList();

                                    list[i + 2] = SymbolDisplayPartFactory.MethodName(name.Substring(3), list[i + 4].Symbol);
                                    list.RemoveRange(i, 2);
                                    length -= 2;

                                    if (i == length - 3
                                        && list[i + 1].IsSpace()
                                        && list[i + 2].IsName())
                                    {
                                        list.RemoveRange(i + 1, 2);
                                        length -= 2;
                                    }
                                    else if (i < length - 5
                                        && list[i + 1].IsSpace()
                                        && list[i + 2].IsName()
                                        && list[i + 3].IsPunctuation()
                                        && list[i + 4].IsName()
                                        && list[i + 5].IsPunctuation())
                                    {
                                        list.Insert(i + 5, list[i + 2]);
                                        list.Insert(i + 5, SymbolDisplayPartFactory.Text(" to "));
                                        list.RemoveRange(i + 1, 2);
                                        length -= 5;
                                    }

                                    parts = list.ToImmutableArray();
                                }
                            }

                            break;
                        }
                    }

                    break;
                }
            }
        }

        return parts;
    }

    internal static ImmutableArray<AttributeInfo> GetAttributesIncludingInherited(this INamedTypeSymbol namedType, Func<ISymbol, AttributeData, bool> predicate = null)
    {
        HashSet<AttributeInfo> attributes = null;

        foreach (AttributeData attributeData in namedType.GetAttributes())
        {
            if (predicate is null
                || predicate(namedType, attributeData))
            {
                (attributes ??= new HashSet<AttributeInfo>(AttributeInfo.AttributeClassComparer)).Add(new AttributeInfo(namedType, attributeData));
            }
        }

        INamedTypeSymbol baseType = namedType.BaseType;

        while (baseType is not null
            && baseType.SpecialType != SpecialType.System_Object)
        {
            foreach (AttributeData attributeData in baseType.GetAttributes())
            {
                AttributeData attributeUsage = attributeData.AttributeClass.GetAttribute(MetadataNames.System_AttributeUsageAttribute);

                if (attributeUsage is not null)
                {
                    TypedConstant typedConstant = attributeUsage.NamedArguments.FirstOrDefault(f => f.Key == "Inherited").Value;

                    if (typedConstant.Kind != TypedConstantKind.Error
                        && typedConstant.Type?.SpecialType == SpecialType.System_Boolean
                        && (!(bool)typedConstant.Value))
                    {
                        continue;
                    }
                }

                if (predicate is null
                    || predicate(baseType, attributeData))
                {
                    (attributes ??= new HashSet<AttributeInfo>(AttributeInfo.AttributeClassComparer)).Add(new AttributeInfo(baseType, attributeData));
                }
            }

            baseType = baseType.BaseType;
        }

        return (attributes is not null)
            ? attributes.ToImmutableArray()
            : ImmutableArray<AttributeInfo>.Empty;
    }

    public static IEnumerable<ISymbol> GetExplicitImplementations(this INamedTypeSymbol typeSymbol, bool includeAccessors = false)
    {
        if (!typeSymbol.TypeKind.Is(TypeKind.Delegate, TypeKind.Enum))
        {
            foreach (ISymbol member in typeSymbol.GetMembers())
            {
                if (IsExplicitImplementation(member, includeAccessors))
                    yield return member;
            }
        }
    }

    public static bool IsExplicitImplementation(this ISymbol symbol, bool includeAccessors = false)
    {
        switch (symbol.Kind)
        {
            case SymbolKind.Event:
            {
                var eventSymbol = (IEventSymbol)symbol;

                return !eventSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty;
            }
            case SymbolKind.Method:
            {
                var methodSymbol = (IMethodSymbol)symbol;

                if (methodSymbol.MethodKind != MethodKind.ExplicitInterfaceImplementation)
                    return false;

                ImmutableArray<IMethodSymbol> explicitInterfaceImplementations = methodSymbol.ExplicitInterfaceImplementations;

                if (explicitInterfaceImplementations.IsDefaultOrEmpty)
                    return false;

                if (!includeAccessors)
                {
                    if (methodSymbol.MetadataName.EndsWith(".get_Item", StringComparison.Ordinal))
                    {
                        if (explicitInterfaceImplementations[0].MethodKind == MethodKind.PropertyGet)
                            return false;
                    }
                    else if (methodSymbol.MetadataName.EndsWith(".set_Item", StringComparison.Ordinal))
                    {
                        if (explicitInterfaceImplementations[0].MethodKind == MethodKind.PropertySet)
                            return false;
                    }
                }

                return true;
            }
            case SymbolKind.Property:
            {
                var propertySymbol = (IPropertySymbol)symbol;

                return !propertySymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty;
            }
        }

        return false;
    }
}
