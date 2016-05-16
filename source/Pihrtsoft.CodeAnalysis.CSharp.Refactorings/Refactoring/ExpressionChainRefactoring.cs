// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    //TODO: add ElementAccessExpressionSyntax
    internal static class ExpressionChainRefactoring
    {
        public static void FormatExpressionChain(CodeRefactoringContext context, MemberAccessExpressionSyntax memberAccessExpression)
        {
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
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxTriviaList triviaList = expression.GetIndentTrivia().Add(SyntaxHelper.DefaultIndent);

            triviaList = triviaList.Insert(0, SyntaxHelper.NewLine);

            var rewriter = new ExpressionChainSyntaxRewriter(triviaList);

            SyntaxNode newNode = rewriter.Visit(expression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(expression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> FormatExpressionChainOnSingleLineAsync(
            Document document,
            MemberAccessExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newNode = RemoveWhitespaceOrEndOfLineSyntaxRewriter.VisitNode(expression)
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
                        if (expression.Parent.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                            return (MemberAccessExpressionSyntax)expression.Parent.Parent;

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
