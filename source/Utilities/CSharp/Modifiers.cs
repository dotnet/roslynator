// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class Modifiers
    {
        public static SyntaxTokenList FromAccessibility(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return Private();
                case Accessibility.Protected:
                    return Protected();
                case Accessibility.Internal:
                    return Internal();
                case Accessibility.ProtectedOrInternal:
                    return ProtectedInternal();
                case Accessibility.Public:
                    return Public();
                case Accessibility.NotApplicable:
                    return default(SyntaxTokenList);
                case Accessibility.ProtectedAndInternal:
                    throw new NotSupportedException($"Value '{accessibility}' is not supported.)");
                default:
                    {
                        Debug.Assert(false, accessibility.ToString());
                        return default(SyntaxTokenList);
                    }
            }
        }

        public static SyntaxTokenList PrivateStaticReadOnly()
        {
            return TokenList(PrivateToken(), StaticToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList Internal()
        {
            return TokenList(InternalToken());
        }

        public static SyntaxTokenList InternalStatic()
        {
            return TokenList(InternalToken(), StaticToken());
        }

        public static SyntaxTokenList Protected()
        {
            return TokenList(ProtectedToken());
        }

        public static SyntaxTokenList ProtectedInternal()
        {
            return TokenList(ProtectedToken(), InternalToken());
        }

        public static SyntaxTokenList Private()
        {
            return TokenList(PrivateToken());
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
