// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractExpressionFromParenthesesRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            if (!parenthesizedExpression.OpenParenToken.IsMissing
                && parenthesizedExpression.OpenParenToken.Span.Contains(context.Span))
            {
                return true;
            }

            if (!parenthesizedExpression.CloseParenToken.IsMissing
                && parenthesizedExpression.CloseParenToken.Span.Contains(context.Span))
            {
                return true;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ParenthesizedExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax newExpression = expression.Expression
                .WithTriviaFrom(expression)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(expression, newExpression);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
