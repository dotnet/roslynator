// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ConditionalExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            if (conditionalExpression.IsSingleline())
            {
                context.RegisterRefactoring(
                    "Format conditional expression on multiple lines",
                    cancellationToken => FormatConditionalExpressionOnMultipleLinesAsync(context.Document, conditionalExpression, cancellationToken));
            }
            else
            {
                context.RegisterRefactoring(
                    "Format conditional expression on a single line",
                    cancellationToken => FormatConditionalExpressionOnSingleLineAsync(context.Document, conditionalExpression, cancellationToken));
            }

            ConditionalExpressionToIfElseRefactoring.Refactor(context, conditionalExpression);

            if (conditionalExpression.Condition != null
                && conditionalExpression.WhenTrue != null
                && conditionalExpression.WhenFalse != null
                && conditionalExpression.Condition.Span.Contains(context.Span))
            {
                context.RegisterRefactoring(
                    "Swap expressions in conditional expression",
                    cancellationToken =>
                    {
                        return SwapStatementsAsync(
                            context.Document,
                            conditionalExpression,
                            cancellationToken);
                    });
            }
        }

        private static async Task<Document> SwapStatementsAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ConditionalExpressionSyntax newConditionalExpression = conditionalExpression
                .WithCondition(conditionalExpression.Condition.Negate())
                .WithWhenTrue(conditionalExpression.WhenFalse.WithTriviaFrom(conditionalExpression.WhenTrue))
                .WithWhenFalse(conditionalExpression.WhenTrue.WithTriviaFrom(conditionalExpression.WhenFalse))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(conditionalExpression, newConditionalExpression);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> FormatConditionalExpressionOnMultipleLinesAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newRoot = oldRoot.ReplaceNode(conditionalExpression, CreateMultilineConditionalExpression(conditionalExpression));

            return document.WithSyntaxRoot(newRoot);
        }

        private static ConditionalExpressionSyntax CreateMultilineConditionalExpression(ConditionalExpressionSyntax conditionalExpression)
        {
            SyntaxTriviaList triviaList = conditionalExpression.Parent.GetIndentTrivia().Add(SyntaxHelper.DefaultIndent);

            triviaList = triviaList.Insert(0, SyntaxHelper.NewLine);

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
                    condition.WithoutTrailingTrivia(),
                    conditionalExpression.WhenTrue.WithoutTrailingTrivia(),
                    conditionalExpression.WhenFalse.WithoutTrailingTrivia())
                .WithQuestionToken(CreateToken(SyntaxKind.QuestionToken, triviaList))
                .WithColonToken(CreateToken(SyntaxKind.ColonToken, triviaList));
        }

        private static async Task<Document> FormatConditionalExpressionOnSingleLineAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

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
                TriviaList(SyntaxHelper.NewLine));
        }
    }
}