// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseConditionalAccessRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax logicalAndExpression)
        {
            ExpressionSyntax expression = FindExpressionThanCanBeNull(logicalAndExpression);

            if (expression != null)
            {
                SyntaxNode node = FindConditionallyAccessedNode(expression, logicalAndExpression);

                if (node != null
                    && !node.SpanContainsDirectives())
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.UseConditionalAccess, logicalAndExpression);
                }
            }
        }

        private static ExpressionSyntax FindExpressionThanCanBeNull(BinaryExpressionSyntax logicalAndExpression)
        {
            ExpressionSyntax left = logicalAndExpression.Left;

            if (left?.IsKind(SyntaxKind.NotEqualsExpression) == true)
            {
                var notEquals = (BinaryExpressionSyntax)left;

                if (notEquals.Right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
                    return notEquals.Left;
            }

            return null;
        }

        private static SyntaxNode FindConditionallyAccessedNode(BinaryExpressionSyntax logicalAndExpression)
        {
            ExpressionSyntax expression = FindExpressionThanCanBeNull(logicalAndExpression);

            return FindConditionallyAccessedNode(expression, logicalAndExpression);
        }

        private static SyntaxNode FindConditionallyAccessedNode(ExpressionSyntax expression, BinaryExpressionSyntax logicalAndExpression)
        {
            ExpressionSyntax right = logicalAndExpression.Right;

            if (right?.IsMissing == false)
            {
                if (right.IsKind(SyntaxKind.LogicalNotExpression))
                    right = ((PrefixUnaryExpressionSyntax)right).Operand;

                SyntaxKind expressionKind = expression.Kind();

                SyntaxToken firstToken = right.GetFirstToken();

                int start = firstToken.SpanStart;

                SyntaxNode node = firstToken.Parent;

                while (node?.SpanStart == start)
                {
                    if (expressionKind == node.Kind()
                        && node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ElementAccessExpression)
                        && expression.IsEquivalentTo(node, topLevel: false))
                    {
                        return node;
                    }

                    node = node.Parent;
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax logicalAnd,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newNode = Refactor(logicalAnd)
                .Parenthesize(moveTrivia: true)
                .WithSimplifierAnnotation();

            return await document.ReplaceNodeAsync(logicalAnd, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static ExpressionSyntax Refactor(BinaryExpressionSyntax logicalAnd)
        {
            SyntaxNode node = FindConditionallyAccessedNode(logicalAnd);
            TextSpan span = node.Span;

            ExpressionSyntax expression = logicalAnd.Right;
            SyntaxKind expressionKind = expression.Kind();
            SyntaxTriviaList trailingTrivia = expression.GetTrailingTrivia();

            if (expressionKind == SyntaxKind.LogicalNotExpression)
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)expression;
                ExpressionSyntax operand = logicalNot.Operand;
                SyntaxToken operatorToken = logicalNot.OperatorToken;

                string s = operand.ToFullString();

                ExpressionSyntax newNode = ParseExpression(
                    s.Substring(0, span.Length) +
                    "?" +
                    s.Substring(span.Length, operand.Span.Length - span.Length - trailingTrivia.Span.Length) +
                    " == false");

                return newNode
                    .PrependToLeadingTrivia(logicalNot.GetLeadingAndTrailingTrivia())
                    .WithTrailingTrivia(trailingTrivia);
            }
            else
            {
                string s = expression.ToFullString();

                string text =
                    s.Substring(0, span.Length) +
                    "?" +
                    s.Substring(span.Length, expression.Span.Length - span.Length - trailingTrivia.Span.Length);

                if (AddBooleanComparison(expressionKind))
                    text += " == true";

                return ParseExpression(text).WithTrailingTrivia(trailingTrivia);
            }
        }

        private static bool AddBooleanComparison(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                    return false;
                default:
                    return true;
            }
        }
    }
}
