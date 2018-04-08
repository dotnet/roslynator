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
    internal static class RemoveRedundantCastRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            CastExpressionSyntax castExpression,
            CancellationToken cancellationToken)
        {
            var parenthesizedExpression = (ParenthesizedExpressionSyntax)castExpression.Parent;

            ParenthesizedExpressionSyntax newNode = parenthesizedExpression
                .WithExpression(castExpression.Expression.WithTriviaFrom(castExpression))
                .WithFormatterAnnotation()
                .WithSimplifierAnnotation();

            return document.ReplaceNodeAsync(parenthesizedExpression, newNode, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax)invocation.Expression;

            ExpressionSyntax expression = memberAccessExpression.Expression;

            IEnumerable<SyntaxTrivia> trailingTrivia = invocation
                .DescendantTrivia(TextSpan.FromBounds(expression.SpanStart, invocation.Span.End))
                .Where(f => !f.IsWhitespaceOrEndOfLineTrivia())
                .Concat(invocation.GetTrailingTrivia());

            ExpressionSyntax newNode = expression
                .WithTrailingTrivia(trailingTrivia)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
        }
    }
}
