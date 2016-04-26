// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(PostfixUnaryExpressionCodeRefactoringProvider))]
    public class PostfixUnaryExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            PostfixUnaryExpressionSyntax postfixUnaryExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<PostfixUnaryExpressionSyntax>();

            if (postfixUnaryExpression == null)
                return;

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

        private static void PostIncrementToPreIncrement(CodeRefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            if (postfixUnaryExpression.Operand == null)
                return;

            context.RegisterRefactoring(
                "Convert to prefix operator",
                cancellationToken => ChangePostIncrementToPreIncrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
        }

        private static void PostIncrementToPostDecrement(CodeRefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            context.RegisterRefactoring(
                "Convert to decrement operator",
                cancellationToken => ChangePostIncrementToPostDecrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
        }

        private static void PostDecrementToPreDecrement(CodeRefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            if (postfixUnaryExpression.Operand == null)
                return;

            context.RegisterRefactoring(
                "Convert to prefix operator",
                cancellationToken => ChangePostDecrementToPreDecrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
        }

        private static void PostDecrementToPostIncrement(CodeRefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            context.RegisterRefactoring(
                "Convert to increment operator",
                cancellationToken => ChangePostDecrementToPostIncrementAsync(context.Document, postfixUnaryExpression, cancellationToken));
        }

        private static async Task<Document> ChangePostIncrementToPreIncrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PrefixUnaryExpressionSyntax newNode = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.PreIncrementExpression, postfixUnaryExpression.Operand)
                .WithTriviaFrom(postfixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ChangePostIncrementToPostDecrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PostfixUnaryExpressionSyntax newNode = postfixUnaryExpression
                .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.MinusMinusToken))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ChangePostDecrementToPreDecrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PrefixUnaryExpressionSyntax newNode = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.PreDecrementExpression, postfixUnaryExpression.Operand)
                .WithTriviaFrom(postfixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ChangePostDecrementToPostIncrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postfixUnaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PostfixUnaryExpressionSyntax newNode = postfixUnaryExpression
                .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.PlusPlusToken))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(postfixUnaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}