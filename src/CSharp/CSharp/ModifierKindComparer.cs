// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp;

internal class ModifierKindComparer : IComparer<SyntaxKind>
{
    public static ModifierKindComparer Default { get; } = new();

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
                return 10;
            case SyntaxKind.PrivateKeyword:
                return 20;
            case SyntaxKind.ProtectedKeyword:
                return 30;
            case SyntaxKind.InternalKeyword:
                return 40;
#if ROSLYN_4_4
            case SyntaxKind.FileKeyword:
                return 50;
#endif
            case SyntaxKind.ConstKeyword:
                return 60;
            case SyntaxKind.StaticKeyword:
                return 70;
            case SyntaxKind.VirtualKeyword:
                return 80;
            case SyntaxKind.SealedKeyword:
                return 90;
            case SyntaxKind.OverrideKeyword:
                return 100;
            case SyntaxKind.AbstractKeyword:
                return 110;
            case SyntaxKind.ReadOnlyKeyword:
                return 120;
            case SyntaxKind.ExternKeyword:
                return 130;
            case SyntaxKind.UnsafeKeyword:
                return 140;
            case SyntaxKind.FixedKeyword:
                return 150;
            case SyntaxKind.VolatileKeyword:
                return 160;
            case SyntaxKind.AsyncKeyword:
                return 170;
            case SyntaxKind.PartialKeyword:
                return 180;
            case SyntaxKind.ThisKeyword:
                return 190;
            case SyntaxKind.RefKeyword:
                return 200;
            case SyntaxKind.OutKeyword:
                return 210;
            case SyntaxKind.InKeyword:
                return 220;
            default:
                {
                    Debug.Fail($"unknown modifier '{kind}'");
                    return ModifierComparer.MaxRank;
                }
        }
    }
}
