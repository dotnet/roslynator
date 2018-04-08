// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal abstract class MemberDeclarationComparer : IComparer<MemberDeclarationSyntax>
    {
        internal const int MaxRank = 18;

        internal const int ConstRank = 0;
        internal const int FieldRank = 1;

        internal static MemberDeclarationComparer ByKind { get; } = new ByKindMemberDeclarationComparer();

        internal static MemberDeclarationComparer ByKindThenByName { get; } = new ByKindThenByNameMemberDeclarationComparer();

        public abstract int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y);

        public virtual int GetRank(MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxKind kind = member.Kind();

            if (kind == SyntaxKind.FieldDeclaration)
            {
                return (((FieldDeclarationSyntax)member).Modifiers.Contains(SyntaxKind.ConstKeyword))
                    ? ConstRank
                    : FieldRank;
            }

            return MemberDeclarationKindComparer.Default.GetRank(kind);
        }

        internal static bool CanBeSortedByName(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                    return true;
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.IncompleteMember:
                    return false;
                default:
                    {
                        Debug.Fail($"unknown member '{kind}'");
                        return false;
                    }
            }
        }

        private class ByKindMemberDeclarationComparer : MemberDeclarationComparer
        {
            public override int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return GetRank(x).CompareTo(GetRank(y));
            }
        }

        private sealed class ByKindThenByNameMemberDeclarationComparer : ByKindMemberDeclarationComparer
        {
            public override int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
            {
                int result = base.Compare(x, y);

                if (result == 0)
                {
                    return string.Compare(GetName(x), GetName(y), StringComparison.CurrentCulture);
                }
                else
                {
                    return result;
                }
            }

            private static string GetName(MemberDeclarationSyntax member)
            {
                switch (member.Kind())
                {
                    case SyntaxKind.FieldDeclaration:
                        {
                            return ((FieldDeclarationSyntax)member).Declaration?.Variables.FirstOrDefault()?.Identifier.ValueText;
                        }
                    case SyntaxKind.ConstructorDeclaration:
                        return ((ConstructorDeclarationSyntax)member).Identifier.ValueText;
                    case SyntaxKind.DelegateDeclaration:
                        return ((DelegateDeclarationSyntax)member).Identifier.ValueText;
                    case SyntaxKind.EventDeclaration:
                        return ((EventDeclarationSyntax)member).Identifier.ValueText;
                    case SyntaxKind.EventFieldDeclaration:
                        return ((EventFieldDeclarationSyntax)member).Declaration?.Variables.FirstOrDefault()?.Identifier.ValueText;
                    case SyntaxKind.PropertyDeclaration:
                        return ((PropertyDeclarationSyntax)member).Identifier.ValueText;
                    case SyntaxKind.MethodDeclaration:
                        return ((MethodDeclarationSyntax)member).Identifier.ValueText;
                    case SyntaxKind.EnumDeclaration:
                        return ((EnumDeclarationSyntax)member).Identifier.ValueText;
                    case SyntaxKind.InterfaceDeclaration:
                        return ((InterfaceDeclarationSyntax)member).Identifier.ValueText;
                    case SyntaxKind.StructDeclaration:
                        return ((StructDeclarationSyntax)member).Identifier.ValueText;
                    case SyntaxKind.ClassDeclaration:
                        return ((ClassDeclarationSyntax)member).Identifier.ValueText;
                    case SyntaxKind.NamespaceDeclaration:
                        return ((NamespaceDeclarationSyntax)member).Name.ToString();
                    case SyntaxKind.DestructorDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                    case SyntaxKind.ConversionOperatorDeclaration:
                    case SyntaxKind.OperatorDeclaration:
                    case SyntaxKind.IncompleteMember:
                        return "";
                    default:
                        {
                            Debug.Fail($"unknown member '{member.Kind()}'");
                            return "";
                        }
                }
            }
        }
    }
}
