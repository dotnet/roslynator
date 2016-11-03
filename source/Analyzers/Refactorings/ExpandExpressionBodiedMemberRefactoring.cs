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
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            BlockSyntax block = Block(ReturnStatement(arrowExpressionClause.Expression));

            MemberDeclarationSyntax newMemberDeclaration = GetNewDeclaration(arrowExpressionClause.Parent, block);

            SyntaxNode newRoot = oldRoot.ReplaceNode(arrowExpressionClause.Parent, newMemberDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static MemberDeclarationSyntax GetNewDeclaration(SyntaxNode memberDeclaration, BlockSyntax block)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    {
                        AccessorDeclarationSyntax accessorDeclaration = AccessorDeclaration(
                            SyntaxKind.GetAccessorDeclaration,
                            block);

                        return ((PropertyDeclarationSyntax)memberDeclaration)
                            .WithAccessorList(AccessorList(SingletonList(accessorDeclaration)))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        return ((MethodDeclarationSyntax)memberDeclaration)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(block);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        return ((OperatorDeclarationSyntax)memberDeclaration)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(block);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return ((ConversionOperatorDeclarationSyntax)memberDeclaration)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(block);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        AccessorDeclarationSyntax accessorDeclaration = AccessorDeclaration(
                            SyntaxKind.GetAccessorDeclaration,
                            block);

                        return ((IndexerDeclarationSyntax)memberDeclaration)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithAccessorList(AccessorList(SingletonList(accessorDeclaration)));
                    }
            }

            return null;
        }
    }
}
