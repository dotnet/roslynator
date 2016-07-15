// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class RemoveMemberDeclarationRefactoring
    {
        public static SyntaxRemoveOptions DefaultRemoveOptions { get; }
            = SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.KeepUnbalancedDirectives;

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            if (member.Parent.IsKind(SyntaxKind.CompilationUnit))
            {
                var compilationUnit = (CompilationUnitSyntax)member.Parent;

                root = root.ReplaceNode(compilationUnit, compilationUnit.RemoveMember(member));
            }
            else
            {
                var parentMember = (MemberDeclarationSyntax)member.Parent;

                if (parentMember != null)
                {
                    root = root.ReplaceNode(parentMember, parentMember.RemoveMember(member));
                }
                else
                {
                    root = root.RemoveNode(member, DefaultRemoveOptions);
                }
            }

            return document.WithSyntaxRoot(root);
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
    }
}
