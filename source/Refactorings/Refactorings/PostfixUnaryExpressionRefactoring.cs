// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class PostfixUnaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, PostfixUnaryExpressionSyntax postfixUnaryExpression)
        {
            SyntaxKind kind = postfixUnaryExpression.Kind();

            if (kind == SyntaxKind.PostIncrementExpression)
            {
                ReplacePostIncrementWithPreIncrement(context, postfixUnaryExpression);
                ReplacePostIncrementWithPostDecrement(context, postfixUnaryExpression);
            }
            else if (kind == SyntaxKind.PostDecrementExpression)
            {
                ReplacePostDecrementWithPreDecrement(context, postfixUnaryExpression);
                ReplacePostDecrementWithPostIncrement(context, postfixUnaryExpression);
            }
        }

        private static void ReplacePostIncrementWithPreIncrement(RefactoringContext context, PostfixUnaryExpressionSyntax postIncrement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePrefixOperatorWithPostfixOperator))
            {
                ExpressionSyntax operand = postIncrement.Operand;

                if (operand != null)
                {
                    PrefixUnaryExpressionSyntax preIncrement = PreIncrementExpression(operand.WithoutTrivia())
                        .WithTriviaFrom(postIncrement)
                        .WithFormatterAnnotation();

                    context.RegisterRefactoring(
                        $"Replace '{postIncrement}' with '{preIncrement}'",
                        cancellationToken => ChangePostIncrementToPreIncrementAsync(context.Document, postIncrement, preIncrement, cancellationToken));
                }
            }
        }

        private static void ReplacePostIncrementWithPostDecrement(RefactoringContext context, PostfixUnaryExpressionSyntax postIncrement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIncrementOperatorWithDecrementOperator))
            {
                PostfixUnaryExpressionSyntax postDecrement = postIncrement
                    .WithOperatorToken(MinusMinusToken())
                    .WithTriviaFrom(postIncrement)
                    .WithFormatterAnnotation();

                context.RegisterRefactoring(
                    $"Replace '{postIncrement}' with '{postDecrement}'",
                    cancellationToken => ChangePostIncrementToPostDecrementAsync(context.Document, postIncrement, postDecrement, cancellationToken));
            }
        }

        private static void ReplacePostDecrementWithPreDecrement(RefactoringContext context, PostfixUnaryExpressionSyntax postDecrement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePrefixOperatorWithPostfixOperator))
            {
                ExpressionSyntax operand = postDecrement.Operand;

                if (operand != null)
                {
                    PrefixUnaryExpressionSyntax preDecrement = PreDecrementExpression(operand.WithoutTrivia())
                        .WithTriviaFrom(postDecrement)
                        .WithFormatterAnnotation();

                    context.RegisterRefactoring(
                        $"Replace '{postDecrement}' with '{preDecrement}'",
                        cancellationToken => ChangePostDecrementToPreDecrementAsync(context.Document, postDecrement, preDecrement, cancellationToken));
                }
            }
        }

        private static void ReplacePostDecrementWithPostIncrement(RefactoringContext context, PostfixUnaryExpressionSyntax postDecrement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIncrementOperatorWithDecrementOperator))
            {
                PostfixUnaryExpressionSyntax postIncrement = postDecrement
                    .WithOperatorToken(PlusPlusToken())
                    .WithTriviaFrom(postDecrement)
                    .WithFormatterAnnotation();

                context.RegisterRefactoring(
                    $"Replace '{postDecrement}' with '{postIncrement}'",
                    cancellationToken => ChangePostDecrementToPostIncrementAsync(context.Document, postDecrement, postIncrement, cancellationToken));
            }
        }

        private static Task<Document> ChangePostIncrementToPreIncrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postIncrement,
            PrefixUnaryExpressionSyntax preIncrement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(postIncrement, preIncrement, cancellationToken);
        }

        private static Task<Document> ChangePostIncrementToPostDecrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postIncrement,
            PostfixUnaryExpressionSyntax postDecrement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(postIncrement, postDecrement, cancellationToken);
        }

        private static Task<Document> ChangePostDecrementToPreDecrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postDecrement,
            PrefixUnaryExpressionSyntax preDecrement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(postDecrement, preDecrement, cancellationToken);
        }

        private static Task<Document> ChangePostDecrementToPostIncrementAsync(
            Document document,
            PostfixUnaryExpressionSyntax postDecrement,
            PostfixUnaryExpressionSyntax postIncrement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(postDecrement, postIncrement, cancellationToken);
        }
    }
}