// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseExpressionBodiedMemberRefactoring
    {
        private static ExpressionSyntax GetReturnExpressionOrThrowExpression(AccessorListSyntax accessorList)
        {
            StatementSyntax statement = accessorList.Accessors[0].Body.Statements[0];

            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ThrowStatement:
                    return ThrowExpression(((ThrowStatementSyntax)statement).Expression);
                default:
                    throw new InvalidOperationException();
            }
        }

        private static ExpressionSyntax GetExpressionOrThrowExpression(AccessorDeclarationSyntax accessor)
        {
            StatementSyntax statement = accessor.Body.Statements[0];

            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)statement).Expression;
                case SyntaxKind.ThrowStatement:
                    return ThrowExpression(((ThrowStatementSyntax)statement).Expression);
                default:
                    throw new InvalidOperationException();
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
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
                        ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpressionOrThrowExpression(methodDeclaration.Body);

                        return methodDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)node;
                        ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpressionOrThrowExpression(constructorDeclaration.Body);

                        return constructorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var destructorDeclaration = (DestructorDeclarationSyntax)node;
                        ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpressionOrThrowExpression(destructorDeclaration.Body);

                        return destructorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var local = (LocalFunctionStatementSyntax)node;
                        ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpressionOrThrowExpression(local.Body);

                        return local
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)node;
                        ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpressionOrThrowExpression(operatorDeclaration.Body);

                        return operatorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var operatorDeclaration = (ConversionOperatorDeclarationSyntax)node;
                        ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpressionOrThrowExpression(operatorDeclaration.Body);

                        return operatorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;
                        ExpressionSyntax expression = GetReturnExpressionOrThrowExpression(propertyDeclaration.AccessorList);

                        return propertyDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithAccessorList(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;
                        ExpressionSyntax expression = GetReturnExpressionOrThrowExpression(indexerDeclaration.AccessorList);

                        return indexerDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithAccessorList(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    {
                        var accessor = (AccessorDeclarationSyntax)node;

                        ExpressionSyntax expression = GetExpressionOrThrowExpression(accessor);

                        return accessor
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                default:
                    {
                        Debug.Fail(node.Kind().ToString());
                        return node;
                    }
            }
        }
    }
}
