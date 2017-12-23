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
    internal static class UseExpressionBodiedMemberRefactoring
    {
        public static bool CanRefactor(MethodDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && GetExpression(declaration.Body) != null;
        }

        public static bool CanRefactor(OperatorDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && GetReturnExpression(declaration.Body) != null;
        }

        public static bool CanRefactor(ConversionOperatorDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && GetReturnExpression(declaration.Body) != null;
        }

        private static ExpressionSyntax GetReturnExpression(AccessorListSyntax accessorList)
        {
            StatementSyntax statement = accessorList.Accessors[0].Body.Statements[0];

            if (statement is ReturnStatementSyntax returnStatement)
                return returnStatement.Expression;

            throw new InvalidOperationException();
        }

        public static ExpressionSyntax GetReturnExpression(BlockSyntax block)
        {
            if (block != null)
            {
                StatementSyntax statement = block.Statements.SingleOrDefault(shouldThrow: false);

                switch (statement?.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        return ((ReturnStatementSyntax)statement).Expression;
                    case SyntaxKind.ThrowStatement:
                        return ((ThrowStatementSyntax)statement).Expression;
                }
            }

            return default(ExpressionSyntax);
        }

        public static ExpressionSyntax GetExpression(BlockSyntax block)
        {
            if (block != null)
            {
                StatementSyntax statement = block.Statements.SingleOrDefault(shouldThrow: false);

                switch (statement?.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        return ((ReturnStatementSyntax)statement).Expression;
                    case SyntaxKind.ExpressionStatement:
                        return ((ExpressionStatementSyntax)statement).Expression;
                    case SyntaxKind.ThrowStatement:
                        return ((ThrowStatementSyntax)statement).Expression;
                }
            }

            return default(ExpressionSyntax);
        }

        public static ExpressionSyntax GetExpressionOrThrowExpression(BlockSyntax block)
        {
            if (block != null)
            {
                StatementSyntax statement = block.Statements.SingleOrDefault(shouldThrow: false);

                switch (statement?.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        return ((ReturnStatementSyntax)statement).Expression;
                    case SyntaxKind.ExpressionStatement:
                        return ((ExpressionStatementSyntax)statement).Expression;
                }
            }

            return default(ExpressionSyntax);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxNode newNode = GetNewNode(node)
                .WithTrailingTrivia(node.GetTrailingTrivia())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        private static SyntaxNode GetNewNode(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpressionOrThrowExpression(methodDeclaration.Body);

                        return methodDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpressionOrThrowExpression(operatorDeclaration.Body);

                        return operatorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var operatorDeclaration = (ConversionOperatorDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpressionOrThrowExpression(operatorDeclaration.Body);

                        return operatorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;
                        ExpressionSyntax expression = GetReturnExpression(propertyDeclaration.AccessorList);

                        return propertyDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithAccessorList(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;
                        ExpressionSyntax expression = GetReturnExpression(indexerDeclaration.AccessorList);

                        return indexerDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithAccessorList(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return node;
                    }
            }
        }
    }
}
