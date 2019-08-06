// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class RoslynSymbolUtility
    {
        public static bool IsList(INamedTypeSymbol typeSymbol)
        {
            switch (typeSymbol?.MetadataName)
            {
                case "ChildSyntaxList":
                case "SeparatedSyntaxList`1":
                case "SyntaxList`1":
                case "SyntaxNodeOrTokenList":
                case "SyntaxTokenList":
                case "SyntaxTriviaList":
                    {
                        return typeSymbol.ContainingNamespace.HasMetadataName(RoslynMetadataNames.Microsoft_CodeAnalysis);
                    }
            }

            return false;
        }

        public static bool IsRoslynType(ISymbol symbol)
        {
            INamespaceSymbol namespaceSymbol = symbol?.ContainingNamespace;

            while (namespaceSymbol?.IsGlobalNamespace == false)
            {
                if (namespaceSymbol.Name == "CodeAnalysis")
                {
                    INamespaceSymbol containingNamespace = namespaceSymbol.ContainingNamespace;

                    if (containingNamespace?.Name == "Microsoft")
                    {
                        containingNamespace = containingNamespace.ContainingNamespace;

                        if (containingNamespace?.IsGlobalNamespace == true)
                            return true;
                    }
                }

                namespaceSymbol = namespaceSymbol.ContainingNamespace;
            }

            return false;
        }
    }
}
