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
            if (!conditionalExpression.ContainsDiagnostics
                && IsFixable(conditionalExpression)
                && conditionalExpression
                    .DescendantTrivia(conditionalExpression.Span)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseCoalesceExpressionInsteadOfConditionalExpression,
                    conditionalExpression);
            }
        }

        private static bool IsFixable(ConditionalExpressionSyntax conditionalExpression)
        {
            ExpressionSyntax condition = conditionalExpression.Condition.WalkDownParentheses();

            SyntaxKind kind = condition.Kind();

            if (kind == SyntaxKind.EqualsExpression)
            {
                var binaryExpression = (BinaryExpressionSyntax)condition;
                ExpressionSyntax left = binaryExpression.Left;

                if (left?.IsMissing == false
                    && binaryExpression.Right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
                {
                    return left.IsEquivalentTo(
                        conditionalExpression.WhenFalse.WalkDownParentheses(),
                        topLevel: false);
                }
            }
            else if (kind == SyntaxKind.NotEqualsExpression)
            {
                var binaryExpression = (BinaryExpressionSyntax)condition;
                ExpressionSyntax left = binaryExpression.Left;

                if (left?.IsMissing == false
                    && binaryExpression.Right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
                {
                    return left.IsEquivalentTo(
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
                left.WithoutTrivia().Parenthesize().WithSimplifierAnnotation(),
                right.WithoutTrivia().Parenthesize().WithSimplifierAnnotation());

            return document.ReplaceNodeAsync(
                conditionalExpression,
                newNode.WithTriviaFrom(conditionalExpression),
                cancellationToken);
        }
    }
}
