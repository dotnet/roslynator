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
    internal static class PrefixUnaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, PrefixUnaryExpressionSyntax prefixUnaryExpression)
        {
            if (!context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(prefixUnaryExpression.OperatorToken))
                return;

            switch (prefixUnaryExpression.Kind())
            {
                case SyntaxKind.PreIncrementExpression:
                    {
                        InvertPreIncrement(context, prefixUnaryExpression);
                        ReplacePreIncrementWithPostIncrement(context, prefixUnaryExpression);
                        break;
                    }
                case SyntaxKind.PreDecrementExpression:
                    {
                        InvertPreDecrement(context, prefixUnaryExpression);
                        ReplacePreDecrementWithPostDecrement(context, prefixUnaryExpression);
                        break;
                    }
            }
        }

        private static void ReplacePreIncrementWithPostIncrement(RefactoringContext context, PrefixUnaryExpressionSyntax preIncrement)
        {
            if (!context.IsRefactoringEnabled(RefactoringDescriptors.ReplacePrefixOperatorWithPostfixOperator))
                return;

            ExpressionSyntax operand = preIncrement.Operand;

            if (operand == null)
                return;

            PostfixUnaryExpressionSyntax postIncrement = PostIncrementExpression(operand.WithoutTrivia())
                .WithTriviaFrom(preIncrement)
                .WithFormatterAnnotation();

            context.RegisterRefactoring(
                $"Replace '{preIncrement}' with '{postIncrement}'",
                ct => ChangePreIncrementToPostIncrementAsync(context.Document, preIncrement, postIncrement, ct),
                RefactoringDescriptors.ReplacePrefixOperatorWithPostfixOperator);
        }

        private static void InvertPreIncrement(RefactoringContext context, PrefixUnaryExpressionSyntax preIncrement)
        {
            if (!context.IsRefactoringEnabled(RefactoringDescriptors.InvertPrefixOrPostfixUnaryOperator))
                return;

            PrefixUnaryExpressionSyntax preDecrement = preIncrement
                .WithOperatorToken(Token(SyntaxKind.MinusMinusToken))
                .WithTriviaFrom(preIncrement)
                .WithFormatterAnnotation();

            context.RegisterRefactoring(
                $"Invert {preIncrement.OperatorToken}",
                ct => ChangePreIncrementToPreDecrementAsync(context.Document, preIncrement, preDecrement, ct),
                RefactoringDescriptors.InvertPrefixOrPostfixUnaryOperator);
        }

        private static void ReplacePreDecrementWithPostDecrement(RefactoringContext context, PrefixUnaryExpressionSyntax preDecrement)
        {
            if (!context.IsRefactoringEnabled(RefactoringDescriptors.ReplacePrefixOperatorWithPostfixOperator))
                return;

            ExpressionSyntax operand = preDecrement.Operand;

            if (operand == null)
                return;

            PostfixUnaryExpressionSyntax postDecrement = PostDecrementExpression(operand.WithoutTrivia())
                .WithTriviaFrom(preDecrement)
                .WithFormatterAnnotation();

            context.RegisterRefactoring(
                $"Replace '{preDecrement}' with '{postDecrement}'",
                ct => ChangePreDecrementToPostDecrementAsync(context.Document, preDecrement, postDecrement, ct),
                RefactoringDescriptors.ReplacePrefixOperatorWithPostfixOperator);
        }

        private static void InvertPreDecrement(RefactoringContext context, PrefixUnaryExpressionSyntax preDecrement)
        {
            if (!context.IsRefactoringEnabled(RefactoringDescriptors.InvertPrefixOrPostfixUnaryOperator))
                return;

            PrefixUnaryExpressionSyntax preIncrement = preDecrement
                .WithOperatorToken(Token(SyntaxKind.PlusPlusToken))
                .WithTriviaFrom(preDecrement)
                .WithFormatterAnnotation();

            context.RegisterRefactoring(
                $"Invert {preDecrement.OperatorToken}",
                ct => ChangePreDecrementToPreIncrementAsync(context.Document, preDecrement, preIncrement, ct),
                RefactoringDescriptors.InvertPrefixOrPostfixUnaryOperator);
        }

        private static Task<Document> ChangePreIncrementToPostIncrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax preIncrement,
            PostfixUnaryExpressionSyntax postIncrement,
            CancellationToken cancellationToken = default)
        {
            return document.ReplaceNodeAsync(preIncrement, postIncrement, cancellationToken);
        }

        private static Task<Document> ChangePreIncrementToPreDecrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax preIncrement,
            PrefixUnaryExpressionSyntax preDecrement,
            CancellationToken cancellationToken = default)
        {
            return document.ReplaceNodeAsync(preIncrement, preDecrement, cancellationToken);
        }

        private static Task<Document> ChangePreDecrementToPostDecrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax preDecrement,
            PostfixUnaryExpressionSyntax postDecrement,
            CancellationToken cancellationToken = default)
        {
            return document.ReplaceNodeAsync(preDecrement, postDecrement, cancellationToken);
        }

        private static Task<Document> ChangePreDecrementToPreIncrementAsync(
            Document document,
            PrefixUnaryExpressionSyntax preDecrement,
            PrefixUnaryExpressionSyntax preIncrement,
            CancellationToken cancellationToken = default)
        {
            return document.ReplaceNodeAsync(preDecrement, preIncrement, cancellationToken);
        }
    }
}
