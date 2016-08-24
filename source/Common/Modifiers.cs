// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis
{
    public static class Modifiers
    {
        public static SyntaxTokenList PrivateStaticReadOnly()
        {
            return TokenList(Private(), Static(), ReadOnly());
        }

        public static SyntaxTokenList PrivateReadOnly()
        {
            return TokenList(Private(), ReadOnly());
        }

        public static SyntaxTokenList PublicConst()
        {
            return TokenList(Public(), Const());
        }

        private static SyntaxToken Public()
        {
            return PublicToken();
        }

        private static SyntaxToken Private()
        {
            return PrivateToken();
        }

        private static SyntaxToken Static()
        {
            return StaticToken();
        }

        private static SyntaxToken ReadOnly()
        {
            return ReadOnlyToken();
        }

        private static SyntaxToken Const()
        {
            return ConstToken();
        }
    }
}
