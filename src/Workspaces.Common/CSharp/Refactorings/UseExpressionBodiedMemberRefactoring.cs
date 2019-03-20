// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Analysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseExpressionBodiedMemberRefactoring
    {
        public const string Title = "Use expression-bodied member";

        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration, TextSpan? span = null)
        {
            AccessorListSyntax accessorList = propertyDeclaration.AccessorList;

            if (accessorList != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(accessorList) != false)
            {
                AccessorDeclarationSyntax accessor = propertyDeclaration
                    .AccessorList?
                    .Accessors
                    .SingleOrDefault(shouldThrow: false);

                if (accessor?.AttributeLists.Any() == false
                        && accessor.IsKind(SyntaxKind.GetAccessorDeclaration)
                        && accessor.Body != null
                        && (UseExpressionBodiedMemberAnalysis.GetReturnExpression(accessor.Body) != null))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration, TextSpan? span = null)
        {
            return methodDeclaration.Body != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(methodDeclaration.Body) != false
                && UseExpressionBodiedMemberAnalysis.GetExpression(methodDeclaration.Body) != null;
        }

        public static bool CanRefactor(OperatorDeclarationSyntax operatorDeclaration, TextSpan? span = null)
        {
            return operatorDeclaration.Body != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(operatorDeclaration.Body) != false
                && UseExpressionBodiedMemberAnalysis.GetReturnExpression(operatorDeclaration.Body) != null;
        }

        public static bool CanRefactor(ConversionOperatorDeclarationSyntax operatorDeclaration, TextSpan? span = null)
        {
            return operatorDeclaration.Body != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(operatorDeclaration.Body) != false
                && UseExpressionBodiedMemberAnalysis.GetReturnExpression(operatorDeclaration.Body) != null;
        }

        public static bool CanRefactor(LocalFunctionStatementSyntax localFunctionStatement, TextSpan? span = null)
        {
            return localFunctionStatement.Body != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(localFunctionStatement.Body) != false
                && UseExpressionBodiedMemberAnalysis.GetExpression(localFunctionStatement.Body) != null;
        }

        public static bool CanRefactor(IndexerDeclarationSyntax indexerDeclaration, TextSpan? span = null)
        {
            AccessorListSyntax accessorList = indexerDeclaration.AccessorList;

            if (accessorList != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(accessorList) != false)
            {
                AccessorDeclarationSyntax accessor = indexerDeclaration
                    .AccessorList?
                    .Accessors
                    .SingleOrDefault(shouldThrow: false);

                if (accessor?.AttributeLists.Any() == false
                        && accessor.IsKind(SyntaxKind.GetAccessorDeclaration)
                        && accessor.Body != null
                        && (UseExpressionBodiedMemberAnalysis.GetReturnExpression(accessor.Body) != null))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanRefactor(DestructorDeclarationSyntax destructorDeclaration, TextSpan? span = null)
        {
            return destructorDeclaration.Body != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(destructorDeclaration.Body) != false
                && UseExpressionBodiedMemberAnalysis.GetExpression(destructorDeclaration.Body) != null;
        }

        public static bool CanRefactor(ConstructorDeclarationSyntax constructorDeclaration, TextSpan? span = null)
        {
            return constructorDeclaration.Body != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(constructorDeclaration.Body) != false
                && UseExpressionBodiedMemberAnalysis.GetExpression(constructorDeclaration.Body) != null;
        }

        public static bool CanRefactor(AccessorDeclarationSyntax accessorDeclaration, TextSpan? span = null)
        {
            BlockSyntax body = accessorDeclaration.Body;

            return body != null
                && (span == null
                    || span.Value.IsEmptyAndContainedInSpanOrBetweenSpans(accessorDeclaration)
                    || span.Value.IsEmptyAndContainedInSpanOrBetweenSpans(body))
                && !accessorDeclaration.AttributeLists.Any()
                && ((accessorDeclaration.IsKind(SyntaxKind.GetAccessorDeclaration))
                    ? UseExpressionBodiedMemberAnalysis.GetReturnExpression(body) != null
                    : UseExpressionBodiedMemberAnalysis.GetExpression(body) != null)
                && (accessorDeclaration.Parent as AccessorListSyntax)?
                    .Accessors
                    .SingleOrDefault(shouldThrow: false)?
                    .Kind() != SyntaxKind.GetAccessorDeclaration;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationListSelection selectedMembers,
            CancellationToken cancellationToken)
        {
            IEnumerable<MemberDeclarationSyntax> newMembers = selectedMembers
                .UnderlyingList
                .ModifyRange(
                    selectedMembers.FirstIndex,
                    selectedMembers.Count,
                    f => (MemberDeclarationSyntax)GetNewNode(f).WithTrailingTrivia(f.GetTrailingTrivia()).WithFormatterAnnotation());

            return document.ReplaceMembersAsync(SyntaxInfo.MemberDeclarationListInfo(selectedMembers.Parent), newMembers, cancellationToken);
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
                        ExpressionSyntax expression = GetExpression(methodDeclaration.Body);

                        return methodDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpression(constructorDeclaration.Body);

                        return constructorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var destructorDeclaration = (DestructorDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpression(destructorDeclaration.Body);

                        return destructorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var local = (LocalFunctionStatementSyntax)node;
                        ExpressionSyntax expression = GetExpression(local.Body);

                        return local
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpression(operatorDeclaration.Body);

                        return operatorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var operatorDeclaration = (ConversionOperatorDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpression(operatorDeclaration.Body);

                        return operatorDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpressionOrThrowExpression(propertyDeclaration.AccessorList);

                        return propertyDeclaration
                            .WithExpressionBody(ArrowExpressionClause(expression))
                            .WithAccessorList(null)
                            .WithSemicolonToken(SemicolonToken());
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;
                        ExpressionSyntax expression = GetExpressionOrThrowExpression(indexerDeclaration.AccessorList);

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

        private static ExpressionSyntax GetExpressionOrThrowExpression(AccessorListSyntax accessorList)
        {
            return GetExpressionOrThrowExpression(accessorList.Accessors[0]);
        }

        private static ExpressionSyntax GetExpressionOrThrowExpression(AccessorDeclarationSyntax accessor)
        {
            return GetExpression(accessor.Body);
        }

        private static ExpressionSyntax GetExpression(BlockSyntax block)
        {
            StatementSyntax statement = block.Statements[0];

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
    }
}
