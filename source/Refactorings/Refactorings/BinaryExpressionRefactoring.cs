// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ExtractCondition;
using Roslynator.CSharp.Refactorings.ReplaceEqualsExpression;

namespace Roslynator.CSharp.Refactorings
{
    internal static class BinaryExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.NegateOperator))
            {
                SyntaxToken operatorToken = binaryExpression.OperatorToken;

                if (operatorToken.Span.Contains(context.Span)
                    && NegateOperatorRefactoring.CanBeNegated(operatorToken))
                {
                    context.RegisterRefactoring(
                        "Negate operator",
                        cancellationToken => NegateOperatorRefactoring.RefactorAsync(context.Document, operatorToken, cancellationToken));
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBooleanComparison)
                && binaryExpression.IsKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression))
            {
                ExpressionSyntax left = binaryExpression.Left;
                ExpressionSyntax right = binaryExpression.Right;

                if (left?.IsMissing == false
                    && right?.IsMissing == false)
                {
                    if (left.Span.Contains(context.Span))
                    {
                        await AddBooleanComparisonRefactoring.ComputeRefactoringAsync(context, left).ConfigureAwait(false);
                    }
                    else if (right.Span.Contains(context.Span))
                    {
                        await AddBooleanComparisonRefactoring.ComputeRefactoringAsync(context, right).ConfigureAwait(false);
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatBinaryExpression))
                FormatBinaryExpressionRefactoring.ComputeRefactorings(context, binaryExpression);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.NegateBinaryExpression))
                NegateBinaryExpressionRefactoring.ComputeRefactoring(context, binaryExpression);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandCoalesceExpression)
                && binaryExpression.OperatorToken.Span.Contains(context.Span))
            {
                ExpandCoalesceExpressionRefactoring.ComputeRefactoring(context, binaryExpression);
            }

            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.MergeStringLiterals,
                    RefactoringIdentifiers.MergeStringLiteralsIntoMultilineStringLiteral)
                && MergeStringLiteralsRefactoring.CanRefactor(context, binaryExpression))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeStringLiterals))
                {
                    context.RegisterRefactoring(
                        "Merge string literals",
                        cancellationToken => MergeStringLiteralsRefactoring.RefactorAsync(context.Document, binaryExpression, cancellationToken: cancellationToken));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeStringLiteralsIntoMultilineStringLiteral)
                    && binaryExpression
                        .DescendantTrivia(binaryExpression.Span)
                        .Any(f => f.IsKind(SyntaxKind.EndOfLineTrivia)))
                {
                    context.RegisterRefactoring(
                        "Merge string literals into multiline string literal",
                        cancellationToken => MergeStringLiteralsRefactoring.RefactorAsync(context.Document, binaryExpression, multiline: true, cancellationToken: cancellationToken));
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SwapExpressionsInBinaryExpression)
                && context.Span.IsBetweenSpans(binaryExpression))
            {
                SwapExpressionsInBinaryExpressionRefactoring.ComputeRefactoring(context, binaryExpression);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceAsWithCast)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(binaryExpression))
            {
                ReplaceAsWithCastRefactoring.ComputeRefactoring(context, binaryExpression);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.NegateIsExpression))
                NegateIsExpressionRefactoring.ComputeRefactoring(context, binaryExpression);

            if (context.Span.IsContainedInSpanOrBetweenSpans(binaryExpression.OperatorToken))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceEqualsExpressionWithStringEquals))
                    await ReplaceEqualsExpressionWithStringEqualsRefactoring.ComputeRefactoringAsync(context, binaryExpression).ConfigureAwait(false);

                if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ReplaceEqualsExpressionWithStringIsNullOrEmpty,
                    RefactoringIdentifiers.ReplaceEqualsExpressionWithStringIsNullOrWhiteSpace))
                {
                    await ReplaceEqualsExpressionRefactoring.ComputeRefactoringsAsync(context, binaryExpression).ConfigureAwait(false);
                }
            }

            if (!context.Span.IsBetweenSpans(binaryExpression)
                && context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractExpressionFromCondition))
            {
                SelectedExpressions selectedExpressions = SelectedExpressions.TryCreate(binaryExpression, context.Span);
#pragma warning disable RCS1061
                if (selectedExpressions?.Expressions.Length > 1)
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractExpressionFromCondition))
                        ExtractConditionRefactoring.ComputeRefactoring(context, selectedExpressions);
                }
#pragma warning restore RCS1061
            }
        }
    }
}
