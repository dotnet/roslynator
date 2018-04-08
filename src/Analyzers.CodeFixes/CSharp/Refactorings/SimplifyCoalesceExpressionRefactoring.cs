// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyCoalesceExpressionRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxToken operatorToken = binaryExpression.OperatorToken;

            ExpressionSyntax newNode = null;

            if (expression == left)
            {
                IEnumerable<SyntaxTrivia> trivia = binaryExpression.DescendantTrivia(TextSpan.FromBounds(left.FullSpan.Start, operatorToken.FullSpan.End));

                newNode = right.WithLeadingTrivia(trivia);
            }
            else
            {
                IEnumerable<SyntaxTrivia> trivia = binaryExpression.DescendantTrivia(TextSpan.FromBounds(operatorToken.FullSpan.Start, right.FullSpan.End));

                newNode = left.WithTrailingTrivia(trivia);
            }

            newNode = newNode
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }
    }
}
