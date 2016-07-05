// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class FormatExpressionChainRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, MemberAccessExpressionSyntax memberAccessExpression)
        {
            if (!context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.FormatExpressionChain))
                return;

            if (!memberAccessExpression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                return;

            MemberAccessExpressionSyntax[] expressions = GetChain(memberAccessExpression).ToArray();

            if (expressions.Length <= 1)
                return;

            if (expressions[0].IsSingleline(includeExteriorTrivia: false))
            {
                context.RegisterRefactoring(
                    "Format expression chain on multiple lines",
                    cancellationToken => FormatExpressionChainOnMultipleLinesAsync(context.Document, expressions[0], cancellationToken));
            }
            else
            {
                context.RegisterRefactoring(
                    "Format expression chain on a single line",
                    cancellationToken => FormatExpressionChainOnSingleLineAsync(context.Document, expressions[0], cancellationToken));
            }
        }

        private static async Task<Document> FormatExpressionChainOnMultipleLinesAsync(
            Document document,
            MemberAccessExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxTriviaList triviaList = SyntaxHelper.GetIndentTrivia(expression).Add(CSharpFactory.IndentTrivia);

            triviaList = triviaList.Insert(0, CSharpFactory.NewLine);

            var rewriter = new ExpressionChainSyntaxRewriter(triviaList);

            SyntaxNode newNode = rewriter.Visit(expression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(expression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> FormatExpressionChainOnSingleLineAsync(
            Document document,
            MemberAccessExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newNode = WhitespaceOrEndOfLineRemover.RemoveFrom(expression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(expression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IEnumerable<MemberAccessExpressionSyntax> GetChain(MemberAccessExpressionSyntax expression)
        {
            expression = GetTopExpression(expression);

            while (expression != null)
            {
                yield return expression;

                expression = GetExpression(expression);
            }
        }

        private static MemberAccessExpressionSyntax GetExpression(MemberAccessExpressionSyntax expression)
        {
            switch (expression.Expression.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        return (MemberAccessExpressionSyntax)expression.Expression;
                    }
                case SyntaxKind.ElementAccessExpression:
                    {
                        var elementAccess = (ElementAccessExpressionSyntax)expression.Expression;

                        switch (elementAccess.Expression?.Kind())
                        {
                            case SyntaxKind.SimpleMemberAccessExpression:
                                {
                                    return (MemberAccessExpressionSyntax)elementAccess.Expression;
                                }
                            case SyntaxKind.InvocationExpression:
                                {
                                    var invocationExpression = (InvocationExpressionSyntax)elementAccess.Expression;
                                    if (invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                                        return (MemberAccessExpressionSyntax)invocationExpression.Expression;

                                    break;
                                }
                        }

                        break;
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        var invocationExpression = (InvocationExpressionSyntax)expression.Expression;
                        if (invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                            return (MemberAccessExpressionSyntax)invocationExpression.Expression;

                        break;
                    }
            }

            return null;
        }

        private static MemberAccessExpressionSyntax GetTopExpression(MemberAccessExpressionSyntax expression)
        {
            while (true)
            {
                MemberAccessExpressionSyntax parent = GetAncestor(expression);

                if (parent != null)
                    expression = parent;
                else
                    return expression;
            }
        }

        private static MemberAccessExpressionSyntax GetAncestor(MemberAccessExpressionSyntax expression)
        {
            switch (expression.Parent?.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        return (MemberAccessExpressionSyntax)expression.Parent;
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        SyntaxNode node = expression.Parent.Parent;

                        if (node?.IsKind(SyntaxKind.ElementAccessExpression) == true)
                            node = node.Parent;

                        if (node?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                            return (MemberAccessExpressionSyntax)node;

                        break;
                    }
                case SyntaxKind.ElementAccessExpression:
                    {
                        var elementAccess = (ElementAccessExpressionSyntax)expression.Parent;

                        if (elementAccess.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                            return (MemberAccessExpressionSyntax)elementAccess.Parent;

                        break;
                    }
            }

            return null;
        }

        private class ExpressionChainSyntaxRewriter : CSharpSyntaxRewriter
        {
            private MemberAccessExpressionSyntax _previous;
            private readonly SyntaxTriviaList _triviaList;

            public ExpressionChainSyntaxRewriter(SyntaxTriviaList triviaList)
            {
                _triviaList = triviaList;
            }

            public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                if (_previous == null || _previous.Equals(GetAncestor(node)))
                {
                    if (!node.OperatorToken.HasLeadingTrivia)
                        node = node.WithOperatorToken(node.OperatorToken.WithLeadingTrivia(_triviaList));

                    _previous = node;
                }

                return base.VisitMemberAccessExpression(node);
            }
        }
    }
}
