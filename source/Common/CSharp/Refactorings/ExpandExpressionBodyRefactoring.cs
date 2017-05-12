// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpandExpressionBodyRefactoring
    {
        public static bool CanRefactor(ArrowExpressionClauseSyntax arrowExpressionClause)
        {
            if (arrowExpressionClause == null)
                throw new ArgumentNullException(nameof(arrowExpressionClause));

            return arrowExpressionClause.Parent?.SupportsExpressionBody() == true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ArrowExpressionClauseSyntax arrowExpressionClause,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (arrowExpressionClause == null)
                throw new ArgumentNullException(nameof(arrowExpressionClause));

            SyntaxNode parent = arrowExpressionClause.Parent;

            SyntaxNode newNode = Refactor(parent, arrowExpressionClause.Expression).WithFormatterAnnotation();

            return document.ReplaceNodeAsync(parent, newNode, cancellationToken);
        }

        private static SyntaxNode Refactor(SyntaxNode node, ExpressionSyntax expression)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var method = (MethodDeclarationSyntax)node;

                        return method
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(method.ReturnType, expression, method.SemicolonToken));
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)node;

                        return operatorDeclaration
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression, operatorDeclaration.SemicolonToken));
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)node;

                        return conversionOperatorDeclaration
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression, conversionOperatorDeclaration.SemicolonToken));
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;

                        return propertyDeclaration
                            .WithAccessorList(CreateAccessorList(expression, propertyDeclaration.SemicolonToken))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;

                        return indexerDeclaration
                            .WithAccessorList(CreateAccessorList(expression, indexerDeclaration.SemicolonToken))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return node;
                    }
            }
        }

        private static BlockSyntax CreateBlock(ExpressionSyntax expression, SyntaxToken semicolon)
        {
            return CreateBlockWithReturnStatement(expression, semicolon);
        }

        private static BlockSyntax CreateBlock(TypeSyntax returnType, ExpressionSyntax expression, SyntaxToken semicolon)
        {
            if (returnType == null
                || returnType.IsVoid())
            {
                return CreateBlockWithExpressionStatement(expression, semicolon);
            }
            else
            {
                return CreateBlockWithReturnStatement(expression, semicolon);
            }
        }

        private static AccessorListSyntax CreateAccessorList(ExpressionSyntax expression, SyntaxToken semicolon)
        {
            BlockSyntax block = CreateBlockWithReturnStatement(expression, semicolon);

            AccessorListSyntax accessorList = AccessorList(GetAccessorDeclaration(block));

            if (expression.IsSingleLine())
            {
                accessorList = accessorList
                    .RemoveWhitespaceOrEndOfLineTrivia()
                    .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(NewLine()));
            }

            return accessorList;
        }

        private static BlockSyntax CreateBlockWithExpressionStatement(ExpressionSyntax expression, SyntaxToken semicolon)
        {
            return Block(ExpressionStatement(expression, semicolon));
        }

        private static BlockSyntax CreateBlockWithReturnStatement(ExpressionSyntax expression, SyntaxToken semicolon)
        {
            ReturnStatementSyntax returnStatement = ReturnStatement(
                ReturnKeyword().WithLeadingTrivia(expression.GetLeadingTrivia()),
                expression.WithoutLeadingTrivia(),
                semicolon);

            return Block(returnStatement);
        }
    }
}
