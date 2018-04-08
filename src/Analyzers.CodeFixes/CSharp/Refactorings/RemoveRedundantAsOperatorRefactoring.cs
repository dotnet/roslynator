// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantAsOperatorRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            TextSpan span = TextSpan.FromBounds(left.Span.End, right.Span.End);
            IEnumerable<SyntaxTrivia> trivia = binaryExpression.DescendantTrivia(span);

            ExpressionSyntax newNode = left;

            if (trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newNode = newNode.WithTrailingTrivia(binaryExpression.GetTrailingTrivia());
            }
            else
            {
                newNode = newNode
                    .WithTrailingTrivia(trivia.Concat(binaryExpression.GetTrailingTrivia()))
                    .WithFormatterAnnotation();
            }

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }
    }
}
