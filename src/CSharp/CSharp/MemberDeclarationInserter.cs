// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal abstract class MemberDeclarationInserter
    {
        public static MemberDeclarationInserter Default { get; } = new DefaultSyntaxInserter();

        public abstract int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax member);

        public abstract int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, SyntaxKind kind);

        /// <summary>
        /// Creates a new <see cref="ClassDeclarationSyntax"/> with the specified member inserted.
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public ClassDeclarationSyntax Insert(ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return classDeclaration.WithMembers(Insert(classDeclaration.Members, member));
        }

        /// <summary>
        /// Creates a new <see cref="CompilationUnitSyntax"/> with the specified member inserted.
        /// </summary>
        /// <param name="compilationUnit"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public CompilationUnitSyntax Insert(CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return compilationUnit.WithMembers(Insert(compilationUnit.Members, member));
        }

        /// <summary>
        /// Creates a new <see cref="InterfaceDeclarationSyntax"/> with the specified member inserted.
        /// </summary>
        /// <param name="interfaceDeclaration"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public InterfaceDeclarationSyntax Insert(InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return interfaceDeclaration.WithMembers(Insert(interfaceDeclaration.Members, member));
        }

        /// <summary>
        /// Creates a new <see cref="NamespaceDeclarationSyntax"/> with the specified member inserted.
        /// </summary>
        /// <param name="namespaceDeclaration"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public NamespaceDeclarationSyntax Insert(NamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return namespaceDeclaration.WithMembers(Insert(namespaceDeclaration.Members, member));
        }

        /// <summary>
        /// Creates a new <see cref="StructDeclarationSyntax"/> with the specified member inserted.
        /// </summary>
        /// <param name="structDeclaration"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public StructDeclarationSyntax Insert(StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return structDeclaration.WithMembers(Insert(structDeclaration.Members, member));
        }

        /// <summary>
        /// Creates a new <see cref="TypeDeclarationSyntax"/> with the specified member removed.
        /// </summary>
        /// <param name="typeDeclaration"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public TypeDeclarationSyntax Insert(TypeDeclarationSyntax typeDeclaration, MemberDeclarationSyntax member)
        {
            if (typeDeclaration == null)
                throw new ArgumentNullException(nameof(typeDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return typeDeclaration.WithMembers(Insert(typeDeclaration.Members, member));
        }

        /// <summary>
        /// Creates a new list with the specified node inserted.
        /// </summary>
        /// <param name="members"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        internal SyntaxList<MemberDeclarationSyntax> Insert(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax member)
        {
            int index = GetInsertIndex(members, member);

            return members.Insert(index, member);
        }

        private class DefaultSyntaxInserter : MemberDeclarationInserter
        {
            public override int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax member)
            {
                if (member == null)
                    throw new ArgumentNullException(nameof(member));

                int index = -1;

                for (int i = members.Count - 1; i >= 0; i--)
                {
                    int result = MemberDeclarationComparer.ByKind.Compare(members[i], member);

                    if (result == 0)
                    {
                        return i + 1;
                    }
                    else if (result < 0
                        && index == -1)
                    {
                        index = i + 1;
                    }
                }

                if (index == -1)
                    return 0;

                return index;
            }

            public override int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, SyntaxKind kind)
            {
                int index = -1;

                for (int i = members.Count - 1; i >= 0; i--)
                {
                    int result = MemberDeclarationKindComparer.Default.Compare(members[i].Kind(), kind);

                    if (result == 0)
                    {
                        return i + 1;
                    }
                    else if (result < 0
                        && index == -1)
                    {
                        index = i + 1;
                    }
                }

                if (index == -1)
                    return 0;

                return index;
            }
        }
    }
}
