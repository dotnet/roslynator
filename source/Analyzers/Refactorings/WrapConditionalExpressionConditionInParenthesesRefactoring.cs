// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class WrapConditionalExpressionConditionInParenthesesRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            ExpressionSyntax condition = conditionalExpression.Condition;

            if (condition?.IsKind(SyntaxKind.ParenthesizedExpression) == false)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.WrapConditionalExpressionConditionInParentheses,
                    condition.GetLocation());
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            ConditionalExpressionSyntax newNode = conditionalExpression
                .WithCondition(
                    SyntaxFactory.ParenthesizedExpression(
                        conditionalExpression.Condition.WithoutTrivia()
                    ).WithTriviaFrom(conditionalExpression.Condition)
                ).WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
