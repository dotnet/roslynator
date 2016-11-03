// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    public static class Modifiers
    {
        public static SyntaxTokenList Public()
        {
            return TokenList(PublicToken());
        }

        public static SyntaxTokenList Internal()
        {
            return TokenList(InternalToken());
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

        public static SyntaxTokenList Virtual()
        {
            return TokenList(VirtualToken());
        }

        public static SyntaxTokenList PublicVirtual()
        {
            return TokenList(PublicToken(), VirtualToken());
        }

        public static SyntaxTokenList InternalVirtual()
        {
            return TokenList(InternalToken(), VirtualToken());
        }

        public static SyntaxTokenList ProtectedVirtual()
        {
            return TokenList(ProtectedToken(), VirtualToken());
        }

        public static SyntaxTokenList Const()
        {
            return TokenList(ConstToken());
        }

        public static SyntaxTokenList PublicConst()
        {
            return TokenList(PublicToken(), ConstToken());
        }

        public static SyntaxTokenList InternalConst()
        {
            return TokenList(InternalToken(), ConstToken());
        }

        public static SyntaxTokenList ProtectedConst()
        {
            return TokenList(ProtectedToken(), ConstToken());
        }

        public static SyntaxTokenList PrivateConst()
        {
            return TokenList(PrivateToken(), ConstToken());
        }

        public static SyntaxTokenList Static()
        {
            return TokenList(StaticToken());
        }

        public static SyntaxTokenList PublicStatic()
        {
            return TokenList(PublicToken(), StaticToken());
        }

        public static SyntaxTokenList InternalStatic()
        {
            return TokenList(InternalToken(), StaticToken());
        }

        public static SyntaxTokenList ProtectedStatic()
        {
            return TokenList(ProtectedToken(), StaticToken());
        }

        public static SyntaxTokenList PrivateStatic()
        {
            return TokenList(PrivateToken(), StaticToken());
        }

        public static SyntaxTokenList StaticReadOnly()
        {
            return TokenList(StaticToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList PublicStaticReadOnly()
        {
            return TokenList(PublicToken(), StaticToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList InternalStaticReadOnly()
        {
            return TokenList(InternalToken(), StaticToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList ProtectedStaticReadOnly()
        {
            return TokenList(ProtectedToken(), StaticToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList PrivateStaticReadOnly()
        {
            return TokenList(PrivateToken(), StaticToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList ReadOnly()
        {
            return TokenList(ReadOnlyToken());
        }

        public static SyntaxTokenList PublicReadOnly()
        {
            return TokenList(PublicToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList InternalReadOnly()
        {
            return TokenList(InternalToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList ProtectedReadOnly()
        {
            return TokenList(ProtectedToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList PrivateReadOnly()
        {
            return TokenList(PrivateToken(), ReadOnlyToken());
        }

        public static SyntaxTokenList Partial()
        {
            return TokenList(PartialToken());
        }

        public static SyntaxTokenList PublicPartial()
        {
            return TokenList(PublicToken(), PartialToken());
        }

        public static SyntaxTokenList InternalPartial()
        {
            return TokenList(InternalToken(), PartialToken());
        }

        public static SyntaxTokenList PrivatePartial()
        {
            return TokenList(PrivateToken(), PartialToken());
        }

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
    }
}
