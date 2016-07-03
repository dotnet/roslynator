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
                        ReplacePreIncrementWithPostIncrement(context, prefixUnaryExpression);
                        ReplacePreIncrementWithPreDecrement(context, prefixUnaryExpression);
                        break;
                    }
                case SyntaxKind.PreDecrementExpression:
                    {
                        ReplacePreDecrementWithPostDecrement(context, prefixUnaryExpression);
                        ReplacePreDecrementWithPreIncrement(context, prefixUnaryExpression);
                        break;
                    }
            }
        }

        private static void ReplacePreIncrementWithPostIncrement(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePrefixOperatorWithPostfixOperator)
                && prefixUnaryExpression.Operand != null)
            {
                context.RegisterRefactoring(
                    "Replace with postfix operator",
                    cancellationToken => ChangePreIncrementToPostIncrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
            }
        }

        private static void ReplacePreIncrementWithPreDecrement(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIncrementOperatorWithDecrementOperator))
            {
                context.RegisterRefactoring(
                    "Replace with decrement operator",
                    cancellationToken => ChangePreIncrementToPreDecrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
            }
        }

        private static void ReplacePreDecrementWithPostDecrement(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePrefixOperatorWithPostfixOperator)
                && prefixUnaryExpression.Operand != null)
            {
                context.RegisterRefactoring(
                    "Replace with postfix operator",
                    cancellationToken => ChangePreDecrementToPostDecrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
            }
        }

        private static void ReplacePreDecrementWithPreIncrement(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIncrementOperatorWithDecrementOperator))
            {
                context.RegisterRefactoring(
                    "Replace with increment operator",
                    cancellationToken => ChangePreDecrementToPreIncrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
            }
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