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
            return TokenList(PrivateToken(), StaticToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList InternalStatic()
        {
            return TokenList(InternalToken(), StaticToken());
        }

        public static SyntaxTokenList PrivateReadOnly()
        {
            return TokenList(PrivateToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList Public()
        {
            return TokenList(PublicToken());
        }

        public static SyntaxTokenList PublicConst()
        {
            return TokenList(PublicToken(), ConstToken());
        }

        public static SyntaxTokenList PublicStatic()
        {
            return TokenList(PublicToken(), StaticToken());
        }

        public static SyntaxTokenList PublicPartial()
        {
            return TokenList(PublicToken(), PartialToken());
        }

        public static SyntaxTokenList Static()
        {
            return TokenList(StaticToken());
        }
    }
}
