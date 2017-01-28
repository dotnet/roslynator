// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Extensions
{
    public static class SyntaxTokenListExtensions
    {
        public static SyntaxTokenList RemoveAccessModifiers(this SyntaxTokenList tokenList)
        {
            return TokenList(tokenList.Where(token => !token.IsAccessModifier()));
        }

        public static bool ContainsAccessModifier(this SyntaxTokenList tokenList)
        {
            return tokenList.Any(token => token.IsAccessModifier());
        }

        internal static Accessibility GetAccessibility(this SyntaxTokenList tokenList)
        {
            int count = tokenList.Count;

            for (int i = 0; i < count; i++)
            {
                switch (tokenList[i].Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        return Accessibility.Public;
                    case SyntaxKind.PrivateKeyword:
                        return Accessibility.Private;
                    case SyntaxKind.InternalKeyword:
                        return GetAccessModifier(tokenList, i + 1, count, SyntaxKind.ProtectedKeyword, Accessibility.Internal);
                    case SyntaxKind.ProtectedKeyword:
                        return GetAccessModifier(tokenList, i + 1, count, SyntaxKind.InternalKeyword, Accessibility.Protected);
                }
            }

            return Accessibility.NotApplicable;
        }

        private static Accessibility GetAccessModifier(SyntaxTokenList tokenList, int startIndex, int count, SyntaxKind kind, Accessibility accessModifier)
        {
            for (int i = startIndex; i < count; i++)
            {
                if (tokenList[i].Kind() == kind)
                    return Accessibility.ProtectedOrInternal;
            }

            return accessModifier;
        }
    }
}
