// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
            return TokenList(SyntaxKind.PublicKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal()
        {
            return TokenList(SyntaxKind.InternalKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Protected()
        {
            return TokenList(SyntaxKind.ProtectedKeyword);
        }

        /// <summary>
        /// Return modifier list that contains "protected internal" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Protected_Internal()
        {
            return TokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Private()
        {
            return TokenList(SyntaxKind.PrivateKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private protected" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Private_Protected()
        {
            return TokenList(SyntaxKind.PrivateKeyword, SyntaxKind.ProtectedKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "virtual" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Virtual()
        {
            return TokenList(SyntaxKind.VirtualKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public virtual" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Public_Virtual()
        {
            return TokenList(SyntaxKind.PublicKeyword, SyntaxKind.VirtualKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal virtual" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal_Virtual()
        {
            return TokenList(SyntaxKind.InternalKeyword, SyntaxKind.VirtualKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected virtual" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Protected_Virtual()
        {
            return TokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.VirtualKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public abstract" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Public_Abstract()
        {
            return TokenList(SyntaxKind.PublicKeyword, SyntaxKind.AbstractKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal abstract" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal_Abstract()
        {
            return TokenList(SyntaxKind.InternalKeyword, SyntaxKind.AbstractKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected abstract" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Protected_Abstract()
        {
            return TokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.AbstractKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public override" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Public_Override()
        {
            return TokenList(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal override" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal_Override()
        {
            return TokenList(SyntaxKind.InternalKeyword, SyntaxKind.OverrideKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected override" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Protected_Override()
        {
            return TokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "const" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Const()
        {
            return TokenList(SyntaxKind.ConstKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public const" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Public_Const()
        {
            return TokenList(SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal const" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal_Const()
        {
            return TokenList(SyntaxKind.InternalKeyword, SyntaxKind.ConstKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected const" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Protected_Const()
        {
            return TokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.ConstKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private const" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Private_Const()
        {
            return TokenList(SyntaxKind.PrivateKeyword, SyntaxKind.ConstKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "static" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Static()
        {
            return TokenList(SyntaxKind.StaticKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public static" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Public_Static()
        {
            return TokenList(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal static" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal_Static()
        {
            return TokenList(SyntaxKind.InternalKeyword, SyntaxKind.StaticKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected static" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Protected_Static()
        {
            return TokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.StaticKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private static" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Private_Static()
        {
            return TokenList(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "static readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Static_ReadOnly()
        {
            return TokenList(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public static readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Public_Static_ReadOnly()
        {
            return TokenList(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal static readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal_Static_ReadOnly()
        {
            return TokenList(SyntaxKind.InternalKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected static readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Protected_Static_ReadOnly()
        {
            return TokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private static readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Private_Static_ReadOnly()
        {
            return TokenList(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "readonly" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList ReadOnly()
        {
            return TokenList(SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Public_ReadOnly()
        {
            return TokenList(SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal_ReadOnly()
        {
            return TokenList(SyntaxKind.InternalKeyword, SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "protected readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Protected_ReadOnly()
        {
            return TokenList(SyntaxKind.ProtectedKeyword, SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Private_ReadOnly()
        {
            return TokenList(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "partial" modifier.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Partial()
        {
            return TokenList(SyntaxKind.PartialKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Public_Partial()
        {
            return TokenList(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal_Partial()
        {
            return TokenList(SyntaxKind.InternalKeyword, SyntaxKind.PartialKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Private_Partial()
        {
            return TokenList(SyntaxKind.PrivateKeyword, SyntaxKind.PartialKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "public static partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Public_Static_Partial()
        {
            return TokenList(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "internal static partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Internal_Static_Partial()
        {
            return TokenList(SyntaxKind.InternalKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "private static partial" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Private_Static_Partial()
        {
            return TokenList(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword);
        }

        /// <summary>
        /// Creates a list of modifiers that contains "ref readonly" modifiers.
        /// </summary>
        /// <returns></returns>
        public static SyntaxTokenList Ref_ReadOnly()
        {
            return TokenList(SyntaxKind.RefKeyword, SyntaxKind.ReadOnlyKeyword);
        }
    }
}
