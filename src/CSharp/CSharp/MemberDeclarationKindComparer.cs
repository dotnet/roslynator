// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    internal class MemberDeclarationKindComparer : IComparer<SyntaxKind>
    {
        public static MemberDeclarationKindComparer Default { get; } = new MemberDeclarationKindComparer();

        public int Compare(SyntaxKind x, SyntaxKind y)
        {
            return GetRank(x).CompareTo(GetRank(y));
        }

        public virtual int GetRank(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.FieldDeclaration:
                    return MemberDeclarationComparer.FieldRank;
                case SyntaxKind.ConstructorDeclaration:
                    return 2;
                case SyntaxKind.DestructorDeclaration:
                    return 3;
                case SyntaxKind.DelegateDeclaration:
                    return 4;
                case SyntaxKind.EventDeclaration:
                    return 5;
                case SyntaxKind.EventFieldDeclaration:
                    return 6;
                case SyntaxKind.PropertyDeclaration:
                    return 7;
                case SyntaxKind.IndexerDeclaration:
                    return 8;
                case SyntaxKind.MethodDeclaration:
                    return 9;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return 10;
                case SyntaxKind.OperatorDeclaration:
                    return 11;
                case SyntaxKind.EnumDeclaration:
                    return 12;
                case SyntaxKind.InterfaceDeclaration:
                    return 13;
                case SyntaxKind.StructDeclaration:
                    return 14;
                case SyntaxKind.ClassDeclaration:
                    return 15;
                case SyntaxKind.NamespaceDeclaration:
                    return 16;
                case SyntaxKind.IncompleteMember:
                    return 17;
                default:
                    {
                        Debug.Fail($"unknown member '{kind}'");
                        return MemberDeclarationComparer.MaxRank;
                    }
            }
        }
    }
}
