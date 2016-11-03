// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpandExpressionBodiedMemberRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ArrowExpressionClauseSyntax arrowExpressionClause,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax newNode = ExpandExpressionBodiedMember(arrowExpressionClause.Parent, arrowExpressionClause.Expression)
                .WithFormatterAnnotation();

            root = root.ReplaceNode(arrowExpressionClause.Parent, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static MemberDeclarationSyntax ExpandExpressionBodiedMember(SyntaxNode member, ExpressionSyntax expression)
        {
            BlockSyntax block = Block(ReturnStatement(expression));

            switch (member.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    {
                        return ((PropertyDeclarationSyntax)member)
                            .WithAccessorList(CreateAccessorList(block, expression.IsSingleLine()))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        return ((MethodDeclarationSyntax)member)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(block);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        return ((OperatorDeclarationSyntax)member)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(block);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return ((ConversionOperatorDeclarationSyntax)member)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(block);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        return ((IndexerDeclarationSyntax)member)
                            .WithAccessorList(CreateAccessorList(block, expression.IsSingleLine()))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
            }

            return null;
        }

        private static AccessorListSyntax CreateAccessorList(BlockSyntax block, bool singleline)
        {
            AccessorListSyntax accessorList =
                AccessorList(
                    SingletonList(
                        AccessorDeclaration(
                            SyntaxKind.GetAccessorDeclaration,
                            block)));

            if (singleline)
            {
                accessorList = SyntaxRemover.RemoveWhitespaceOrEndOfLine(accessorList)
                    .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLineTrivia()));
            }

            return accessorList;
        }
    }
}
