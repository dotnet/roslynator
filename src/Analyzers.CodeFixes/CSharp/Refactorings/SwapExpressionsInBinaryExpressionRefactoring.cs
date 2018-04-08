// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwapExpressionsInBinaryExpressionRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            BinaryExpressionSyntax newBinaryExpression = binaryExpression
                .WithLeft(right.WithTriviaFrom(left))
                .WithRight(left.WithTriviaFrom(right));

            SyntaxNode newRoot = root.ReplaceNode(binaryExpression, newBinaryExpression);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}