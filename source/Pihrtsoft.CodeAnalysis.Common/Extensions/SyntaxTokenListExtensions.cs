// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis
{
    public static class SyntaxTokenListExtensions
    {
        private static readonly HashSet<SyntaxKind> _accessModifiers = new HashSet<SyntaxKind>(AccessModifierKinds);

        private static IEnumerable<SyntaxKind> AccessModifierKinds
        {
            get
            {
                yield return SyntaxKind.PublicKeyword;
                yield return SyntaxKind.InternalKeyword;
                yield return SyntaxKind.ProtectedKeyword;
                yield return SyntaxKind.PrivateKeyword;
            }
        }

        public static bool Contains(this SyntaxTokenList tokenList, SyntaxKind kind)
            => tokenList.IndexOf(kind) != -1;

        public static bool ContainsAccessModifier(this SyntaxTokenList modifiers)
        {
            for (int i = 0; i < modifiers.Count; i++)
            {
                if (_accessModifiers.Contains(modifiers[i].Kind()))
                    return true;
            }

            return false;
        }
    }
}
