// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Extensions
{
    public static class SyntaxTokenListExtensions
    {
        public static SyntaxTokenList ReplaceAt(this SyntaxTokenList tokenList, int index, SyntaxToken newToken)
        {
            return tokenList.Replace(tokenList[index], newToken);
        }

        public static bool IsFirst(this SyntaxTokenList tokenList, SyntaxToken token)
        {
            return tokenList.IndexOf(token) == 0;
        }

        public static bool IsLast<TNode>(this SyntaxTokenList tokenList, SyntaxToken token)
        {
            return tokenList.Any()
                && tokenList.IndexOf(token) == tokenList.Count - 1;
        }
    }
}
