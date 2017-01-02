// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
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
    }
}
