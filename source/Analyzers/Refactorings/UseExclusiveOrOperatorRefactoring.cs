// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseExclusiveOrOperatorRefactoring
    {
        public static void AnalyzeLogicalOrExpression(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (node.ContainsDiagnostics)
                return;

            if (node.SpanContainsDirectives())
                return;

            var logicalOr = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = logicalOr.Left.WalkDownParentheses();

            ExpressionSyntax right = logicalOr.Right.WalkDownParentheses();

            if (left.IsKind(SyntaxKind.LogicalAndExpression)
                && right.IsKind(SyntaxKind.LogicalAndExpression))
            {
                ExpressionPair expressions = GetExpressionPair((BinaryExpressionSyntax)left);

                if (expressions.IsValid)
                {
                    ExpressionPair expressions2 = GetExpressionPair((BinaryExpressionSyntax)right);

                    if (expressions2.IsValid
                        && (expressions.Expression.Kind() == expressions2.NegatedExpression.Kind()
                        && expressions.NegatedExpression.Kind() == expressions2.Expression.Kind()
                        && SyntaxComparer.AreEquivalent(expressions.Expression, expressions2.NegatedExpression)
                        && SyntaxComparer.AreEquivalent(expressions.NegatedExpression, expressions2.Expression)))
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.UseExclusiveOrOperator, logicalOr);
                    }
                }
            }
        }

        private static ExpressionPair GetExpressionPair(BinaryExpressionSyntax logicalAnd)
        {
            ExpressionSyntax left = logicalAnd.Left.WalkDownParentheses();

            if (left.IsKind(SyntaxKind.LogicalNotExpression))
            {
                ExpressionSyntax right = logicalAnd.Right.WalkDownParentheses();

                if (!right.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    var logicalNot = (PrefixUnaryExpressionSyntax)left;

                    return new ExpressionPair(right, logicalNot.Operand.WalkDownParentheses());
                }
            }
            else
            {
                ExpressionSyntax right = logicalAnd.Right.WalkDownParentheses();

                if (right.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    var logicalNot = (PrefixUnaryExpressionSyntax)right;

                    return new ExpressionPair(left, logicalNot.Operand.WalkDownParentheses());
                }
            }

            return default(ExpressionPair);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax logicalOr,
            CancellationToken cancellationToken)
        {
            var logicalAnd = (BinaryExpressionSyntax)logicalOr.Left.WalkDownParentheses();

            ExpressionSyntax left = logicalAnd.Left;
            ExpressionSyntax right = logicalAnd.Right;

            ExpressionSyntax newLeft = left.WalkDownParentheses();
            ExpressionSyntax newRight = right.WalkDownParentheses();

            if (newLeft.IsKind(SyntaxKind.LogicalNotExpression))
            {
                newLeft = ((PrefixUnaryExpressionSyntax)newLeft).Operand.WalkDownParentheses();
            }
            else if (newRight.IsKind(SyntaxKind.LogicalNotExpression))
            {
                newRight = ((PrefixUnaryExpressionSyntax)newRight).Operand.WalkDownParentheses();
            }

            ExpressionSyntax newNode = ExclusiveOrExpression(
                newLeft.WithTriviaFrom(left).Parenthesize(),
                CaretToken().WithTriviaFrom(logicalOr.OperatorToken),
                newRight.WithTriviaFrom(right).Parenthesize());

            newNode = newNode
                .WithTriviaFrom(logicalOr)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(logicalOr, newNode, cancellationToken);
        }

        private struct ExpressionPair
        {
            public ExpressionPair(ExpressionSyntax expression, ExpressionSyntax negatedExpression)
            {
                Expression = expression;
                NegatedExpression = negatedExpression;
            }

            public bool IsValid
            {
                get { return Expression != null && NegatedExpression != null; }
            }

            public ExpressionSyntax Expression { get; }
            public ExpressionSyntax NegatedExpression { get; }
        }
    }
}
