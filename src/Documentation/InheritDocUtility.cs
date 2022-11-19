// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation;

internal static class InheritDocUtility
{
    public static XElement FindInheritedDocumentation(ISymbol symbol, Func<ISymbol, XElement> getDocumentation)
    {
        if (symbol.IsKind(SymbolKind.Method, SymbolKind.Property, SymbolKind.Event))
        {
            if (symbol is IMethodSymbol methodSymbol
                && methodSymbol.MethodKind == MethodKind.Constructor)
            {
                return FindInheritedDocumentationFromBaseConstructor(methodSymbol, getDocumentation);
            }

            return FindInheritedDocumentationFromBaseMember(symbol, getDocumentation)
                ?? FindInheritedDocumentationFromImplementedInterfaceMember(symbol, getDocumentation);
        }
        else if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            return FindInheritedDocumentationFromBaseType(namedTypeSymbol, getDocumentation)
                ?? FindInheritedDocumentationFromImplementedInterface(namedTypeSymbol, getDocumentation);
        }

        return null;
    }

    private static XElement FindInheritedDocumentationFromBaseConstructor(IMethodSymbol symbol, Func<ISymbol, XElement> getDocumentation)
    {
        foreach (INamedTypeSymbol baseType in symbol.ContainingType.BaseTypes())
        {
            foreach (IMethodSymbol baseConstructor in baseType.Constructors)
            {
                if (ParametersEqual(symbol, baseConstructor))
                {
                    XElement element = getDocumentation(baseConstructor);

                    return (ContainsInheritDoc(element))
                        ? FindInheritedDocumentationFromBaseConstructor(baseConstructor, getDocumentation)
                        : element;
                }
            }
        }

        return null;

        static bool ParametersEqual(IMethodSymbol x, IMethodSymbol y)
        {
            ImmutableArray<IParameterSymbol> parameters1 = x.Parameters;
            ImmutableArray<IParameterSymbol> parameters2 = y.Parameters;

            if (parameters1.Length != parameters2.Length)
                return false;

            for (int i = 0; i < parameters1.Length; i++)
            {
                if (!SymbolEqualityComparer.Default.Equals(parameters1[i].Type, parameters2[i].Type))
                    return false;
            }

            return true;
        }
    }

    private static XElement FindInheritedDocumentationFromBaseMember(ISymbol symbol, Func<ISymbol, XElement> getDocumentation)
    {
        ISymbol s = symbol;

        while ((s = s.OverriddenSymbol()) is not null)
        {
            XElement element = getDocumentation(s);

            if (!ContainsInheritDoc(element))
                return element;
        }

        return null;
    }

    private static XElement FindInheritedDocumentationFromImplementedInterfaceMember(ISymbol symbol, Func<ISymbol, XElement> getDocumentation)
    {
        INamedTypeSymbol containingType = symbol.ContainingType;

        if (containingType is not null)
        {
            foreach (INamedTypeSymbol interfaceSymbol in containingType.Interfaces)
            {
                foreach (ISymbol memberSymbol in interfaceSymbol.GetMembers(symbol.Name))
                {
                    if (SymbolEqualityComparer.Default.Equals(symbol, containingType.FindImplementationForInterfaceMember(memberSymbol)))
                    {
                        XElement element = getDocumentation(memberSymbol);

                        return (ContainsInheritDoc(element))
                            ? FindInheritedDocumentationFromImplementedInterfaceMember(memberSymbol, getDocumentation)
                            : element;
                    }
                }
            }
        }

        return null;
    }

    private static XElement FindInheritedDocumentationFromBaseType(INamedTypeSymbol namedTypeSymbol, Func<ISymbol, XElement> getDocumentation)
    {
        foreach (INamedTypeSymbol baseType in namedTypeSymbol.BaseTypes())
        {
            XElement element = getDocumentation(baseType);

            if (!ContainsInheritDoc(element))
                return element;
        }

        return null;
    }

    private static XElement FindInheritedDocumentationFromImplementedInterface(INamedTypeSymbol namedTypeSymbol, Func<ISymbol, XElement> getDocumentation)
    {
        foreach (INamedTypeSymbol interfaceSymbol in namedTypeSymbol.Interfaces)
        {
            XElement element = getDocumentation(interfaceSymbol);

            return (ContainsInheritDoc(element))
                ? FindInheritedDocumentationFromImplementedInterface(interfaceSymbol, getDocumentation)
                : element;
        }

        return null;
    }

    public static bool ContainsInheritDoc(XElement element)
    {
        if (element is not null)
        {
            using (IEnumerator<XElement> en = element.Elements().GetEnumerator())
            {
                if (en.MoveNext())
                {
                    XElement e = en.Current;

                    if (!en.MoveNext()
                        && string.Equals(e.Name.LocalName, "inheritdoc", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
