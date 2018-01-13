// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Comparers
{
    internal class ModifierComparer : IModifierComparer
    {
        public static readonly ModifierComparer Instance = new ModifierComparer();

        private const int _maxOrderIndex = 16;

        private ModifierComparer()
        {
        }

        public int Compare(SyntaxToken x, SyntaxToken y)
        {
            return GetOrderIndex(x).CompareTo(GetOrderIndex(y));
        }

        public int GetInsertIndex(SyntaxTokenList modifiers, SyntaxToken modifier)
        {
            return GetInsertIndex(modifiers, GetOrderIndex(modifier));
        }

        public int GetInsertIndex(SyntaxTokenList modifiers, SyntaxKind modifierKind)
        {
            return GetInsertIndex(modifiers, GetOrderIndex(modifierKind));
        }

        private static int GetInsertIndex(SyntaxTokenList modifiers, int orderIndex)
        {
            if (modifiers.Any())
            {
                for (int i = orderIndex; i >= 0; i--)
                {
                    SyntaxKind kind = GetKind(i);

                    for (int j = modifiers.Count - 1; j >= 0; j--)
                    {
                        if (modifiers[j].IsKind(kind))
                            return j + 1;
                    }
                }
            }

            return 0;
        }

        private static int GetOrderIndex(SyntaxToken token)
        {
            return GetOrderIndex(token.Kind());
        }

        private static int GetOrderIndex(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.NewKeyword:
                    return 0;
                case SyntaxKind.PublicKeyword:
                    return 1;
                case SyntaxKind.PrivateKeyword:
                    return 2;
                case SyntaxKind.ProtectedKeyword:
                    return 3;
                case SyntaxKind.InternalKeyword:
                    return 4;
                case SyntaxKind.ConstKeyword:
                    return 5;
                case SyntaxKind.StaticKeyword:
                    return 6;
                case SyntaxKind.VirtualKeyword:
                    return 7;
                case SyntaxKind.SealedKeyword:
                    return 8;
                case SyntaxKind.OverrideKeyword:
                    return 9;
                case SyntaxKind.AbstractKeyword:
                    return 10;
                case SyntaxKind.ReadOnlyKeyword:
                    return 11;
                case SyntaxKind.ExternKeyword:
                    return 12;
                case SyntaxKind.UnsafeKeyword:
                    return 13;
                case SyntaxKind.VolatileKeyword:
                    return 14;
                case SyntaxKind.AsyncKeyword:
                    return 15;
                case SyntaxKind.PartialKeyword:
                    return 16;
                default:
                    {
                        Debug.Fail($"unknown modifier '{kind}'");
                        return _maxOrderIndex;
                    }
            }
        }

        private static SyntaxKind GetKind(int orderIndex)
        {
            switch (orderIndex)
            {
                case 0:
                    return SyntaxKind.NewKeyword;
                case 1:
                    return SyntaxKind.PublicKeyword;
                case 2:
                    return SyntaxKind.ProtectedKeyword;
                case 3:
                    return SyntaxKind.InternalKeyword;
                case 4:
                    return SyntaxKind.PrivateKeyword;
                case 5:
                    return SyntaxKind.ConstKeyword;
                case 6:
                    return SyntaxKind.StaticKeyword;
                case 7:
                    return SyntaxKind.VirtualKeyword;
                case 8:
                    return SyntaxKind.SealedKeyword;
                case 9:
                    return SyntaxKind.OverrideKeyword;
                case 10:
                    return SyntaxKind.AbstractKeyword;
                case 11:
                    return SyntaxKind.ReadOnlyKeyword;
                case 12:
                    return SyntaxKind.ExternKeyword;
                case 13:
                    return SyntaxKind.UnsafeKeyword;
                case 14:
                    return SyntaxKind.VolatileKeyword;
                case 15:
                    return SyntaxKind.AsyncKeyword;
                case 16:
                    return SyntaxKind.PartialKeyword;
                default:
                    return SyntaxKind.None;
            }
        }

        public bool IsListSorted(SyntaxTokenList modifiers)
        {
            int count = modifiers.Count;

            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    if (Compare(modifiers[i], modifiers[i + 1]) > 0)
                        return false;
                }
            }

            return true;
        }
    }
}
