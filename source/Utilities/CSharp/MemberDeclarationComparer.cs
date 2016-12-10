// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal class MemberDeclarationComparer : IComparer<MemberDeclarationSyntax>
    {
        public static readonly MemberDeclarationComparer Instance = new MemberDeclarationComparer();
        public static readonly int MaxOrderIndex = 18;

        public int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            return GetOrderIndex(x).CompareTo(GetOrderIndex(y));
        }

        public static int GetOrderIndex(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.FieldDeclaration:
                    {
                        return (((FieldDeclarationSyntax)memberDeclaration).IsConst()) ? 0 : 1;
                    }
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
                        Debug.Assert(false, $"unknown member '{memberDeclaration.Kind()}'");
                        return MaxOrderIndex;
                    }
            }
        }

        public static int GetOrderIndex(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.FieldDeclaration:
                    return 1;
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
                        Debug.Assert(false, $"unknown member '{kind}'");
                        return MaxOrderIndex;
                    }
            }
        }
    }
}
