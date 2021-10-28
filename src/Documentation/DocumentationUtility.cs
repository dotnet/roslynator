// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using Microsoft.CodeAnalysis;
using Roslynator.Text;

namespace Roslynator.Documentation
{
    internal static class DocumentationUtility
    {
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
