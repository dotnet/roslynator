// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Roslynator.Text;

namespace Roslynator.Documentation
{
    internal static class DocumentationUtility
    {
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
            var commonNamespaces = new List<INamespaceSymbol>();

            var map = new Dictionary<INamespaceSymbol, List<List<INamespaceSymbol>>>(
                comparer: MetadataNameEqualityComparer<INamespaceSymbol>.Instance);

            foreach (ITypeSymbol symbol in symbols)
            {
                List<INamespaceSymbol> namespaces = symbol.GetContainingNamespaces().ToList();

                namespaces.Reverse();

                if (!map.TryGetValue(namespaces[0], out List<List<INamespaceSymbol>> value))
                {
                    value = new List<List<INamespaceSymbol>>();
                    map[namespaces[0]] = value;
                }

                value.Add(namespaces);
            }

            foreach (KeyValuePair<INamespaceSymbol, List<List<INamespaceSymbol>>> kvp in map)
            {
                List<List<INamespaceSymbol>> values = kvp.Value;
                List<INamespaceSymbol> first = values[0];
                int i = 1;

                while (i < first.Count)
                {
                    var success = true;

                    for (int j = 1; j < values.Count; j++)
                    {
                        List<INamespaceSymbol> other = values[j];

                        if (i >= other.Count
                            || !MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(first[i], other[i]))
                        {
                            success = false;
                            break;
                        }
                    }

                    if (success)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }

                commonNamespaces.AddRange(first[i - 1].GetContainingNamespacesAndSelf());
            }

            return commonNamespaces;
        }

        public static string CreateLocalLink(ISymbol symbol, string prefix = null)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            if (prefix != null)
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
                sb.Append("_");
                count--;
            }

            count = 0;

            INamedTypeSymbol t = symbol.ContainingType;

            while (t != null)
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
                sb.Append("_");
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
                    sb.Append("_");
                    sb.Append(arity.ToString());
                }
            }
        }
    }
}
