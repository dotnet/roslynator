// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpandCoalesceExpressionRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (!binaryExpression.IsKind(SyntaxKind.CoalesceExpression))
                return;

            if (binaryExpression.Left?.IsMissing != false)
                return;

            if (binaryExpression.Right?.IsMissing != false)
                return;

            context.RegisterRefactoring(
                "Expand ??",
                cancellationToken => RefactorAsync(context.Document, binaryExpression, cancellationToken));
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax left = binaryExpression.Left.WithoutTrivia();
            ExpressionSyntax right = binaryExpression.Right.WithoutTrivia();

            ExpressionSyntax expression = left.WalkDownParentheses();

            ExpressionSyntax condition = null;
            ExpressionSyntax whenTrue = null;

            if (expression.IsKind(SyntaxKind.AsExpression))
            {
                var asExpression = (BinaryExpressionSyntax)expression;

                condition = IsExpression(
                    asExpression.Left,
                    IsKeyword().WithTriviaFrom(asExpression.OperatorToken),
                    asExpression.Right);

                whenTrue = CastExpression((TypeSyntax)asExpression.Right, asExpression.Left.Parenthesize());
            }
            else
            {
                condition = NotEqualsExpression(left, NullLiteralExpression());
                whenTrue = left;
            }

            ConditionalExpressionSyntax conditionalExpression = ConditionalExpression(
                condition.ParenthesizeIf(!CSharpFacts.IsSingleTokenExpression(condition.Kind()), simplifiable: false),
                whenTrue.Parenthesize(),
                right.Parenthesize());

            conditionalExpression = conditionalExpression
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, conditionalExpression, cancellationToken);
        }
    }
}
