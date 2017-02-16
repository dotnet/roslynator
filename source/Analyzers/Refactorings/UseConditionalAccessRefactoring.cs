// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
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
                .WithLeadingTrivia(logicalAnd.GetLeadingTrivia())
                .WithFormatterAnnotation()
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

            if (expressionKind == SyntaxKind.LogicalNotExpression)
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)expression;
                ExpressionSyntax operand = logicalNot.Operand;
                SyntaxToken operatorToken = logicalNot.OperatorToken;

                string s = operand.ToFullString();

                int length = operand.GetLeadingTrivia().Span.Length + span.Length;

                var sb = new StringBuilder();
                sb.Append(s, 0, length);
                sb.Append("?");
                sb.Append(s, length, s.Length - length);
                sb.Append(" == false");

                return ParseExpression(sb.ToString());
            }
            else
            {
                string s = expression.ToFullString();

                int length = expression.GetLeadingTrivia().Span.Length + span.Length;

                var sb = new StringBuilder();
                sb.Append(s, 0, length);
                sb.Append("?");
                sb.Append(s, length, s.Length - length);

                switch (expressionKind)
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
                        break;
                    default:
                        {
                            sb.Append(" == true");
                            break;
                        }
                }

                return ParseExpression(sb.ToString());
            }
        }
    }
}
