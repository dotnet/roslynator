// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class PostfixUnaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            if (!context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(postfixUnaryExpression.OperatorToken))
                return;

            switch (postfixUnaryExpression.Kind())
            {
                case SyntaxKind.PostIncrementExpression:
                    {
                        InvertPostIncrement(context, postfixUnaryExpression);
                        ReplacePostIncrementWithPreIncrement(context, postfixUnaryExpression);
                        break;
                    }
                case SyntaxKind.PostDecrementExpression:
                    {
                        InvertPostDecrement(context, postfixUnaryExpression);
                        ReplacePostDecrementWithPreDecrement(context, postfixUnaryExpression);
                        break;
                    }
            }
        }

        private static void ReplacePostIncrementWithPreIncrement(RefactoringContext context, PostfixUnaryExpressionSyntax postIncrement)
        {
            if (!context.IsRefactoringEnabled(RefactoringDescriptors.ReplacePrefixOperatorWithPostfixOperator))
                return;

            ExpressionSyntax operand = postIncrement.Operand;

            if (operand == null)
                return;

            PrefixUnaryExpressionSyntax preIncrement = PreIncrementExpression(operand.WithoutTrivia())
                .WithTriviaFrom(postIncrement)
                .WithFormatterAnnotation();

            context.RegisterRefactoring(
                $"Replace '{postIncrement}' with '{preIncrement}'",
                ct => ChangePostIncrementToPreIncrementAsync(context.Document, postIncrement, preIncrement, ct),
                RefactoringDescriptors.ReplacePrefixOperatorWithPostfixOperator);
        }

        private static void InvertPostIncrement(RefactoringContext context, PostfixUnaryExpressionSyntax postIncrement)
        {
            if (!context.IsRefactoringEnabled(RefactoringDescriptors.InvertPrefixOrPostfixUnaryOperator))
                return;

            PostfixUnaryExpressionSyntax postDecrement = postIncrement
                .WithOperatorToken(Token(SyntaxKind.MinusMinusToken))
                .WithTriviaFrom(postIncrement)
                .WithFormatterAnnotation();

            context.RegisterRefactoring(
                $"Invert {postIncrement.OperatorToken}",
                ct => ChangePostIncrementToPostDecrementAsync(context.Document, postIncrement, postDecrement, ct),
                RefactoringDescriptors.InvertPrefixOrPostfixUnaryOperator);
        }

        private static void ReplacePostDecrementWithPreDecrement(RefactoringContext context, PostfixUnaryExpressionSyntax postDecrement)
        {
            if (!context.IsRefactoringEnabled(RefactoringDescriptors.ReplacePrefixOperatorWithPostfixOperator))
                return;

            ExpressionSyntax operand = postDecrement.Operand;

            if (operand == null)
                return;

            PrefixUnaryExpressionSyntax preDecrement = PreDecrementExpression(operand.WithoutTrivia())
                .WithTriviaFrom(postDecrement)
                .WithFormatterAnnotation();

            context.RegisterRefactoring(
                $"Replace '{postDecrement}' with '{preDecrement}'",
                ct => ChangePostDecrementToPreDecrementAsync(context.Document, postDecrement, preDecrement, ct),
                RefactoringDescriptors.ReplacePrefixOperatorWithPostfixOperator);
        }

        private static void InvertPostDecrement(RefactoringContext context, PostfixUnaryExpressionSyntax postDecrement)
        {
            if (!context.IsRefactoringEnabled(RefactoringDescriptors.InvertPrefixOrPostfixUnaryOperator))
                return;

            PostfixUnaryExpressionSyntax postIncrement = postDecrement
                .WithOperatorToken(Token(SyntaxKind.PlusPlusToken))
                .WithTriviaFrom(postDecrement)
                .WithFormatterAnnotation();

            context.RegisterRefactoring(
                $"Invert {postDecrement.OperatorToken}",
                ct => ChangePostDecrementToPostIncrementAsync(context.Document, postDecrement, postIncrement, ct),
                RefactoringDescriptors.InvertPrefixOrPostfixUnaryOperator);
        }

        private static Task<Document> ChangePostIncrementToPreIncrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postIncrement,
            PrefixUnaryExpressionSyntax preIncrement,
            CancellationToken cancellationToken = default)
        {
            return document.ReplaceNodeAsync(postIncrement, preIncrement, cancellationToken);
        }

        private static Task<Document> ChangePostIncrementToPostDecrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postIncrement,
            PostfixUnaryExpressionSyntax postDecrement,
            CancellationToken cancellationToken = default)
        {
            return document.ReplaceNodeAsync(postIncrement, postDecrement, cancellationToken);
        }

        private static Task<Document> ChangePostDecrementToPreDecrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postDecrement,
            PrefixUnaryExpressionSyntax preDecrement,
            CancellationToken cancellationToken = default)
        {
            return document.ReplaceNodeAsync(postDecrement, preDecrement, cancellationToken);
        }

        private static Task<Document> ChangePostDecrementToPostIncrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postDecrement,
            PostfixUnaryExpressionSyntax postIncrement,
            CancellationToken cancellationToken = default)
        {
            return document.ReplaceNodeAsync(postDecrement, postIncrement, cancellationToken);
        }
    }
}
