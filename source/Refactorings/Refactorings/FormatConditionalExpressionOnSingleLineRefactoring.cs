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
    internal static class FormatConditionalExpressionOnSingleLineRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await document.ReplaceNodeAsync(
                conditionalExpression,
                CreateSinglelineConditionalExpression(conditionalExpression),
                cancellationToken).ConfigureAwait(false);
        }

        private static ConditionalExpressionSyntax CreateSinglelineConditionalExpression(ConditionalExpressionSyntax conditionalExpression)
        {
            ExpressionSyntax condition = conditionalExpression.Condition;

            ParenthesizedExpressionSyntax newCondition = null;

            if (condition.IsKind(SyntaxKind.ParenthesizedExpression))
            {
                newCondition = (ParenthesizedExpressionSyntax)condition;
            }
            else
            {
                newCondition = ParenthesizedExpression(condition.WithoutTrailingTrivia())
                    .WithCloseParenToken(CreateTokenWithTrailingNewLine(SyntaxKind.CloseParenToken));
            }

            return ConditionalExpression(
                    newCondition.WithTrailingTrivia(Space),
                    QuestionToken().WithTrailingSpace(),
                    conditionalExpression.WhenTrue.WithTrailingTrivia(Space),
                    ColonToken().WithTrailingSpace(),
                    conditionalExpression.WhenFalse.WithoutTrailingTrivia());
        }

        private static SyntaxToken CreateTokenWithTrailingNewLine(SyntaxKind kind)
        {
            return Token(
                TriviaList(),
                kind,
                TriviaList(NewLineTrivia()));
        }
    }
}