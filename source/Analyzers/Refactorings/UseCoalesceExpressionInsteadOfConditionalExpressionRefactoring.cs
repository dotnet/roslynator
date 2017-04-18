// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCoalesceExpressionInsteadOfConditionalExpressionRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            if (conditionalExpression.Condition?.IsMissing == false
                && CanBeReplacedWithCoalesceExpression(conditionalExpression)
                && conditionalExpression
                    .DescendantTrivia(conditionalExpression.Span)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseCoalesceExpressionInsteadOfConditionalExpression,
                    conditionalExpression);
            }
        }

        private static bool CanBeReplacedWithCoalesceExpression(ConditionalExpressionSyntax conditionalExpression)
        {
            ExpressionSyntax condition = conditionalExpression.Condition.WalkDownParentheses();

            if (condition.IsKind(SyntaxKind.EqualsExpression))
            {
                var binaryExpression = (BinaryExpressionSyntax)condition;

                if (binaryExpression.Left?.IsMissing == false
                    && binaryExpression.Right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
                {
                    return binaryExpression.Left.IsEquivalentTo(
                        conditionalExpression.WhenFalse.WalkDownParentheses(),
                        topLevel: false);
                }
            }
            else if (condition.IsKind(SyntaxKind.NotEqualsExpression))
            {
                var binaryExpression = (BinaryExpressionSyntax)condition;

                if (binaryExpression.Left?.IsMissing == false
                    && binaryExpression.Right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
                {
                    return binaryExpression.Left.IsEquivalentTo(
                        conditionalExpression.WhenTrue.WalkDownParentheses(),
                        topLevel: false);
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            var binaryExpression = (BinaryExpressionSyntax)conditionalExpression.Condition.WalkDownParentheses();

            ExpressionSyntax left = (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
                ? conditionalExpression.WhenFalse
                : conditionalExpression.WhenTrue;

            ExpressionSyntax right = (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
                ? conditionalExpression.WhenTrue
                : conditionalExpression.WhenFalse;

            BinaryExpressionSyntax newNode = CSharpFactory.CoalesceExpression(
                left.WithoutTrivia(),
                right.WithoutTrivia());

            return document.ReplaceNodeAsync(
                conditionalExpression,
                newNode.WithTriviaFrom(conditionalExpression),
                cancellationToken);
        }
    }
}
