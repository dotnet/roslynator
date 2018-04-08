// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    /// <summary>
    /// Serves as a factory for a modifier list.
    /// </summary>
    public static class Modifiers
    {
        /// <summary>
        /// Creates a list of modifiers that contains "public" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Public()
        {
            return TokenList(PublicKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal()
        {
            return TokenList(InternalKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Protected()
        {
            return TokenList(ProtectedKeyword());
        }

        /// <summary>
        /// Return modifier list that contains "protected internal" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList ProtectedInternal()
        {
            return TokenList(ProtectedKeyword(), InternalKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Private()
        {
            return TokenList(PrivateKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private protected" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PrivateProtected()
        {
            return TokenList(PrivateKeyword(), ProtectedKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "virtual" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Virtual()
        {
            return TokenList(VirtualKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public virtual" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PublicVirtual()
        {
            return TokenList(PublicKeyword(), VirtualKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal virtual" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList InternalVirtual()
        {
            return TokenList(InternalKeyword(), VirtualKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected virtual" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList ProtectedVirtual()
        {
            return TokenList(ProtectedKeyword(), VirtualKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public abstract" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PublicAbstract()
        {
            return TokenList(PublicKeyword(), AbstractKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal abstract" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList InternalAbstract()
        {
            return TokenList(InternalKeyword(), AbstractKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected abstract" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList ProtectedAbstract()
        {
            return TokenList(ProtectedKeyword(), AbstractKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public override" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PublicOverride()
        {
            return TokenList(PublicKeyword(), OverrideKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal override" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList InternalOverride()
        {
            return TokenList(InternalKeyword(), OverrideKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected override" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList ProtectedOverride()
        {
            return TokenList(ProtectedKeyword(), OverrideKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "const" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Const()
        {
            return TokenList(ConstKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public const" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PublicConst()
        {
            return TokenList(PublicKeyword(), ConstKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal const" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList InternalConst()
        {
            return TokenList(InternalKeyword(), ConstKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected const" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList ProtectedConst()
        {
            return TokenList(ProtectedKeyword(), ConstKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private const" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PrivateConst()
        {
            return TokenList(PrivateKeyword(), ConstKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "static" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Static()
        {
            return TokenList(StaticKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public static" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PublicStatic()
        {
            return TokenList(PublicKeyword(), StaticKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal static" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList InternalStatic()
        {
            return TokenList(InternalKeyword(), StaticKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected static" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList ProtectedStatic()
        {
            return TokenList(ProtectedKeyword(), StaticKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private static" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PrivateStatic()
        {
            return TokenList(PrivateKeyword(), StaticKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "static readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList StaticReadOnly()
        {
            return TokenList(StaticKeyword(), ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public static readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PublicStaticReadOnly()
        {
            return TokenList(PublicKeyword(), StaticKeyword(), ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal static readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList InternalStaticReadOnly()
        {
            return TokenList(InternalKeyword(), StaticKeyword(), ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected static readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList ProtectedStaticReadOnly()
        {
            return TokenList(ProtectedKeyword(), StaticKeyword(), ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private static readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PrivateStaticReadOnly()
        {
            return TokenList(PrivateKeyword(), StaticKeyword(), ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "readonly" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList ReadOnly()
        {
            return TokenList(ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PublicReadOnly()
        {
            return TokenList(PublicKeyword(), ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList InternalReadOnly()
        {
            return TokenList(InternalKeyword(), ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList ProtectedReadOnly()
        {
            return TokenList(ProtectedKeyword(), ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PrivateReadOnly()
        {
            return TokenList(PrivateKeyword(), ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "partial" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Partial()
        {
            return TokenList(PartialKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PublicPartial()
        {
            return TokenList(PublicKeyword(), PartialKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList InternalPartial()
        {
            return TokenList(InternalKeyword(), PartialKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PrivatePartial()
        {
            return TokenList(PrivateKeyword(), PartialKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public static partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PublicStaticPartial()
        {
            return TokenList(PublicKeyword(), StaticKeyword(), PartialKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal static partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList InternalStaticPartial()
        {
            return TokenList(InternalKeyword(), StaticKeyword(), PartialKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private static partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList PrivateStaticPartial()
        {
            return TokenList(PrivateKeyword(), StaticKeyword(), PartialKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers from the specified accessibility.
        /// </summary>
        /// <param name="accessibility"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a list of modifiers that contains "in" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList In()
        {
            return TokenList(InKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "out" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Out()
        {
            return TokenList(OutKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "ref" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Ref()
        {
            return TokenList(RefKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "ref readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList RefReadOnly()
        {
            return TokenList(RefKeyword(), ReadOnlyKeyword());
        }

        /// <summary>
        /// Creates a list of modifiers that contains "params" modifier.
        /// </summary>
        /// <returns></returns>
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
