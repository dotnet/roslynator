// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
                    RefactoringIdentifiers.JoinStringExpressions,
                    RefactoringIdentifiers.UseStringBuilderInsteadOfConcatenation)
                && context.Span.IsBetweenSpans(binaryExpression)
                && binaryExpression.IsKind(SyntaxKind.AddExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                StringConcatenationExpression concatenation;
                if (StringConcatenationExpression.TryCreate(binaryExpression, semanticModel, context.CancellationToken, out concatenation))
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.JoinStringExpressions))
                        JoinStringExpressionsRefactoring.ComputeRefactoring(context, concatenation);

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseStringBuilderInsteadOfConcatenation))
                        UseStringBuilderInsteadOfConcatenationRefactoring.ComputeRefactoring(context, concatenation);
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
                && context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ExtractExpressionFromCondition,
                    RefactoringIdentifiers.JoinStringExpressions))
            {
                BinaryExpressionSelection binaryExpressionSelection = BinaryExpressionSelection.Create(binaryExpression, context.Span);

                if (binaryExpressionSelection.Expressions.Length > 1)
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractExpressionFromCondition))
                        ExtractConditionRefactoring.ComputeRefactoring(context, binaryExpressionSelection);

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.JoinStringExpressions)
                        && binaryExpression.IsKind(SyntaxKind.AddExpression))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        StringConcatenationExpression concatenation;
                        if (StringConcatenationExpression.TryCreate(binaryExpressionSelection, semanticModel, context.CancellationToken, out concatenation))
                        {
                            JoinStringExpressionsRefactoring.ComputeRefactoring(context, concatenation);
                        }
                    }
                }
            }
        }
    }
}
