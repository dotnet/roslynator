// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceAsWithCastRefactoring
    {
        internal static void ComputeRefactoring(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression.IsKind(SyntaxKind.AsExpression)
                && binaryExpression.Left?.IsMissing == false
                && binaryExpression.Right is TypeSyntax)
            {
                context.RegisterRefactoring(
                    "Replace as with cast",
                    cancellationToken => RefactorAsync(context.Document, binaryExpression, context.CancellationToken));
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            CastExpressionSyntax castExpression = CastExpression((TypeSyntax)binaryExpression.Right, binaryExpression.Left)
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, castExpression, cancellationToken);
        }
    }
}