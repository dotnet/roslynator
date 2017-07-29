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
                && IsFixable(conditionalExpression, context.SemanticModel, context.CancellationToken)
                && conditionalExpression
                    .DescendantTrivia(conditionalExpression.Span)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseCoalesceExpressionInsteadOfConditionalExpression,
                    conditionalExpression);
            }
        }

        private static bool IsFixable(ConditionalExpressionSyntax conditionalExpression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = conditionalExpression.Condition.WalkDownParentheses();

            switch (condition.Kind())
            {
                case SyntaxKind.EqualsExpression:
                    return IsFixable((BinaryExpressionSyntax)condition, conditionalExpression.WhenFalse, semanticModel, cancellationToken);
                case SyntaxKind.NotEqualsExpression:
                    return IsFixable((BinaryExpressionSyntax)condition, conditionalExpression.WhenTrue, semanticModel, cancellationToken);
                default:
                    return false;
            }
        }

        private static bool IsFixable(BinaryExpressionSyntax binaryExpression, ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;

            if (left?.IsMissing == false
                && binaryExpression.Right?.IsKind(SyntaxKind.NullLiteralExpression) == true
                && left.IsEquivalentTo(expression.WalkDownParentheses(), topLevel: false))
            {
                ITypeSymbol symbol = semanticModel.GetTypeSymbol(left, cancellationToken);

                if (symbol?.IsErrorType() == false)
                {
                    return symbol.IsReferenceType
                        || symbol.IsConstructedFrom(SpecialType.System_Nullable_T);
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
                left.WithoutTrivia().Parenthesize(),
                right.WithoutTrivia().Parenthesize());

            return document.ReplaceNodeAsync(
                conditionalExpression,
                newNode.WithTriviaFrom(conditionalExpression),
                cancellationToken);
        }
    }
}
