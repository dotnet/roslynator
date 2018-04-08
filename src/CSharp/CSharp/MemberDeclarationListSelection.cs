// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    /// <summary>
    /// Represents selected member declarations in a <see cref="SyntaxList{MemberDeclarationSyntax}"/>.
    /// </summary>
    public sealed class MemberDeclarationListSelection : SyntaxListSelection<MemberDeclarationSyntax>
    {
        private MemberDeclarationListSelection(SyntaxNode parent, SyntaxList<MemberDeclarationSyntax> members, TextSpan span, SelectionResult result)
             : this(parent, members, span, result.FirstIndex, result.LastIndex)
        {
        }

        private MemberDeclarationListSelection(SyntaxNode parent, SyntaxList<MemberDeclarationSyntax> members, TextSpan span, int firstIndex, int lastIndex)
             : base(members, span, firstIndex, lastIndex)
        {
            Parent = parent;
        }

        /// <summary>
        /// Gets a node that contains selected members.
        /// </summary>
        public SyntaxNode Parent { get; }

        /// <summary>
        /// Creates a new <see cref="MemberDeclarationListSelection"/> based on the specified compilation unit and span.
        /// </summary>
        /// <param name="compilationUnit"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static MemberDeclarationListSelection Create(CompilationUnitSyntax compilationUnit, TextSpan span)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return Create(compilationUnit, compilationUnit.Members, span);
        }

        /// <summary>
        /// Creates a new <see cref="MemberDeclarationListSelection"/> based on the specified namespace declaration and span.
        /// </summary>
        /// <param name="namespaceDeclaration"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static MemberDeclarationListSelection Create(NamespaceDeclarationSyntax namespaceDeclaration, TextSpan span)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return Create(namespaceDeclaration, namespaceDeclaration.Members, span);
        }

        /// <summary>
        /// Creates a new <see cref="MemberDeclarationListSelection"/> based on the specified type declaration and span.
        /// </summary>
        /// <param name="typeDeclaration"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static MemberDeclarationListSelection Create(TypeDeclarationSyntax typeDeclaration, TextSpan span)
        {
            if (typeDeclaration == null)
                throw new ArgumentNullException(nameof(typeDeclaration));

            return Create(typeDeclaration, typeDeclaration.Members, span);
        }

        private static MemberDeclarationListSelection Create(SyntaxNode parent, SyntaxList<MemberDeclarationSyntax> members, TextSpan span)
        {
            SelectionResult result = SelectionResult.Create(members, span);

            return new MemberDeclarationListSelection(parent, members, span, result);
        }

        /// <summary>
        /// Creates a new <see cref="MemberDeclarationListSelection"/> based on the specified namespace declaration and span.
        /// </summary>
        /// <param name="namespaceDeclaration"></param>
        /// <param name="span"></param>
        /// <param name="selectedMembers"></param>
        /// <returns>True if the specified span contains at least one member; otherwise, false.</returns>
        public static bool TryCreate(NamespaceDeclarationSyntax namespaceDeclaration, TextSpan span, out MemberDeclarationListSelection selectedMembers)
        {
            selectedMembers = Create(namespaceDeclaration, span, 1, int.MaxValue);
            return selectedMembers != null;
        }

        internal static bool TryCreate(NamespaceDeclarationSyntax namespaceDeclaration, TextSpan span, int minCount, out MemberDeclarationListSelection selectedMembers)
        {
            selectedMembers = Create(namespaceDeclaration, span, minCount, int.MaxValue);
            return selectedMembers != null;
        }

        internal static bool TryCreate(NamespaceDeclarationSyntax namespaceDeclaration, TextSpan span, int minCount, int maxCount, out MemberDeclarationListSelection selectedMembers)
        {
            selectedMembers = Create(namespaceDeclaration, span, minCount, maxCount);
            return selectedMembers != null;
        }

        /// <summary>
        /// Creates a new <see cref="MemberDeclarationListSelection"/> based on the specified type declaration and span.
        /// </summary>
        /// <param name="typeDeclaration"></param>
        /// <param name="span"></param>
        /// <param name="selectedMembers"></param>
        /// <returns>True if the specified span contains at least one member; otherwise, false.</returns>
        public static bool TryCreate(TypeDeclarationSyntax typeDeclaration, TextSpan span, out MemberDeclarationListSelection selectedMembers)
        {
            selectedMembers = Create(typeDeclaration, span, 1, int.MaxValue);
            return selectedMembers != null;
        }

        internal static bool TryCreate(TypeDeclarationSyntax typeDeclaration, TextSpan span, int minCount, out MemberDeclarationListSelection selectedMembers)
        {
            selectedMembers = Create(typeDeclaration, span, minCount, int.MaxValue);
            return selectedMembers != null;
        }

        internal static bool TryCreate(TypeDeclarationSyntax typeDeclaration, TextSpan span, int minCount, int maxCount, out MemberDeclarationListSelection selectedMembers)
        {
            selectedMembers = Create(typeDeclaration, span, minCount, maxCount);
            return selectedMembers != null;
        }

        private static MemberDeclarationListSelection Create(NamespaceDeclarationSyntax declaration, TextSpan span, int minCount, int maxCount)
        {
            if (declaration == null)
                return null;

            return Create(declaration, declaration.Members, span, minCount, maxCount);
        }

        private static MemberDeclarationListSelection Create(TypeDeclarationSyntax declaration, TextSpan span, int minCount, int maxCount)
        {
            if (declaration == null)
                return null;

            return Create(declaration, declaration.Members, span, minCount, maxCount);
        }

        private static MemberDeclarationListSelection Create(MemberDeclarationSyntax declaration, SyntaxList<MemberDeclarationSyntax> members, TextSpan span, int minCount, int maxCount)
        {
            SelectionResult result = SelectionResult.Create(members, span, minCount, maxCount);

            if (!result.Success)
                return null;

            return new MemberDeclarationListSelection(declaration, members, span, result);
        }
    }
}
