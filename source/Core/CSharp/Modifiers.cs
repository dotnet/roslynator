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
        public static SyntaxTokenList None()
        {
            return TokenList();
        }

        public static SyntaxTokenList Public()
        {
            return TokenList(PublicKeyword());
        }

        public static SyntaxTokenList Internal()
        {
            return TokenList(InternalKeyword());
        }

        public static SyntaxTokenList Protected()
        {
            return TokenList(ProtectedKeyword());
        }

        public static SyntaxTokenList ProtectedInternal()
        {
            return TokenList(ProtectedKeyword(), InternalKeyword());
        }

        public static SyntaxTokenList Private()
        {
            return TokenList(PrivateKeyword());
        }

        public static SyntaxTokenList PrivateProtected()
        {
            return TokenList(PrivateKeyword(), ProtectedKeyword());
        }

        public static SyntaxTokenList Virtual()
        {
            return TokenList(VirtualKeyword());
        }

        public static SyntaxTokenList PublicVirtual()
        {
            return TokenList(PublicKeyword(), VirtualKeyword());
        }

        public static SyntaxTokenList InternalVirtual()
        {
            return TokenList(InternalKeyword(), VirtualKeyword());
        }

        public static SyntaxTokenList ProtectedVirtual()
        {
            return TokenList(ProtectedKeyword(), VirtualKeyword());
        }

        public static SyntaxTokenList PublicAbstract()
        {
            return TokenList(PublicKeyword(), AbstractKeyword());
        }

        public static SyntaxTokenList InternalAbstract()
        {
            return TokenList(InternalKeyword(), AbstractKeyword());
        }

        public static SyntaxTokenList ProtectedAbstract()
        {
            return TokenList(ProtectedKeyword(), AbstractKeyword());
        }

        public static SyntaxTokenList PublicOverride()
        {
            return TokenList(PublicKeyword(), OverrideKeyword());
        }

        public static SyntaxTokenList InternalOverride()
        {
            return TokenList(InternalKeyword(), OverrideKeyword());
        }

        public static SyntaxTokenList ProtectedOverride()
        {
            return TokenList(ProtectedKeyword(), OverrideKeyword());
        }

        public static SyntaxTokenList Const()
        {
            return TokenList(ConstKeyword());
        }

        public static SyntaxTokenList PublicConst()
        {
            return TokenList(PublicKeyword(), ConstKeyword());
        }

        public static SyntaxTokenList InternalConst()
        {
            return TokenList(InternalKeyword(), ConstKeyword());
        }

        public static SyntaxTokenList ProtectedConst()
        {
            return TokenList(ProtectedKeyword(), ConstKeyword());
        }

        public static SyntaxTokenList PrivateConst()
        {
            return TokenList(PrivateKeyword(), ConstKeyword());
        }

        public static SyntaxTokenList Static()
        {
            return TokenList(StaticKeyword());
        }

        public static SyntaxTokenList PublicStatic()
        {
            return TokenList(PublicKeyword(), StaticKeyword());
        }

        public static SyntaxTokenList InternalStatic()
        {
            return TokenList(InternalKeyword(), StaticKeyword());
        }

        public static SyntaxTokenList ProtectedStatic()
        {
            return TokenList(ProtectedKeyword(), StaticKeyword());
        }

        public static SyntaxTokenList PrivateStatic()
        {
            return TokenList(PrivateKeyword(), StaticKeyword());
        }

        public static SyntaxTokenList StaticReadOnly()
        {
            return TokenList(StaticKeyword(), ReadOnlyKeyword());
        }

        public static SyntaxTokenList PublicStaticReadOnly()
        {
            return TokenList(PublicKeyword(), StaticKeyword(), ReadOnlyKeyword());
        }

        public static SyntaxTokenList InternalStaticReadOnly()
        {
            return TokenList(InternalKeyword(), StaticKeyword(), ReadOnlyKeyword());
        }

        public static SyntaxTokenList ProtectedStaticReadOnly()
        {
            return TokenList(ProtectedKeyword(), StaticKeyword(), ReadOnlyKeyword());
        }

        public static SyntaxTokenList PrivateStaticReadOnly()
        {
            return TokenList(PrivateKeyword(), StaticKeyword(), ReadOnlyKeyword());
        }

        public static SyntaxTokenList ReadOnly()
        {
            return TokenList(ReadOnlyKeyword());
        }

        public static SyntaxTokenList PublicReadOnly()
        {
            return TokenList(PublicKeyword(), ReadOnlyKeyword());
        }

        public static SyntaxTokenList InternalReadOnly()
        {
            return TokenList(InternalKeyword(), ReadOnlyKeyword());
        }

        public static SyntaxTokenList ProtectedReadOnly()
        {
            return TokenList(ProtectedKeyword(), ReadOnlyKeyword());
        }

        public static SyntaxTokenList PrivateReadOnly()
        {
            return TokenList(PrivateKeyword(), ReadOnlyKeyword());
        }

        public static SyntaxTokenList Partial()
        {
            return TokenList(PartialKeyword());
        }

        public static SyntaxTokenList PublicPartial()
        {
            return TokenList(PublicKeyword(), PartialKeyword());
        }

        public static SyntaxTokenList InternalPartial()
        {
            return TokenList(InternalKeyword(), PartialKeyword());
        }

        public static SyntaxTokenList PrivatePartial()
        {
            return TokenList(PrivateKeyword(), PartialKeyword());
        }

        public static SyntaxTokenList PublicStaticPartial()
        {
            return TokenList(PublicKeyword(), StaticKeyword(), PartialKeyword());
        }

        public static SyntaxTokenList InternalStaticPartial()
        {
            return TokenList(InternalKeyword(), StaticKeyword(), PartialKeyword());
        }

        public static SyntaxTokenList PrivateStaticPartial()
        {
            return TokenList(PrivateKeyword(), StaticKeyword(), PartialKeyword());
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
                    return PrivateProtected();
                default:
                    {
                        Debug.Fail(accessibility.ToString());
                        return default(SyntaxTokenList);
                    }
            }
        }

        public static SyntaxTokenList In()
        {
            return TokenList(InKeyword());
        }

        public static SyntaxTokenList Out()
        {
            return TokenList(OutKeyword());
        }

        public static SyntaxTokenList Ref()
        {
            return TokenList(RefKeyword());
        }

        public static SyntaxTokenList RefReadOnly()
        {
            return TokenList(RefKeyword(), ReadOnlyKeyword());
        }

        public static SyntaxTokenList Params()
        {
            return TokenList(ParamsKeyword());
        }

        internal static SyntaxTokenList FromParameterSymbol(IParameterSymbol parameterSymbol)
        {
            if (parameterSymbol == null)
                throw new ArgumentNullException(nameof(parameterSymbol));

            if (parameterSymbol.IsParams)
                return Params();

            switch (parameterSymbol.RefKind)
            {
                case RefKind.None:
                    return default(SyntaxTokenList);
                case RefKind.Ref:
                    return Ref();
                case RefKind.Out:
                    return Out();
            }

            Debug.Fail(parameterSymbol.RefKind.ToString());
            return default(SyntaxTokenList);
        }
    }
}
