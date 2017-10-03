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

        public static bool CanRefactor(ConstructorDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && GetExpression(declaration.Body) != null;
        }

        public static bool CanRefactor(DestructorDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && GetExpression(declaration.Body) != null;
        }

        public static bool CanRefactor(LocalFunctionStatementSyntax localFunctionStatement)
        {
            if (localFunctionStatement == null)
                throw new ArgumentNullException(nameof(localFunctionStatement));

            return localFunctionStatement.ExpressionBody == null
                && GetExpression(localFunctionStatement.Body) != null;
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

        public static bool CanRefactor(AccessorDeclarationSyntax accessor)
        {
            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor));

            return accessor.ExpressionBody == null
                && !accessor.AttributeLists.Any()
                && GetExpression(accessor.Body) != null;
        }

        public static ExpressionSyntax GetReturnExpression(AccessorListSyntax accessorList)
        {
            if (accessorList != null)
            {
                SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

                if (accessors.Count == 1)
                {
                    AccessorDeclarationSyntax accessor = accessors[0];

                    if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration)
                        && !accessor.AttributeLists.Any())
                    {
                        return GetReturnExpression(accessor.Body);
                    }
                }
            }

            return default(ExpressionSyntax);
        }

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

        public static ExpressionSyntax GetReturnExpression(BlockSyntax block)
        {
            if (block != null)
            {
                StatementSyntax statement = block.Statements.SingleOrDefault(throwException: false);

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
                StatementSyntax statement = block.Statements.SingleOrDefault(throwException: false);

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
                StatementSyntax statement = block.Statements.SingleOrDefault(throwException: false);

                switch (statement?.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        return ((ReturnStatementSyntax)statement).Expression;
                    case SyntaxKind.ExpressionStatement:
                        return ((ExpressionStatementSyntax)statement).Expression;
                    case SyntaxKind.ThrowStatement:
                        return ThrowExpression(((ThrowStatementSyntax)statement).Expression);
                }
            }

            return default(ExpressionSyntax);
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
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpressionOrThrowExpression(constructorDeclaration.Body);

                        return constructorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var destructorDeclaration = (DestructorDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpressionOrThrowExpression(destructorDeclaration.Body);

                        return destructorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var local = (LocalFunctionStatementSyntax)node;
                        ExpressionSyntax expression = GetExpressionOrThrowExpression(local.Body);

                        return local
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
