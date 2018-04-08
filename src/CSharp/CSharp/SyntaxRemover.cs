// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.SyntaxRewriters;

namespace Roslynator.CSharp
{
    internal static class SyntaxRemover
    {
        public static SyntaxRemoveOptions DefaultRemoveOptions
        {
            get { return SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.KeepUnbalancedDirectives; }
        }

        public static TRoot RemoveNode<TRoot>(TRoot root, SyntaxNode node) where TRoot : SyntaxNode
        {
            return root.RemoveNode(node, GetRemoveOptions(node));
        }

        public static SyntaxRemoveOptions GetRemoveOptions(SyntaxNode node)
        {
            SyntaxRemoveOptions removeOptions = DefaultRemoveOptions;

            if (node.GetLeadingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (node.GetTrailingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return removeOptions;
        }

        public static SyntaxRemoveOptions GetRemoveOptions(CSharpSyntaxNode node)
        {
            SyntaxRemoveOptions removeOptions = DefaultRemoveOptions;

            if (node.GetLeadingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (node.GetTrailingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return removeOptions;
        }

        internal static MemberDeclarationSyntax RemoveSingleLineDocumentationComment(MemberDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            SyntaxTriviaList leadingTrivia = declaration.GetLeadingTrivia();

            SyntaxTriviaList.Reversed.Enumerator en = leadingTrivia.Reverse().GetEnumerator();

            int i = 0;
            while (en.MoveNext())
            {
                SyntaxKind kind = en.Current.Kind();

                if (kind == SyntaxKind.WhitespaceTrivia
                    || kind == SyntaxKind.EndOfLineTrivia)
                {
                    i++;
                }
                else if (kind == SyntaxKind.SingleLineDocumentationCommentTrivia)
                {
                    return declaration.WithLeadingTrivia(leadingTrivia.Take(leadingTrivia.Count - (i + 1)));
                }
                else
                {
                    return declaration;
                }
            }

            return declaration;
        }

        public static TNode RemoveComments<TNode>(TNode node, CommentKinds kinds) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return RemoveComments(node, node.FullSpan, kinds);
        }

        public static TNode RemoveComments<TNode>(TNode node, TextSpan span, CommentKinds kinds) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var remover = new CommentRemover(node, kinds, span);

            return (TNode)remover.Visit(node);
        }

        public static TNode RemoveTrivia<TNode>(TNode node, TextSpan? span = null) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return (TNode)TriviaRemover.GetInstance(span).Visit(node);
        }

        public static TNode RemoveWhitespace<TNode>(TNode node, TextSpan? span = null) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return (TNode)WhitespaceRemover.GetInstance(span).Visit(node);
        }

        public static ClassDeclarationSyntax RemoveMember(ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            int index = classDeclaration.Members.IndexOf(member);

            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            classDeclaration = classDeclaration.WithMembers(classDeclaration.Members.ReplaceAt(index, newMember));

            return classDeclaration.RemoveNode(classDeclaration.Members[index], GetRemoveOptions(newMember));
        }

        public static CompilationUnitSyntax RemoveMember(CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            int index = compilationUnit.Members.IndexOf(member);

            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            compilationUnit = compilationUnit.WithMembers(compilationUnit.Members.ReplaceAt(index, newMember));

            return compilationUnit.RemoveNode(compilationUnit.Members[index], GetRemoveOptions(newMember));
        }

        public static InterfaceDeclarationSyntax RemoveMember(InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            int index = interfaceDeclaration.Members.IndexOf(member);

            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            interfaceDeclaration = interfaceDeclaration.WithMembers(interfaceDeclaration.Members.ReplaceAt(index, newMember));

            return interfaceDeclaration.RemoveNode(interfaceDeclaration.Members[index], GetRemoveOptions(newMember));
        }

        public static NamespaceDeclarationSyntax RemoveMember(NamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            int index = namespaceDeclaration.Members.IndexOf(member);

            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            namespaceDeclaration = namespaceDeclaration.WithMembers(namespaceDeclaration.Members.ReplaceAt(index, newMember));

            return namespaceDeclaration.RemoveNode(namespaceDeclaration.Members[index], GetRemoveOptions(newMember));
        }

        public static StructDeclarationSyntax RemoveMember(StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            int index = structDeclaration.Members.IndexOf(member);

            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            structDeclaration = structDeclaration.WithMembers(structDeclaration.Members.ReplaceAt(index, newMember));

            return structDeclaration.RemoveNode(structDeclaration.Members[index], GetRemoveOptions(newMember));
        }

        public static TypeDeclarationSyntax RemoveMember(TypeDeclarationSyntax typeDeclaration, MemberDeclarationSyntax member)
        {
            if (typeDeclaration == null)
                throw new ArgumentNullException(nameof(typeDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            int index = typeDeclaration.Members.IndexOf(member);

            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            typeDeclaration = typeDeclaration.WithMembers(typeDeclaration.Members.ReplaceAt(index, newMember));

            return typeDeclaration.RemoveNode(typeDeclaration.Members[index], GetRemoveOptions(newMember));
        }
    }
}
