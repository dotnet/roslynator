// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
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
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ExpressionSyntax newExpression = expression.Expression
                .WithTriviaFrom(expression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(expression, newExpression);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
