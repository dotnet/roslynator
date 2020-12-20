// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Analysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertBlockBodyToExpressionBodyRefactoring
    {
        public const string Title = "Use expression-bodied member";

        public static bool CanRefactor(SyntaxNode node)
        {
            switch (node)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    return CanRefactor(methodDeclaration);
                case PropertyDeclarationSyntax propertyDeclaration:
                    return CanRefactor(propertyDeclaration);
                case IndexerDeclarationSyntax indexerDeclaration:
                    return CanRefactor(indexerDeclaration);
                case OperatorDeclarationSyntax operatorDeclaration:
                    return CanRefactor(operatorDeclaration);
                case ConversionOperatorDeclarationSyntax conversionOperatorDeclaration:
                    return CanRefactor(conversionOperatorDeclaration);
                case ConstructorDeclarationSyntax constructorDeclaration:
                    return CanRefactor(constructorDeclaration);
                case DestructorDeclarationSyntax destructorDeclaration:
                    return CanRefactor(destructorDeclaration);
                case AccessorDeclarationSyntax accessorDeclaration:
                    return CanRefactor(accessorDeclaration);
                case LocalFunctionStatementSyntax localFunctionStatement:
                    return CanRefactor(localFunctionStatement);
            }

            return false;
        }

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
                    && BlockExpressionAnalysis.SupportsExpressionBody(accessor.Body, allowExpressionStatement: false))
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
                && BlockExpressionAnalysis.SupportsExpressionBody(methodDeclaration.Body);
        }

        public static bool CanRefactor(OperatorDeclarationSyntax operatorDeclaration, TextSpan? span = null)
        {
            return operatorDeclaration.Body != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(operatorDeclaration.Body) != false
                && BlockExpressionAnalysis.SupportsExpressionBody(operatorDeclaration.Body, allowExpressionStatement: false);
        }

        public static bool CanRefactor(ConversionOperatorDeclarationSyntax operatorDeclaration, TextSpan? span = null)
        {
            return operatorDeclaration.Body != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(operatorDeclaration.Body) != false
                && BlockExpressionAnalysis.SupportsExpressionBody(operatorDeclaration.Body, allowExpressionStatement: false);
        }

        public static bool CanRefactor(LocalFunctionStatementSyntax localFunctionStatement, TextSpan? span = null)
        {
            return localFunctionStatement.Body != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(localFunctionStatement.Body) != false
                && BlockExpressionAnalysis.SupportsExpressionBody(localFunctionStatement.Body);
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
                    && BlockExpressionAnalysis.SupportsExpressionBody(accessor.Body, allowExpressionStatement: false))
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
                && BlockExpressionAnalysis.SupportsExpressionBody(destructorDeclaration.Body);
        }

        public static bool CanRefactor(ConstructorDeclarationSyntax constructorDeclaration, TextSpan? span = null)
        {
            return constructorDeclaration.Body != null
                && span?.IsEmptyAndContainedInSpanOrBetweenSpans(constructorDeclaration.Body) != false
                && BlockExpressionAnalysis.SupportsExpressionBody(constructorDeclaration.Body);
        }

        public static bool CanRefactor(AccessorDeclarationSyntax accessorDeclaration, TextSpan? span = null)
        {
            BlockSyntax body = accessorDeclaration.Body;

            return body != null
                && (span?.IsEmptyAndContainedInSpanOrBetweenSpans(accessorDeclaration) != false
                    || span.Value.IsEmptyAndContainedInSpanOrBetweenSpans(body))
                && !accessorDeclaration.AttributeLists.Any()
                && BlockExpressionAnalysis.SupportsExpressionBody(body, allowExpressionStatement: !accessorDeclaration.IsKind(SyntaxKind.GetAccessorDeclaration))
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
                    member =>
                    {
                        if (CanRefactor(member))
                        {
                            var newMember = (MemberDeclarationSyntax)Refactor(member);

                            return newMember
                                .WithTrailingTrivia(member.GetTrailingTrivia())
                                .WithFormatterAnnotation();
                        }

                        return member;
                    });

            return document.ReplaceMembersAsync(SyntaxInfo.MemberDeclarationListInfo(selectedMembers.Parent), newMembers, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken = default)
        {
            SyntaxNode newNode = Refactor(node).WithFormatterAnnotation();

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        private static SyntaxNode Refactor(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;
                        BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(methodDeclaration.Body);

                        return methodDeclaration
                            .WithExpressionBody(CreateExpressionBody(analysis, methodDeclaration))
                            .WithSemicolonToken(CreateSemicolonToken(methodDeclaration.Body, analysis))
                            .WithBody(null);
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)node;
                        BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(constructorDeclaration.Body);

                        return constructorDeclaration
                            .WithExpressionBody(CreateExpressionBody(analysis, constructorDeclaration))
                            .WithSemicolonToken(CreateSemicolonToken(constructorDeclaration.Body, analysis))
                            .WithBody(null);
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var destructorDeclaration = (DestructorDeclarationSyntax)node;
                        BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(destructorDeclaration.Body);

                        return destructorDeclaration
                            .WithExpressionBody(CreateExpressionBody(analysis, destructorDeclaration))
                            .WithSemicolonToken(CreateSemicolonToken(destructorDeclaration.Body, analysis))
                            .WithBody(null);
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunction = (LocalFunctionStatementSyntax)node;
                        BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(localFunction.Body);

                        return localFunction
                            .WithExpressionBody(CreateExpressionBody(analysis, localFunction))
                            .WithSemicolonToken(CreateSemicolonToken(localFunction.Body, analysis))
                            .WithBody(null);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)node;
                        BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(operatorDeclaration.Body);

                        return operatorDeclaration
                            .WithExpressionBody(CreateExpressionBody(analysis, operatorDeclaration))
                            .WithSemicolonToken(CreateSemicolonToken(operatorDeclaration.Body, analysis))
                            .WithBody(null);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var operatorDeclaration = (ConversionOperatorDeclarationSyntax)node;
                        BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(operatorDeclaration.Body);

                        return operatorDeclaration
                            .WithExpressionBody(CreateExpressionBody(analysis, operatorDeclaration))
                            .WithSemicolonToken(CreateSemicolonToken(operatorDeclaration.Body, analysis))
                            .WithBody(null);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;
                        BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(propertyDeclaration.AccessorList);

                        return propertyDeclaration
                            .WithExpressionBody(CreateExpressionBody(analysis, propertyDeclaration))
                            .WithSemicolonToken(CreateSemicolonToken(analysis.Block, analysis))
                            .WithAccessorList(null);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;
                        BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(indexerDeclaration.AccessorList);

                        return indexerDeclaration
                            .WithExpressionBody(CreateExpressionBody(analysis, indexerDeclaration))
                            .WithSemicolonToken(CreateSemicolonToken(analysis.Block, analysis))
                            .WithAccessorList(null);
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.InitAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    {
                        var accessor = (AccessorDeclarationSyntax)node;
                        BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(accessor);

                        return accessor
                            .WithExpressionBody(CreateExpressionBody(analysis, accessor))
                            .WithSemicolonToken(CreateSemicolonToken(analysis.Block, analysis))
                            .WithBody(null);
                    }
                default:
                    {
                        Debug.Fail(node.Kind().ToString());
                        return node;
                    }
            }
        }

        private static ArrowExpressionClauseSyntax CreateExpressionBody(BlockExpressionAnalysis analysis, SyntaxNode declaration)
        {
            SyntaxToken arrowToken = Token(SyntaxKind.EqualsGreaterThanToken);

            ExpressionSyntax expression = analysis.Expression;

            SyntaxToken keyword = analysis.ReturnOrThrowKeyword;

            switch (keyword.Kind())
            {
                case SyntaxKind.ThrowKeyword:
                    {
                        expression = ThrowExpression(keyword, expression);
                        break;
                    }
                case SyntaxKind.ReturnKeyword:
                    {
                        SyntaxTriviaList leading = keyword.LeadingTrivia;

                        if (!leading.IsEmptyOrWhitespace())
                            arrowToken = arrowToken.WithLeadingTrivia(leading);

                        SyntaxTriviaList trailing = keyword.TrailingTrivia;

                        if (!trailing.IsEmptyOrWhitespace())
                            arrowToken = arrowToken.WithTrailingTrivia(trailing);

                        break;
                    }
            }

            expression = SyntaxTriviaAnalysis.SetIndentation(expression, declaration);

            return ArrowExpressionClause(arrowToken, expression);
        }

        private static SyntaxToken CreateSemicolonToken(BlockSyntax block, BlockExpressionAnalysis analysis)
        {
            SyntaxTriviaList trivia = analysis.SemicolonToken.TrailingTrivia;

            SyntaxTriviaList leading = block.CloseBraceToken.LeadingTrivia;

            if (!leading.IsEmptyOrWhitespace())
                trivia = trivia.AddRange(leading);

            SyntaxTriviaList trailing = block.CloseBraceToken.TrailingTrivia;

            if (!trailing.IsEmptyOrWhitespace()
                || !analysis.SemicolonToken.TrailingTrivia.LastOrDefault().IsEndOfLineTrivia())
            {
                trivia = trivia.AddRange(trailing);
            }

            return analysis.SemicolonToken.WithTrailingTrivia(trivia);
        }
    }
}
