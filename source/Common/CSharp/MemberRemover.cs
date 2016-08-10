// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class MemberRemover
    {
        public static SyntaxRemoveOptions DefaultRemoveOptions { get; }
            = SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.KeepUnbalancedDirectives;

        public static async Task<Document> RemoveAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (member.Parent.IsKind(SyntaxKind.CompilationUnit))
            {
                var compilationUnit = (CompilationUnitSyntax)member.Parent;

                root = root.ReplaceNode(compilationUnit, compilationUnit.Remove(member));
            }
            else
            {
                var parentMember = (MemberDeclarationSyntax)member.Parent;

                if (parentMember != null)
                {
                    root = root.ReplaceNode(parentMember, parentMember.Remove(member));
                }
                else
                {
                    root = root.RemoveNode(member, DefaultRemoveOptions);
                }
            }

            return document.WithSyntaxRoot(root);
        }

        public static MemberDeclarationSyntax RemoveAt(this MemberDeclarationSyntax declaration, int index)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)declaration).RemoveAt(index);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).RemoveAt(index);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).RemoveAt(index);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).RemoveAt(index);
            }

            return declaration;
        }

        public static MemberDeclarationSyntax Remove(this MemberDeclarationSyntax declaration, MemberDeclarationSyntax member)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (declaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)declaration).Remove(member);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Remove(member);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Remove(member);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).Remove(member);
            }

            return declaration;
        }

        public static MemberDeclarationSyntax RemoveAt(this ClassDeclarationSyntax declaration, int index)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return Remove(declaration, declaration.Members[index], index);
        }

        public static MemberDeclarationSyntax Remove(this ClassDeclarationSyntax declaration, MemberDeclarationSyntax member)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return Remove(declaration, member, declaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax Remove(
            ClassDeclarationSyntax declaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            declaration = declaration
                .WithMembers(declaration.Members.Replace(member, newMember));

            return declaration
                .RemoveNode(declaration.Members[index], GetRemoveOptions(newMember));
        }

        public static MemberDeclarationSyntax RemoveAt(this InterfaceDeclarationSyntax declaration, int index)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return Remove(declaration, declaration.Members[index], index);
        }

        public static MemberDeclarationSyntax Remove(this InterfaceDeclarationSyntax declaration, MemberDeclarationSyntax member)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return Remove(declaration, member, declaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax Remove(
            InterfaceDeclarationSyntax declaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            declaration = declaration
                .WithMembers(declaration.Members.Replace(member, newMember));

            return declaration
                .RemoveNode(declaration.Members[index], GetRemoveOptions(newMember));
        }

        public static MemberDeclarationSyntax RemoveAt(this StructDeclarationSyntax declaration, int index)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return Remove(declaration, declaration.Members[index], index);
        }

        public static MemberDeclarationSyntax Remove(this StructDeclarationSyntax declaration, MemberDeclarationSyntax member)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return Remove(declaration, member, declaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax Remove(
            StructDeclarationSyntax declaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            declaration = declaration
                .WithMembers(declaration.Members.Replace(member, newMember));

            return declaration
                .RemoveNode(declaration.Members[index], GetRemoveOptions(newMember));
        }

        public static CompilationUnitSyntax RemoveAt(this CompilationUnitSyntax compilationUnit, int index)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return Remove(compilationUnit, compilationUnit.Members[index], index);
        }

        public static CompilationUnitSyntax Remove(this CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return Remove(compilationUnit, member, compilationUnit.Members.IndexOf(member));
        }

        private static CompilationUnitSyntax Remove(
            CompilationUnitSyntax compilationUnit,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            compilationUnit = compilationUnit
                .WithMembers(compilationUnit.Members.Replace(member, newMember));

            return compilationUnit
                .RemoveNode(compilationUnit.Members[index], GetRemoveOptions(newMember));
        }

        public static MemberDeclarationSyntax RemoveAt(this NamespaceDeclarationSyntax declaration, int index)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return Remove(declaration, declaration.Members[index], index);
        }

        public static MemberDeclarationSyntax Remove(this NamespaceDeclarationSyntax declaration, MemberDeclarationSyntax member)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return Remove(declaration, member, declaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax Remove(
            NamespaceDeclarationSyntax declaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            declaration = declaration
                .WithMembers(declaration.Members.Replace(member, newMember));

            return declaration
                .RemoveNode(declaration.Members[index], GetRemoveOptions(newMember));
        }

        internal static SyntaxRemoveOptions GetRemoveOptions(MemberDeclarationSyntax newMember)
        {
            SyntaxRemoveOptions removeOptions = DefaultRemoveOptions;

            if (newMember.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (newMember.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return removeOptions;
        }

        internal static MemberDeclarationSyntax RemoveSingleLineDocumentationComment(MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            SyntaxTriviaList leadingTrivia = memberDeclaration.GetLeadingTrivia();

            SyntaxTriviaList.Reversed.Enumerator en = leadingTrivia.Reverse().GetEnumerator();

            int i = 0;
            while (en.MoveNext())
            {
                if (en.Current.IsWhitespaceOrEndOfLineTrivia())
                {
                    i++;
                }
                else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    return memberDeclaration.WithLeadingTrivia(leadingTrivia.Take(leadingTrivia.Count - (i + 1)));
                }
                else
                {
                    return memberDeclaration;
                }
            }

            return memberDeclaration;
        }
    }
}
