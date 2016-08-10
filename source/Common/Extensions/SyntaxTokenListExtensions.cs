// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis
{
    public static class SyntaxTokenListExtensions
    {
        public static bool Contains(this SyntaxTokenList tokenList, SyntaxKind kind)
        {
            return tokenList.IndexOf(kind) != -1;
        }

        public static bool ContainsAccessModifier(this SyntaxTokenList tokenList)
        {
            return tokenList.Any(token => token.IsAccessModifier());
        }

        public static SyntaxTokenList RemoveAccessModifiers(this SyntaxTokenList tokenList)
        {
            for (int i = tokenList.Count - 1; i >= 0; i--)
            {
                if (tokenList[i].IsAccessModifier())
                    tokenList = tokenList.RemoveAt(i);
            }

            return tokenList;
        }
    }
}
