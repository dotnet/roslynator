// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Comparers
{
    internal class MemberDeclarationComparer : IMemberDeclarationComparer
    {
        public static readonly MemberDeclarationComparer ByKind = new MemberDeclarationComparer(MemberDeclarationSortMode.ByKind);

        public static readonly MemberDeclarationComparer ByKindThenByName = new MemberDeclarationComparer(MemberDeclarationSortMode.ByKindThenByName);

        private const int _maxOrderIndex = 18;

        private MemberDeclarationComparer(MemberDeclarationSortMode sortMode = MemberDeclarationSortMode.ByKind)
        {
            SortMode = sortMode;
        }

        public MemberDeclarationSortMode SortMode { get; }

        public int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
        {
            return Compare(x, y, SortMode);
        }

        private static int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y, MemberDeclarationSortMode sortMode)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int result = GetOrderIndex(x).CompareTo(GetOrderIndex(y));

            if (sortMode == MemberDeclarationSortMode.ByKindThenByName
                && result == 0)
            {
                return string.Compare(GetName(x), GetName(y), StringComparison.CurrentCulture);
            }
            else
            {
                return result;
            }
        }

        public bool IsSorted(IEnumerable<MemberDeclarationSyntax> members)
        {
            return IsSorted(members, SortMode);
        }

        public static bool IsSorted(IEnumerable<MemberDeclarationSyntax> members, MemberDeclarationSortMode sortMode)
        {
            if (members == null)
                throw new ArgumentNullException(nameof(members));

            using (IEnumerator<MemberDeclarationSyntax> en = members.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    MemberDeclarationSyntax member1 = en.Current;

                    while (en.MoveNext())
                    {
                        MemberDeclarationSyntax member2 = en.Current;

                        if (Compare(member1, member2, sortMode) > 0)
                            return false;

                        member1 = member2;
                    }
                }
            }

            return true;
        }

        public int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return GetInsertIndex(members, GetOrderIndex(member));
        }

        public int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, SyntaxKind memberKind)
        {
            return GetInsertIndex(members, GetOrderIndex(memberKind));
        }

        public int GetFieldInsertIndex(SyntaxList<MemberDeclarationSyntax> members, bool isConst)
        {
            return GetInsertIndex(members, (isConst) ? 0 : 1);
        }

        private static int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, int orderIndex)
        {
            if (members.Any())
            {
                for (int i = orderIndex; i >= 0; i--)
                {
                    SyntaxKind kind = GetKind(i);

                    for (int j = members.Count - 1; j >= 0; j--)
                    {
                        if (IsMatch(members[j], kind, i))
                            return j + 1;
                    }
                }
            }

            return 0;
        }

        public static MemberDeclarationComparer GetInstance(MemberDeclarationSortMode sortMode)
        {
            switch (sortMode)
            {
                case MemberDeclarationSortMode.ByKind:
                    return ByKind;
                case MemberDeclarationSortMode.ByKindThenByName:
                    return ByKindThenByName;
                default:
                    throw new ArgumentException("", nameof(sortMode));
            }
        }

        private static bool IsMatch(MemberDeclarationSyntax memberDeclaration, SyntaxKind kind, int orderIndex)
        {
            switch (orderIndex)
            {
                case 0:
                    {
                        return memberDeclaration.IsKind(SyntaxKind.FieldDeclaration)
                           && ((FieldDeclarationSyntax)memberDeclaration).IsConst();
                    }
                case 1:
                    {
                        return memberDeclaration.IsKind(SyntaxKind.FieldDeclaration)
                           && !((FieldDeclarationSyntax)memberDeclaration).IsConst();
                    }
                default:
                    {
                        return memberDeclaration.IsKind(kind);
                    }
            }
        }

        private static int GetOrderIndex(MemberDeclarationSyntax memberDeclaration)
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
                        Debug.Fail($"unknown member '{memberDeclaration.Kind()}'");
                        return _maxOrderIndex;
                    }
            }
        }

        private static int GetOrderIndex(SyntaxKind kind)
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
                        Debug.Fail($"unknown member '{kind}'");
                        return _maxOrderIndex;
                    }
            }
        }

        private static SyntaxKind GetKind(int orderIndex)
        {
            switch (orderIndex)
            {
                case 1:
                    return SyntaxKind.FieldDeclaration;
                case 2:
                    return SyntaxKind.ConstructorDeclaration;
                case 3:
                    return SyntaxKind.DestructorDeclaration;
                case 4:
                    return SyntaxKind.DelegateDeclaration;
                case 5:
                    return SyntaxKind.EventDeclaration;
                case 6:
                    return SyntaxKind.EventFieldDeclaration;
                case 7:
                    return SyntaxKind.PropertyDeclaration;
                case 8:
                    return SyntaxKind.IndexerDeclaration;
                case 9:
                    return SyntaxKind.MethodDeclaration;
                case 10:
                    return SyntaxKind.ConversionOperatorDeclaration;
                case 11:
                    return SyntaxKind.OperatorDeclaration;
                case 12:
                    return SyntaxKind.EnumDeclaration;
                case 13:
                    return SyntaxKind.InterfaceDeclaration;
                case 14:
                    return SyntaxKind.StructDeclaration;
                case 15:
                    return SyntaxKind.ClassDeclaration;
                case 16:
                    return SyntaxKind.NamespaceDeclaration;
                case 17:
                    return SyntaxKind.IncompleteMember;
                default:
                    return SyntaxKind.None;
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

        public static bool CanBeSortedAlphabetically(SyntaxKind kind)
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
    }
}
