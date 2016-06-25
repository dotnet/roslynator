// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class PostfixUnaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            switch (postfixUnaryExpression.Kind())
            {
                case SyntaxKind.PostIncrementExpression:
                    {
                        PostIncrementToPreIncrement(context, postfixUnaryExpression);
                        PostIncrementToPostDecrement(context, postfixUnaryExpression);
                        break;
                    }
                case SyntaxKind.PostDecrementExpression:
                    {
                        PostDecrementToPreDecrement(context, postfixUnaryExpression);
                        PostDecrementToPostIncrement(context, postfixUnaryExpression);
                        break;
                    }
            }
        }

        private static void PostIncrementToPreIncrement(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            if (postfixUnaryExpression.Operand == null)
                return;

            context.RegisterRefactoring(
                "Convert to prefix operator",
                cancellationToken => ChangePostIncrementToPreIncrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
        }

        private static void PostIncrementToPostDecrement(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            context.RegisterRefactoring(
                "Convert to decrement operator",
                cancellationToken => ChangePostIncrementToPostDecrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
        }

        private static void PostDecrementToPreDecrement(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            if (postfixUnaryExpression.Operand == null)
                return;

            context.RegisterRefactoring(
                "Convert to prefix operator",
                cancellationToken => ChangePostDecrementToPreDecrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
        }

        private static void PostDecrementToPostIncrement(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            context.RegisterRefactoring(
                "Convert to increment operator",
                cancellationToken => ChangePostDecrementToPostIncrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
        }

        private static async Task<Document> ChangePostIncrementToPreIncrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            PrefixUnaryExpressionSyntax newNode = PrefixUnaryExpression(SyntaxKind.PreIncrementExpression, postfixUnaryExpression.Operand)
                .WithTriviaFrom(postfixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> ChangePostIncrementToPostDecrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            PostfixUnaryExpressionSyntax newNode = postfixUnaryExpression
                .WithOperatorToken(Token(SyntaxKind.MinusMinusToken))
                .WithTriviaFrom(postfixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> ChangePostDecrementToPreDecrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            PrefixUnaryExpressionSyntax newNode = PrefixUnaryExpression(SyntaxKind.PreDecrementExpression, postfixUnaryExpression.Operand)
                .WithTriviaFrom(postfixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> ChangePostDecrementToPostIncrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            PostfixUnaryExpressionSyntax newNode = postfixUnaryExpression
                .WithOperatorToken(Token(SyntaxKind.PlusPlusToken))
                .WithTriviaFrom(postfixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }
    }
}