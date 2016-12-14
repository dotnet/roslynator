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
    internal static class FormatConditionalExpressionOnMultipleLinesRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await document.ReplaceNodeAsync(
                conditionalExpression,
                CreateMultilineConditionalExpression(conditionalExpression),
                cancellationToken).ConfigureAwait(false);
        }

        private static ConditionalExpressionSyntax CreateMultilineConditionalExpression(ConditionalExpressionSyntax conditionalExpression)
        {
            SyntaxTriviaList triviaList = SyntaxHelper.GetIndentTrivia(conditionalExpression.Parent)
                .Add(IndentTrivia())
                .Insert(0, NewLineTrivia());

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
                    newCondition.WithoutTrailingTrivia(),
                    CreateToken(SyntaxKind.QuestionToken, triviaList),
                    conditionalExpression.WhenTrue.WithoutTrailingTrivia(),
                    CreateToken(SyntaxKind.ColonToken, triviaList),
                    conditionalExpression.WhenFalse.WithoutTrailingTrivia());
        }

        private static SyntaxToken CreateToken(SyntaxKind kind, SyntaxTriviaList triviaList)
        {
            return Token(kind)
                .WithLeadingTrivia(triviaList)
                .WithTrailingSpace();
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