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
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(PrefixUnaryExpressionCodeRefactoringProvider))]
    public class PrefixUnaryExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            PrefixUnaryExpressionSyntax prefixUnaryExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<PrefixUnaryExpressionSyntax>();

            if (prefixUnaryExpression == null)
                return;

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

        private static void PreIncrementToPostIncrement(CodeRefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            if (prefixUnaryExpression.Operand == null)
                return;

            context.RegisterRefactoring(
                "Convert to postfix operator",
                cancellationToken => ChangePreIncrementToPostIncrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
        }

        private static void PreIncrementToPreDecrement(CodeRefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            context.RegisterRefactoring(
                "Convert to decrement operator",
                cancellationToken => ChangePreIncrementToPreDecrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
        }

        private static void PreDecrementToPostDecrement(CodeRefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            if (prefixUnaryExpression.Operand == null)
                return;

            context.RegisterRefactoring(
                "Convert to postfix operator",
                cancellationToken => ChangePreDecrementToPostDecrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
        }

        private static void PreDecrementToPreIncrement(CodeRefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            context.RegisterRefactoring(
                "Convert to increment operator",
                cancellationToken => ChangePreDecrementToPreIncrementAsync(context.Document, prefixUnaryExpression, cancellationToken));
        }

        private static async Task<Document> ChangePreIncrementToPostIncrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax prefixUnaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PostfixUnaryExpressionSyntax newNode = SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, prefixUnaryExpression.Operand)
                .WithTriviaFrom(prefixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(prefixUnaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ChangePreIncrementToPreDecrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax prefixUnaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PrefixUnaryExpressionSyntax newNode = prefixUnaryExpression.WithOperatorToken(SyntaxFactory.Token(SyntaxKind.MinusMinusToken))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(prefixUnaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ChangePreDecrementToPostDecrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax prefixUnaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PostfixUnaryExpressionSyntax newNode = SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostDecrementExpression, prefixUnaryExpression.Operand)
                .WithTriviaFrom(prefixUnaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(prefixUnaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ChangePreDecrementToPreIncrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax prefixUnaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PrefixUnaryExpressionSyntax newNode = prefixUnaryExpression.WithOperatorToken(SyntaxFactory.Token(SyntaxKind.PlusPlusToken))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(prefixUnaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}