// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Roslynator.Text;

namespace Roslynator.Documentation;

internal static class DocumentationUtility
{
    public static string GetSymbolLabel(ISymbol symbol, DocumentationContext context)
    {
        if (symbol.IsKind(SymbolKind.Namespace))
        {
            switch (context.Options.FilesLayout)
            {
                case FilesLayout.FlatNamespaces:
                    {
                        string label = symbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces_GlobalNamespace_OmittedAsContaining);

                        if (context.CommonNamespaces.Count > 0
                            && !context.CommonNamespaces.Contains((INamespaceSymbol)symbol, MetadataNameEqualityComparer<INamespaceSymbol>.Instance))
                        {
                            (INamespaceSymbol symbol, string displayString) commonNamespace = default;

                            foreach ((INamespaceSymbol _, string displayString) cn in context.CommonNamespacesAsText)
                            {
                                if (label.StartsWith(cn.displayString)
                                    && label[cn.displayString.Length] == '.')
                                {
                                    Debug.Assert(commonNamespace == default);
                                    commonNamespace = cn;
                                }
                            }

                            Debug.Assert(commonNamespace != default);

                            if (commonNamespace != default)
                                label = label.Substring(commonNamespace.displayString.Length + 1);
                        }

                        return label;
                    }
                case FilesLayout.Hierarchical:
                    {
                        if (context.CommonNamespaces.Contains((INamespaceSymbol)symbol, MetadataNameEqualityComparer<INamespaceSymbol>.Instance))
                            return symbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces_GlobalNamespace_OmittedAsContaining);

                        return symbol.Name;
                    }
                default:
                    {
                        throw new InvalidOperationException($"Unknown value '{context.Options.FilesLayout}'.");
                    }
            }
        }
        else if (symbol.IsKind(SymbolKind.NamedType))
        {
            string label = symbol.ToDisplayString(TypeSymbolDisplayFormats.Name_TypeParameters);

            if (symbol.ContainingType is not null)
            {
                label = symbol.ContainingType.ToDisplayString(TypeSymbolDisplayFormats.Name_TypeParameters)
                    + "."
                    + label;
            }

            return label;
        }
        else
        {
            string label = symbol.Name;

            if (symbol is IMethodSymbol methodSymbol)
            {
                if (methodSymbol.MethodKind == MethodKind.Constructor)
                {
                    label = symbol.ContainingType.ToDisplayString(TypeSymbolDisplayFormats.Name_TypeParameters);
                }
            }
            else if (symbol.Kind == SymbolKind.Property
                && ((IPropertySymbol)symbol).IsIndexer)
            {
                label = "Item[]";
            }

            ISymbol explicitImplementation = symbol.GetFirstExplicitInterfaceImplementation();

            if (explicitImplementation is not null)
            {
                label = explicitImplementation.ContainingType.ToDisplayString(TypeSymbolDisplayFormats.Name_TypeParameters)
                    + "."
                    + label;
            }

            return label;
        }
    }

    public static bool ShouldGenerateNamespaceFile(INamespaceSymbol namespaceSymbol, IEnumerable<INamespaceSymbol> commonNamespaces)
    {
        foreach (INamespaceSymbol commonNamespace in commonNamespaces)
        {
            foreach (INamespaceSymbol containingSymbol in commonNamespace.GetContainingNamespaces())
            {
                if (MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(namespaceSymbol, containingSymbol))
                    return false;
            }
        }

        return true;
    }

    public static List<INamespaceSymbol> FindCommonNamespaces(IEnumerable<ITypeSymbol> symbols)
    {
        List<(INamespaceSymbol symbol, string text)> namespaces = symbols
            .Select(f => f.ContainingNamespace)
            .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
            .Select(f => (f, f.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces_GlobalNamespace)))
            .ToList();

        for (int i = namespaces.Count - 1; i >= 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                string n1 = namespaces[i].text;
                string n2 = namespaces[j].text;

                if (n1 == n2)
                {
                    namespaces.RemoveAt(j);
                }
                else if (n2.StartsWith(n1)
                    && n2[n1.Length] == '.')
                {
                    namespaces.RemoveAt(j);
                }
                else if (n1.StartsWith(n2)
                    && n1[n2.Length] == '.')
                {
                    namespaces.RemoveAt(i);
                    break;
                }
            }
        }

        return namespaces.ConvertAll(f => f.symbol);
    }

    public static string CreateLocalLink(ISymbol symbol, string prefix = null)
    {
        StringBuilder sb = StringBuilderCache.GetInstance();

        if (prefix is not null)
            sb.Append(prefix);

        int count = 0;

        INamespaceSymbol n = symbol.ContainingNamespace;

        while (n?.IsGlobalNamespace == false)
        {
            n = n.ContainingNamespace;
            count++;
        }

        while (count > 0)
        {
            int c = count;

            n = symbol.ContainingNamespace;

            while (c > 1)
            {
                n = n.ContainingNamespace;
                c--;
            }

            sb.Append(n.Name);
            sb.Append('_');
            count--;
        }

        count = 0;

        INamedTypeSymbol t = symbol.ContainingType;

        while (t is not null)
        {
            t = t.ContainingType;
            count++;
        }

        while (count > 0)
        {
            t = symbol.ContainingType;

            while (count > 1)
            {
                t = t.ContainingType;
                count--;
            }

            AppendType(t);
            sb.Append('_');
            count--;
        }

        if (symbol.IsKind(SymbolKind.NamedType))
        {
            AppendType((INamedTypeSymbol)symbol);
        }
        else
        {
            sb.Append(symbol.Name);
        }

        return StringBuilderCache.GetStringAndFree(sb);

        void AppendType(INamedTypeSymbol typeSymbol)
        {
            sb.Append(typeSymbol.Name);

            int arity = typeSymbol.Arity;

            if (arity > 0)
            {
                sb.Append('_');
                sb.Append(arity);
            }
        }
    }
}
