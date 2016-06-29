// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.Comparers
{
    public sealed class ModifierSorter : IComparer<SyntaxToken>
    {
        public static readonly ModifierSorter Instance = new ModifierSorter();

        public int Compare(SyntaxToken x, SyntaxToken y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            return GetOrderIndex(x).CompareTo(GetOrderIndex(y));
        }

        private static int GetOrderIndex(SyntaxToken syntaxToken)
        {
            switch (syntaxToken.Kind())
            {
                case SyntaxKind.NewKeyword:
                    return 0;
                case SyntaxKind.PublicKeyword:
                    return 1;
                case SyntaxKind.ProtectedKeyword:
                    return 2;
                case SyntaxKind.InternalKeyword:
                    return 3;
                case SyntaxKind.PrivateKeyword:
                    return 4;
                case SyntaxKind.StaticKeyword:
                    return 5;
                case SyntaxKind.VirtualKeyword:
                    return 6;
                case SyntaxKind.SealedKeyword:
                    return 7;
                case SyntaxKind.OverrideKeyword:
                    return 8;
                case SyntaxKind.AbstractKeyword:
                    return 9;
                case SyntaxKind.ReadOnlyKeyword:
                    return 10;
                case SyntaxKind.ExternKeyword:
                    return 11;
                case SyntaxKind.UnsafeKeyword:
                    return 12;
                case SyntaxKind.VolatileKeyword:
                    return 13;
                case SyntaxKind.AsyncKeyword:
                    return 14;
                case SyntaxKind.PartialKeyword:
                    return 15;
                default:
                    return 16;
                }
        }

        public static bool AreModifiersSorted(SyntaxTokenList list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (Instance.Compare(list[i], list[i + 1]) >= 0)
                    return false;
            }

            return true;
        }
    }
}
