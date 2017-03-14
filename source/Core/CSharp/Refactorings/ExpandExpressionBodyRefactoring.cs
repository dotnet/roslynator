// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
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

            var parent = arrowExpressionClause.Parent as MemberDeclarationSyntax;

            return parent?.SupportsExpressionBody() == true;
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

            SyntaxNode member = arrowExpressionClause.Parent;

            SyntaxNode newMember = Refactor(member, arrowExpressionClause.Expression).WithFormatterAnnotation();

            return document.ReplaceNodeAsync(member, newMember, cancellationToken);
        }

        private static SyntaxNode Refactor(SyntaxNode member, ExpressionSyntax expression)
        {
            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var method = (MethodDeclarationSyntax)member;

                        return method
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression, method));
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        return ((OperatorDeclarationSyntax)member)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression));
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return ((ConversionOperatorDeclarationSyntax)member)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression));
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        return ((PropertyDeclarationSyntax)member)
                            .WithAccessorList(CreateAccessorList(expression))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        return ((IndexerDeclarationSyntax)member)
                            .WithAccessorList(CreateAccessorList(expression))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
                default:
                    {
                        Debug.Assert(false, member.Kind().ToString());
                        return member;
                    }
            }
        }

        private static BlockSyntax CreateBlock(ExpressionSyntax expression)
        {
            return Block(ReturnStatement(expression));
        }

        private static BlockSyntax CreateBlock(ExpressionSyntax expression, MethodDeclarationSyntax methodDeclaration)
        {
            TypeSyntax returnType = methodDeclaration.ReturnType;

            if (returnType == null || returnType.IsVoid())
            {
                return Block(ExpressionStatement(expression));
            }
            else
            {
                return CreateBlock(expression);
            }
        }

        private static AccessorListSyntax CreateAccessorList(ExpressionSyntax expression)
        {
            BlockSyntax block = CreateBlock(expression);

            AccessorListSyntax accessorList = AccessorList(GetAccessorDeclaration(block));

            if (expression.IsSingleLine())
            {
                accessorList = Remover.RemoveWhitespaceOrEndOfLine(accessorList)
                    .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(NewLineTrivia()));
            }

            return accessorList;
        }
    }
}
