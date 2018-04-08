// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
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
            if (!context.Span.IsEmpty)
                return;

            if (!memberAccessExpression.Span.Contains(context.Span))
                return;

            if (!memberAccessExpression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            List<MemberAccessExpressionSyntax> expressions = GetChain(memberAccessExpression, semanticModel, context.CancellationToken);

            if (expressions == null)
                return;

            if (expressions.Count <= 1)
                return;

            if (expressions[0].IsSingleLine(includeExteriorTrivia: false))
            {
                context.RegisterRefactoring(
                    "Format expression chain on multiple lines",
                    ct => SyntaxFormatter.ToMultiLineAsync(context.Document, expressions.ToArray(), ct));
            }
            else
            {
                context.RegisterRefactoring(
                    "Format expression chain on a single line",
                    ct => SyntaxFormatter.ToSingleLineAsync(context.Document, expressions[0], ct));
            }
        }

        private static List<MemberAccessExpressionSyntax> GetChain(
            MemberAccessExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            List<MemberAccessExpressionSyntax> expressions = null;

            expression = GetTopExpression(expression);

            while (expression != null)
            {
                (expressions ?? (expressions = new List<MemberAccessExpressionSyntax>())).Add(expression);

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

                        if (memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                            && memberAccess.Expression?.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                        {
                            ISymbol symbol = semanticModel.GetSymbol(memberAccess.Expression, cancellationToken);

                            if (symbol?.Kind == SymbolKind.Namespace)
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
                {
                    expression = parent;
                }
                else
                {
                    return expression;
                }
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

                        if (node?.Kind() == SyntaxKind.ElementAccessExpression)
                            node = node.Parent;

                        if (node?.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                            return (MemberAccessExpressionSyntax)node;

                        break;
                    }
                case SyntaxKind.ElementAccessExpression:
                    {
                        var elementAccess = (ElementAccessExpressionSyntax)expression.Parent;

                        if (elementAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                            return (MemberAccessExpressionSyntax)elementAccess.Parent;

                        break;
                    }
            }

            return null;
        }
    }
}
