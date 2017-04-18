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
            if (CanRefactor(binaryExpression))
            {
                context.RegisterRefactoring(
                    "Expand ??",
                    cancellationToken => RefactorAsync(context.Document, binaryExpression, cancellationToken));
            }
        }

        public static bool CanRefactor(BinaryExpressionSyntax binaryExpression)
        {
            return binaryExpression.IsKind(SyntaxKind.CoalesceExpression)
                && binaryExpression.Left?.IsMissing == false
                && binaryExpression.Right?.IsMissing == false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax left = binaryExpression.Left.WithoutTrivia();
            ExpressionSyntax right = binaryExpression.Right.WithoutTrivia();

            SyntaxNode newNode = ConditionalExpression(
                ParenthesizedExpression(
                    NotEqualsExpression(
                        left,
                        NullLiteralExpression())),
                left,
                right);

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }
    }
}
