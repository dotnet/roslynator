// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                && binaryExpression.IsKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)
                && binaryExpression.Left?.IsMissing == false
                && binaryExpression.Right?.IsMissing == false
                && context.SupportsSemanticModel)
            {
                if (binaryExpression.Left.Span.Contains(context.Span))
                {
                    await AddBooleanComparisonRefactoring.ComputeRefactoringAsync(context, binaryExpression.Left).ConfigureAwait(false);
                }
                else if (binaryExpression.Right.Span.Contains(context.Span))
                {
                    await AddBooleanComparisonRefactoring.ComputeRefactoringAsync(context, binaryExpression.Right).ConfigureAwait(false);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatBinaryExpression))
                FormatBinaryExpressionRefactoring.ComputeRefactorings(context, binaryExpression);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.NegateBinaryExpression))
                NegateBinaryExpressionRefactoring.ComputeRefactoring(context, binaryExpression);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandCoalesceExpression)
                && binaryExpression.OperatorToken.Span.Contains(context.Span)
                && ExpandCoalesceExpressionRefactoring.CanRefactor(binaryExpression))
            {
                context.RegisterRefactoring(
                    "Expand ??",
                    cancellationToken =>
                    {
                        return ExpandCoalesceExpressionRefactoring.RefactorAsync(
                            context.Document,
                            binaryExpression,
                            cancellationToken);
                    });
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
                && SwapExpressionsRefactoring.CanRefactor(binaryExpression)
                && context.Span.IsBetweenSpans(binaryExpression))
            {
                context.RegisterRefactoring(
                    "Swap expressions",
                    cancellationToken =>
                    {
                        return SwapExpressionsRefactoring.RefactorAsync(
                            context.Document,
                            binaryExpression,
                            cancellationToken);
                    });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceAsWithCast))
                ReplaceAsWithCastRefactoring.ComputeRefactoring(context, binaryExpression);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceEqualsExpressionWithStringEquals)
                && context.Span.IsContainedInSpanOrBetweenSpans(binaryExpression.OperatorToken))
            {
                await ReplaceEqualsExpressionWithStringEqualsRefactoring.ComputeRefactoringAsync(context, binaryExpression).ConfigureAwait(false);
            }
        }
    }
}
