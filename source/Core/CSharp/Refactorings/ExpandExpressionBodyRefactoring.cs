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
                            .WithBody(CreateBlock(expression, method));
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructor = (ConstructorDeclarationSyntax)node;

                        return constructor
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(Block(ExpressionStatement(expression)));
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var destructor = (DestructorDeclarationSyntax)node;

                        return destructor
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(Block(ExpressionStatement(expression)));
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        return ((OperatorDeclarationSyntax)node)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression));
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return ((ConversionOperatorDeclarationSyntax)node)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression));
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        return ((PropertyDeclarationSyntax)node)
                            .WithAccessorList(CreateAccessorList(expression))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        return ((IndexerDeclarationSyntax)node)
                            .WithAccessorList(CreateAccessorList(expression))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
                case SyntaxKind.GetAccessorDeclaration:
                    {
                        var accessor = (AccessorDeclarationSyntax)node;

                        return accessor
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression));
                    }
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    {
                        var accessor = (AccessorDeclarationSyntax)node;

                        return accessor
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(Block(ExpressionStatement(expression)));
                    }
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return node;
                    }
            }
        }

        private static BlockSyntax CreateBlock(ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.ThrowExpression))
            {
                return Block(ExpressionStatement(expression));
            }
            else
            {
                return Block(ReturnStatement(expression));
            }
        }

        private static BlockSyntax CreateBlock(ExpressionSyntax expression, MethodDeclarationSyntax methodDeclaration)
        {
            TypeSyntax returnType = methodDeclaration.ReturnType;

            if (returnType == null
                || returnType.IsVoid()
                || expression.IsKind(SyntaxKind.ThrowExpression))
            {
                return Block(ExpressionStatement(expression));
            }
            else
            {
                return Block(ReturnStatement(expression));
            }
        }

        private static AccessorListSyntax CreateAccessorList(ExpressionSyntax expression)
        {
            BlockSyntax block = Block(ReturnStatement(expression));

            AccessorListSyntax accessorList = AccessorList(GetAccessorDeclaration(block));

            if (expression.IsSingleLine())
            {
                return accessorList
                    .RemoveWhitespaceOrEndOfLineTrivia()
                    .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(NewLine()));
            }
            else
            {
                return accessorList;
            }
        }
    }
}
