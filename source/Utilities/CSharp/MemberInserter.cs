// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public static class MemberInserter
    {
        public static CompilationUnitSyntax InsertMember(CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithMembers(InsertMember(compilationUnit.Members, member));
        }

        public static int GetInsertIndex(CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return GetInsertIndex(compilationUnit.Members, member);
        }

        public static int GetInsertIndex(CompilationUnitSyntax compilationUnit, SyntaxKind memberKind)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return GetInsertIndex(compilationUnit.Members, memberKind);
        }

        public static int GetFieldInsertIndex(CompilationUnitSyntax compilationUnit, bool isConst = false)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return GetFieldInsertIndex(compilationUnit.Members, isConst);
        }

        public static NamespaceDeclarationSyntax InsertMember(NamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return namespaceDeclaration.WithMembers(InsertMember(namespaceDeclaration.Members, member));
        }

        public static int GetInsertIndex(NamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return GetInsertIndex(namespaceDeclaration.Members, member);
        }

        public static int GetInsertIndex(NamespaceDeclarationSyntax namespaceDeclaration, SyntaxKind memberKind)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return GetInsertIndex(namespaceDeclaration.Members, memberKind);
        }

        public static int GetFieldInsertIndex(NamespaceDeclarationSyntax namespaceDeclaration, bool isConst = false)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return GetFieldInsertIndex(namespaceDeclaration.Members, isConst);
        }

        public static ClassDeclarationSyntax InsertMember(ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithMembers(InsertMember(classDeclaration.Members, member));
        }

        public static int GetInsertIndex(ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return GetInsertIndex(classDeclaration.Members, member);
        }

        public static int GetInsertIndex(ClassDeclarationSyntax classDeclaration, SyntaxKind memberKind)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return GetInsertIndex(classDeclaration.Members, memberKind);
        }

        public static int GetFieldInsertIndex(ClassDeclarationSyntax classDeclaration, bool isConst = false)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return GetFieldInsertIndex(classDeclaration.Members, isConst);
        }

        public static StructDeclarationSyntax InsertMember(StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return structDeclaration.WithMembers(InsertMember(structDeclaration.Members, member));
        }

        public static int GetInsertIndex(StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return GetInsertIndex(structDeclaration.Members, member);
        }

        public static int GetInsertIndex(StructDeclarationSyntax structDeclaration, SyntaxKind memberKind)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return GetInsertIndex(structDeclaration.Members, memberKind);
        }

        public static int GetFieldInsertIndex(StructDeclarationSyntax structDeclaration, bool isConst = false)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return GetFieldInsertIndex(structDeclaration.Members, isConst);
        }

        public static InterfaceDeclarationSyntax InsertMember(InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return interfaceDeclaration.WithMembers(InsertMember(interfaceDeclaration.Members, member));
        }

        public static int GetInsertIndex(InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return GetInsertIndex(interfaceDeclaration.Members, member);
        }

        public static int GetInsertIndex(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxKind memberKind)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return GetInsertIndex(interfaceDeclaration.Members, memberKind);
        }

        public static int GetFieldInsertIndex(InterfaceDeclarationSyntax interfaceDeclaration, bool isConst = false)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return GetFieldInsertIndex(interfaceDeclaration.Members, isConst);
        }

        public static SyntaxList<MemberDeclarationSyntax> InsertMember(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax member)
        {
            return members.Insert(GetInsertIndex(members, member), member);
        }

        public static int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return GetInsertIndex(
                members,
                MemberDeclarationComparer.GetOrderIndex(member),
                member.Kind());
        }

        public static int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, SyntaxKind kind)
        {
            return GetInsertIndex(
                members,
                MemberDeclarationComparer.GetOrderIndex(kind),
                kind);
        }

        public static int GetFieldInsertIndex(SyntaxList<MemberDeclarationSyntax> members, bool isConst)
        {
            return GetInsertIndex(
                members,
                (isConst) ? 0 : 1,
                SyntaxKind.None);
        }

        private static int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, int orderIndex, SyntaxKind kind)
        {
            if (members.Any())
            {
                for (int i = orderIndex; i >= 0; i--)
                {
                    for (int j = members.Count - 1; j >= 0; j--)
                    {
                        if (IsMatch(members[j], kind, i))
                            return j + 1;
                    }
                }
            }

            return 0;
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
    }
}
