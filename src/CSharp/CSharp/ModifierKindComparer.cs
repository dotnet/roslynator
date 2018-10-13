// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    internal class ModifierKindComparer : IComparer<SyntaxKind>
    {
        public static ModifierKindComparer Default { get; } = new ModifierKindComparer();

        public int Compare(SyntaxKind x, SyntaxKind y)
        {
            return GetRank(x).CompareTo(GetRank(y));
        }

        public virtual int GetRank(SyntaxKind kind)
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
                case SyntaxKind.FixedKeyword:
                    return 14;
                case SyntaxKind.VolatileKeyword:
                    return 15;
                case SyntaxKind.AsyncKeyword:
                    return 16;
                case SyntaxKind.PartialKeyword:
                    return 17;
                case SyntaxKind.ThisKeyword:
                    return 18;
                case SyntaxKind.RefKeyword:
                    return 19;
                case SyntaxKind.OutKeyword:
                    return 20;
                case SyntaxKind.InKeyword:
                    return 21;
                default:
                    {
                        Debug.Fail($"unknown modifier '{kind}'");
                        return ModifierComparer.MaxRank;
                    }
            }
        }
    }
}
