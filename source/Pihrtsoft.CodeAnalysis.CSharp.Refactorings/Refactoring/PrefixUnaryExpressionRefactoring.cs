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
    internal static class PrefixUnaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            switch (prefixUnaryExpression.Kind())
            {
                case SyntaxKind.PreIncrementExpression:
                    {
                        PreIncrementToPostIncrement(context, prefixUnaryExpression);
                        PreIncrementToPreDecrement(context, prefixUnaryExpression);
                        break;
                    }
                case SyntaxKind.PreDecrementExpression:
                    {
                        PreDecrementToPostDecrement(context, prefixUnaryExpression);
                        PreDecrementToPreIncrement(context, prefixUnaryExpression);
                        break;
                    }
            }
        }

        private static void PreIncrementToPostIncrement(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            if (prefixUnaryExpression.Operand == null)
                return;

            context.RegisterRefactoring(
                "Convert to postfix operator",
                cancellationToken => ChangePreIncrementToPostIncrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
        }

        private static void PreIncrementToPreDecrement(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            context.RegisterRefactoring(
                "Convert to decrement operator",
                cancellationToken => ChangePreIncrementToPreDecrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
        }

        private static void PreDecrementToPostDecrement(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            if (prefixUnaryExpression.Operand == null)
                return;

            context.RegisterRefactoring(
                "Convert to postfix operator",
                cancellationToken => ChangePreDecrementToPostDecrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
        }

        private static void PreDecrementToPreIncrement(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            context.RegisterRefactoring(
                "Convert to increment operator",
                cancellationToken => ChangePreDecrementToPreIncrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
        }

        private static async Task<Document> ChangePreIncrementToPostIncrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax prefixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            PostfixUnaryExpressionSyntax newNode = PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, prefixUnaryExpression.Operand)
                .WithTriviaFrom(prefixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(prefixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> ChangePreIncrementToPreDecrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax prefixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            PrefixUnaryExpressionSyntax newNode = prefixUnaryExpression
                .WithOperatorToken(Token(SyntaxKind.MinusMinusToken))
                .WithTriviaFrom(prefixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(prefixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> ChangePreDecrementToPostDecrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax prefixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            PostfixUnaryExpressionSyntax newNode = PostfixUnaryExpression(SyntaxKind.PostDecrementExpression, prefixUnaryExpression.Operand)
                .WithTriviaFrom(prefixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(prefixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> ChangePreDecrementToPreIncrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax prefixUnaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            PrefixUnaryExpressionSyntax newNode = prefixUnaryExpression
                .WithOperatorToken(Token(SyntaxKind.PlusPlusToken))
                .WithTriviaFrom(prefixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(prefixUnaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }
    }
}