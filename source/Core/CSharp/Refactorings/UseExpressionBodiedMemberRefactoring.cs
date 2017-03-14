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
    internal static class UseExpressionBodiedMemberRefactoring
    {
        public static bool CanRefactor(MethodDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && GetMethodExpression(declaration.Body) != null;
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

        public static bool CanRefactor(PropertyDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && GetReturnExpression(declaration.AccessorList) != null;
        }

        public static bool CanRefactor(IndexerDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && GetReturnExpression(declaration.AccessorList) != null;
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

        private static ExpressionSyntax GetReturnExpressionFast(AccessorListSyntax accessorList)
        {
            var returnStatement = (ReturnStatementSyntax)accessorList.Accessors[0].Body.Statements[0];

            return returnStatement.Expression;
        }

        public static ExpressionSyntax GetReturnExpression(BlockSyntax block)
        {
            if (block != null)
            {
                StatementSyntax statement = block.SingleStatementOrDefault();

                if (statement?.IsKind(SyntaxKind.ReturnStatement) == true)
                    return ((ReturnStatementSyntax)statement).Expression;
            }

            return default(ExpressionSyntax);
        }

        public static ExpressionSyntax GetMethodExpression(BlockSyntax block)
        {
            if (block != null)
            {
                StatementSyntax statement = block.SingleStatementOrDefault();

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
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            MemberDeclarationSyntax newMember = GetNewMember(member)
                .WithTrailingTrivia(member.GetTrailingTrivia())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(member, newMember, cancellationToken);
        }

        private static MemberDeclarationSyntax GetNewMember(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)declaration;
                        ExpressionSyntax expression = GetMethodExpression(methodDeclaration.Body);

                        return methodDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)declaration;
                        ExpressionSyntax expression = GetReturnExpression(operatorDeclaration.Body);

                        return operatorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var operatorDeclaration = (ConversionOperatorDeclarationSyntax)declaration;
                        ExpressionSyntax expression = GetReturnExpression(operatorDeclaration.Body);

                        return operatorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)declaration;
                        ExpressionSyntax expression = GetReturnExpressionFast(propertyDeclaration.AccessorList);

                        return propertyDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithAccessorList(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)declaration;
                        ExpressionSyntax expression = GetReturnExpressionFast(indexerDeclaration.AccessorList);

                        return indexerDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithAccessorList(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                default:
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return declaration;
                    }
            }
        }
    }
}
