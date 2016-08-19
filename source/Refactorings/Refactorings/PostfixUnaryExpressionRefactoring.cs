// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class PostfixUnaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            switch (postfixUnaryExpression.Kind())
            {
                case SyntaxKind.PostIncrementExpression:
                    {
                        ReplacePostIncrementWithPreIncrement(context, postfixUnaryExpression);
                        ReplacePostIncrementWithPostDecrement(context, postfixUnaryExpression);
                        break;
                    }
                case SyntaxKind.PostDecrementExpression:
                    {
                        ReplacePostDecrementWithPreDecrement(context, postfixUnaryExpression);
                        ReplacePostDecrementWithPostIncrement(context, postfixUnaryExpression);
                        break;
                    }
            }
        }

        private static void ReplacePostIncrementWithPreIncrement(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePrefixOperatorWithPostfixOperator)
                && postfixUnaryExpression.Operand != null)
            {
                context.RegisterRefactoring(
                    "Replace with prefix operator",
                    cancellationToken => ChangePostIncrementToPreIncrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
            }
        }

        private static void ReplacePostIncrementWithPostDecrement(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIncrementOperatorWithDecrementOperator))
            {
                context.RegisterRefactoring(
                    "Replace with decrement operator",
                    cancellationToken => ChangePostIncrementToPostDecrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
            }
        }

        private static void ReplacePostDecrementWithPreDecrement(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePrefixOperatorWithPostfixOperator)
                && postfixUnaryExpression.Operand != null)
            {
                context.RegisterRefactoring(
                    "Replace with prefix operator",
                    cancellationToken => ChangePostDecrementToPreDecrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
            }
        }

        private static void ReplacePostDecrementWithPostIncrement(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIncrementOperatorWithDecrementOperator))
            {
                context.RegisterRefactoring(
                    "Replace with increment operator",
                    cancellationToken => ChangePostDecrementToPostIncrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
            }
        }

        private static async Task<Document> ChangePostIncrementToPreIncrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            PrefixUnaryExpressionSyntax newNode = PrefixUnaryExpression(SyntaxKind.PreIncrementExpression, postfixUnaryExpression.Operand)
                .WithTriviaFrom(postfixUnaryExpression)
                .WithFormatterAnnotation();

            root = root.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> ChangePostIncrementToPostDecrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            PostfixUnaryExpressionSyntax newNode = postfixUnaryExpression
                .WithOperatorToken(Token(SyntaxKind.MinusMinusToken))
                .WithTriviaFrom(postfixUnaryExpression)
                .WithFormatterAnnotation();

            root = root.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> ChangePostDecrementToPreDecrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            PrefixUnaryExpressionSyntax newNode = PrefixUnaryExpression(SyntaxKind.PreDecrementExpression, postfixUnaryExpression.Operand)
                .WithTriviaFrom(postfixUnaryExpression)
                .WithFormatterAnnotation();

            root = root.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> ChangePostDecrementToPostIncrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            PostfixUnaryExpressionSyntax newNode = postfixUnaryExpression
                .WithOperatorToken(Token(SyntaxKind.PlusPlusToken))
                .WithTriviaFrom(postfixUnaryExpression)
                .WithFormatterAnnotation();

            root = root.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }
    }
}