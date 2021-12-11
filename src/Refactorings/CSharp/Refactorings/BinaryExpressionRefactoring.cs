// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Refactorings.ExtractCondition;
using Roslynator.CSharp.Refactorings.ReplaceEqualsExpression;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class BinaryExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            SyntaxToken operatorToken = binaryExpression.OperatorToken;

            if (context.IsRefactoringEnabled(RefactoringDescriptors.InvertOperator)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(operatorToken)
                && InvertOperatorRefactoring.CanBeInverted(operatorToken))
            {
                context.RegisterRefactoring(
                    "Invert operator",
                    ct => InvertOperatorRefactoring.RefactorAsync(context.Document, operatorToken, ct),
                    RefactoringDescriptors.InvertOperator);
            }

            if (context.Span.IsEmptyAndContainedInSpan(operatorToken))
            {
                if (context.IsRefactoringEnabled(RefactoringDescriptors.InvertBinaryExpression))
                    InvertBinaryExpressionRefactoring.ComputeRefactoring(context, binaryExpression);

                if (context.IsRefactoringEnabled(RefactoringDescriptors.SwapBinaryOperands))
                    SwapBinaryOperandsRefactoring.ComputeRefactoring(context, binaryExpression);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapBinaryExpression))
                WrapBinaryExpressionRefactoring.ComputeRefactorings(context, binaryExpression);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExpandCoalesceExpression)
                && operatorToken.Span.Contains(context.Span))
            {
                ExpandCoalesceExpressionRefactoring.ComputeRefactoring(context, binaryExpression);
            }

            if (context.IsAnyRefactoringEnabled(
                RefactoringDescriptors.ExtractExpressionFromCondition,
                RefactoringDescriptors.JoinStringExpressions,
                RefactoringDescriptors.UseStringBuilderInsteadOfConcatenation)
                && !context.Span.IsEmpty
                && binaryExpression.IsKind(SyntaxKind.AddExpression, SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression))
            {
                ExpressionChain chain = binaryExpression.AsChain(context.Span);

                ExpressionChain.Enumerator en = chain.GetEnumerator();

                if (en.MoveNext()
                    && en.MoveNext())
                {
                    if (context.IsRefactoringEnabled(RefactoringDescriptors.ExtractExpressionFromCondition))
                        ExtractConditionRefactoring.ComputeRefactoring(context, chain);

                    if (binaryExpression.IsKind(SyntaxKind.AddExpression))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        StringConcatenationExpressionInfo concatenationInfo = SyntaxInfo.StringConcatenationExpressionInfo(chain, semanticModel, context.CancellationToken);
                        if (concatenationInfo.Success)
                        {
                            if (context.IsRefactoringEnabled(RefactoringDescriptors.JoinStringExpressions))
                                JoinStringExpressionsRefactoring.ComputeRefactoring(context, concatenationInfo);

                            if (context.IsRefactoringEnabled(RefactoringDescriptors.UseStringBuilderInsteadOfConcatenation))
                                UseStringBuilderInsteadOfConcatenationRefactoring.ComputeRefactoring(context, concatenationInfo);
                        }
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ReplaceAsExpressionWithExplicitCast)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(binaryExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (ReplaceAsWithCastAnalysis.IsFixable(binaryExpression, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        ReplaceAsExpressionWithExplicitCastRefactoring.Title,
                        ct => ReplaceAsExpressionWithExplicitCastRefactoring.RefactorAsync(context.Document, binaryExpression, ct),
                        RefactoringDescriptors.ReplaceAsExpressionWithExplicitCast);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.InvertIsExpression))
                InvertIsExpressionRefactoring.ComputeRefactoring(context, binaryExpression);

            if (context.Span.IsContainedInSpanOrBetweenSpans(operatorToken))
            {
                if (context.IsRefactoringEnabled(RefactoringDescriptors.ReplaceEqualityOperatorWithStringEquals))
                    await ReplaceEqualityOperatorWithStringEqualsRefactoring.ComputeRefactoringAsync(context, binaryExpression).ConfigureAwait(false);

                if (context.IsAnyRefactoringEnabled(
                    RefactoringDescriptors.ReplaceEqualityOperatorWithStringIsNullOrEmpty,
                    RefactoringDescriptors.ReplaceEqualityOperatorWithStringIsNullOrWhiteSpace))
                {
                    await ReplaceEqualityOperatorRefactoring.ComputeRefactoringsAsync(context, binaryExpression).ConfigureAwait(false);
                }
            }
        }
    }
}
