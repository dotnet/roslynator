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
            SyntaxToken openParen = parenthesizedExpression.OpenParenToken;

            if (!openParen.IsMissing
                && openParen.Span.Contains(context.Span))
            {
                return true;
            }
            else
            {
                SyntaxToken closeParen = parenthesizedExpression.CloseParenToken;

                if (!closeParen.IsMissing
                    && closeParen.Span.Contains(context.Span))
                {
                    return true;
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ParenthesizedExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newExpression = expression.Expression
                .WithTriviaFrom(expression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(expression, newExpression, cancellationToken);
        }
    }
}
