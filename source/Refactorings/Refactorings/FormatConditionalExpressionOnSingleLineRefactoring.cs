// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatConditionalExpressionOnSingleLineRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = oldRoot.ReplaceNode(conditionalExpression, CreateSinglelineConditionalExpression(conditionalExpression));

            return document.WithSyntaxRoot(newRoot);
        }

        private static ConditionalExpressionSyntax CreateSinglelineConditionalExpression(ConditionalExpressionSyntax conditionalExpression)
        {
            ParenthesizedExpressionSyntax condition = null;
            if (conditionalExpression.Condition.IsKind(SyntaxKind.ParenthesizedExpression))
            {
                condition = (ParenthesizedExpressionSyntax)conditionalExpression.Condition;
            }
            else
            {
                condition = ParenthesizedExpression(conditionalExpression.Condition.WithoutTrailingTrivia())
                    .WithCloseParenToken(CreateTokenWithTrailingNewLine(SyntaxKind.CloseParenToken));
            }

            return ConditionalExpression(
                    condition.WithTrailingTrivia(Space),
                    conditionalExpression.WhenTrue.WithTrailingTrivia(Space),
                    conditionalExpression.WhenFalse.WithoutTrailingTrivia())
                .WithQuestionToken(Token(SyntaxKind.QuestionToken).WithTrailingSpace())
                .WithColonToken(Token(SyntaxKind.ColonToken).WithTrailingSpace());
        }

        private static SyntaxToken CreateTokenWithTrailingNewLine(SyntaxKind kind)
        {
            return Token(
                TriviaList(),
                kind,
                TriviaList(CSharpFactory.NewLineTrivia()));
        }
    }
}