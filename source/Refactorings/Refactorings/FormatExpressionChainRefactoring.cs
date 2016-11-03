// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatExpressionChainRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccessExpression)
        {
            if (context.Span.IsEmpty
                && memberAccessExpression.Span.Contains(context.Span)
                && memberAccessExpression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                List<MemberAccessExpressionSyntax> expressions = GetChain(memberAccessExpression, semanticModel, context.CancellationToken);

                if (expressions.Count > 1)
                {
                    if (expressions[0].IsSingleLine(includeExteriorTrivia: false))
                    {
                        context.RegisterRefactoring(
                            "Format expression chain on multiple lines",
                            cancellationToken =>
                            {
                                return FormatExpressionChainOnMultipleLinesAsync(
                                    context.Document,
                                    expressions.ToImmutableArray(),
                                    cancellationToken);
                            });
                    }
                    else
                    {
                        context.RegisterRefactoring(
                            "Format expression chain on a single line",
                            cancellationToken =>
                            {
                                return FormatExpressionChainOnSingleLineAsync(
                                    context.Document,
                                    expressions[0],
                                    cancellationToken);
                            });
                    }
                }
            }
        }

        private static async Task<Document> FormatExpressionChainOnMultipleLinesAsync(
            Document document,
            ImmutableArray<MemberAccessExpressionSyntax> expressions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            MemberAccessExpressionSyntax expression = expressions[0];

            SyntaxTriviaList triviaList = SyntaxUtility.GetIndentTrivia(expression).Add(CSharpFactory.IndentTrivia());

            triviaList = triviaList.Insert(0, CSharpFactory.NewLineTrivia());

            var rewriter = new ExpressionChainSyntaxRewriter(expressions, triviaList);

            SyntaxNode newNode = rewriter.Visit(expression)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(expression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> FormatExpressionChainOnSingleLineAsync(
            Document document,
            MemberAccessExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newNode = SyntaxRemover.RemoveWhitespaceOrEndOfLine(expression)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(expression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static List<MemberAccessExpressionSyntax> GetChain(
            MemberAccessExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var expressions = new List<MemberAccessExpressionSyntax>();

            expression = GetTopExpression(expression);

            while (expression != null)
            {
                expressions.Add(expression);

                expression = GetExpression(expression, semanticModel, cancellationToken);
            }

            return expressions;
        }

        private static MemberAccessExpressionSyntax GetExpression(
            MemberAccessExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
                switch (expression.Expression.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression.Expression;

                        if (memberAccess.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true
                            && memberAccess.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                        {
                            ISymbol symbol = semanticModel
                                .GetSymbolInfo(memberAccess.Expression, cancellationToken)
                                .Symbol;

                            if (symbol?.IsNamespace() == true)
                                return null;
                        }

                        return memberAccess;
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
            private readonly ImmutableArray<MemberAccessExpressionSyntax> _expressions;
            private readonly SyntaxTriviaList _triviaList;

            public ExpressionChainSyntaxRewriter(ImmutableArray<MemberAccessExpressionSyntax> expressions, SyntaxTriviaList triviaList)
            {
                _expressions = expressions;
                _triviaList = triviaList;
            }

            public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                var newNode = (MemberAccessExpressionSyntax)base.VisitMemberAccessExpression(node);

                if (_expressions.Contains(node)
                    && !node.OperatorToken.HasLeadingTrivia)
                {
                    return newNode.WithOperatorToken(node.OperatorToken.WithLeadingTrivia(_triviaList));
                }
                else
                {
                    return newNode;
                }
            }
        }
    }
}
