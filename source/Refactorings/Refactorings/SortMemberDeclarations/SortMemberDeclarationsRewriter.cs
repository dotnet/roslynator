// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxRewriters.SortMembers
{
    internal class SortMemberDeclarationsRewriter : CSharpSyntaxRewriter
    {
        private readonly IComparer<MemberDeclarationSyntax> _memberComparer;
        private readonly IComparer<EnumMemberDeclarationSyntax> _enumMemberComparer;

        public SortMemberDeclarationsRewriter(
            IComparer<MemberDeclarationSyntax> memberComparer,
            IComparer<EnumMemberDeclarationSyntax> enumMemberComparer)
        {
            _memberComparer = memberComparer ?? throw new ArgumentNullException(nameof(memberComparer));
            _enumMemberComparer = enumMemberComparer ?? throw new ArgumentNullException(nameof(enumMemberComparer));
        }

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            node = (NamespaceDeclarationSyntax)base.VisitNamespaceDeclaration(node);

            return node.WithMembers(SortMembers(node.Members));
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            node = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);

            return node.WithMembers(SortMembers(node.Members));
        }

        public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
        {
            node = (StructDeclarationSyntax)base.VisitStructDeclaration(node);

            return node.WithMembers(SortMembers(node.Members));
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            node = (InterfaceDeclarationSyntax)base.VisitInterfaceDeclaration(node);

            return node.WithMembers(SortMembers(node.Members));
        }

        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            node = (EnumDeclarationSyntax)base.VisitEnumDeclaration(node);

            return node.WithMembers(SortMembers(node.Members));
        }

        private SyntaxList<MemberDeclarationSyntax> SortMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            return members
                .OrderBy(f => f, _memberComparer)
                .ToSyntaxList();
        }

        private SeparatedSyntaxList<EnumMemberDeclarationSyntax> SortMembers(SeparatedSyntaxList<EnumMemberDeclarationSyntax> members)
        {
            return members
                .OrderBy(f => f, _enumMemberComparer)
                .ToSeparatedSyntaxList();
        }
    }
}
