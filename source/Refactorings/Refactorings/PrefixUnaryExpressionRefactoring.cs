// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class PrefixUnaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            SyntaxKind kind = prefixUnaryExpression.Kind();

            if (kind == SyntaxKind.PreIncrementExpression)
            {
                ReplacePreIncrementWithPostIncrement(context, prefixUnaryExpression);
                ReplacePreIncrementWithPreDecrement(context, prefixUnaryExpression);
            }
            else if (kind == SyntaxKind.PreDecrementExpression)
            {
                ReplacePreDecrementWithPostDecrement(context, prefixUnaryExpression);
                ReplacePreDecrementWithPreIncrement(context, prefixUnaryExpression);
            }
        }

        private static void ReplacePreIncrementWithPostIncrement(RefactoringContext context, PrefixUnaryExpressionSyntax preIncrement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePrefixOperatorWithPostfixOperator))
            {
                ExpressionSyntax operand = preIncrement.Operand;

                if (operand != null)
                {
                    PostfixUnaryExpressionSyntax postIncrement = PostIncrementExpression(operand.WithoutTrivia())
                        .WithTriviaFrom(preIncrement)
                        .WithFormatterAnnotation();

                    context.RegisterRefactoring(
                        $"Replace '{preIncrement}' with '{postIncrement}'",
                        cancellationToken => ChangePreIncrementToPostIncrementAsync(context.Document, preIncrement, postIncrement, cancellationToken));
                }
            }
        }

        private static void ReplacePreIncrementWithPreDecrement(RefactoringContext context, PrefixUnaryExpressionSyntax preIncrement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIncrementOperatorWithDecrementOperator))
            {
                PrefixUnaryExpressionSyntax preDecrement = preIncrement
                    .WithOperatorToken(MinusMinusToken())
                    .WithTriviaFrom(preIncrement)
                    .WithFormatterAnnotation();

                context.RegisterRefactoring(
                    $"Replace '{preIncrement}' with '{preDecrement}'",
                    cancellationToken => ChangePreIncrementToPreDecrementAsync(context.Document, preIncrement, preDecrement, cancellationToken));
            }
        }

        private static void ReplacePreDecrementWithPostDecrement(RefactoringContext context, PrefixUnaryExpressionSyntax preDecrement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePrefixOperatorWithPostfixOperator))
            {
                ExpressionSyntax operand = preDecrement.Operand;

                if (operand != null)
                {
                    PostfixUnaryExpressionSyntax postDecrement = PostDecrementExpression(operand.WithoutTrivia())
                        .WithTriviaFrom(preDecrement)
                        .WithFormatterAnnotation();

                    context.RegisterRefactoring(
                        $"Replace '{preDecrement}' with '{postDecrement}'",
                        cancellationToken => ChangePreDecrementToPostDecrementAsync(context.Document, preDecrement, postDecrement, cancellationToken));
                }
            }
        }

        private static void ReplacePreDecrementWithPreIncrement(RefactoringContext context, PrefixUnaryExpressionSyntax preDecrement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIncrementOperatorWithDecrementOperator))
            {
                PrefixUnaryExpressionSyntax preIncrement = preDecrement
                    .WithOperatorToken(PlusPlusToken())
                    .WithTriviaFrom(preDecrement)
                    .WithFormatterAnnotation();

                context.RegisterRefactoring(
                    $"Replace '{preDecrement}' with '{preIncrement}'",
                    cancellationToken => ChangePreDecrementToPreIncrementAsync(context.Document, preDecrement, preIncrement, cancellationToken));
            }
        }

        private static Task<Document> ChangePreIncrementToPostIncrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax preIncrement,
            PostfixUnaryExpressionSyntax postIncrement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(preIncrement, postIncrement, cancellationToken);
        }

        private static Task<Document> ChangePreIncrementToPreDecrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax preIncrement,
            PrefixUnaryExpressionSyntax preDecrement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(preIncrement, preDecrement, cancellationToken);
        }

        private static Task<Document> ChangePreDecrementToPostDecrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax preDecrement,
            PostfixUnaryExpressionSyntax postDecrement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(preDecrement, postDecrement, cancellationToken);
        }

        private static Task<Document> ChangePreDecrementToPreIncrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax preDecrement,
            PrefixUnaryExpressionSyntax preIncrement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(preDecrement, preIncrement, cancellationToken);
        }
    }
}