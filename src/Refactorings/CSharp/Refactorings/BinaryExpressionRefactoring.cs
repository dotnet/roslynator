// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InvertOperator)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(operatorToken)
                && InvertOperatorRefactoring.CanBeInverted(operatorToken))
            {
                context.RegisterRefactoring(
                    "Invert operator",
                    cancellationToken => InvertOperatorRefactoring.RefactorAsync(context.Document, operatorToken, cancellationToken),
                    RefactoringIdentifiers.InvertOperator);
            }

            if (context.Span.IsEmptyAndContainedInSpan(operatorToken))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.InvertBinaryExpression))
                    InvertBinaryExpressionRefactoring.ComputeRefactoring(context, binaryExpression);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.SwapBinaryOperands))
                    SwapBinaryOperandsRefactoring.ComputeRefactoring(context, binaryExpression);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatBinaryExpression))
                FormatBinaryExpressionRefactoring.ComputeRefactorings(context, binaryExpression);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandCoalesceExpression)
                && operatorToken.Span.Contains(context.Span))
            {
                ExpandCoalesceExpressionRefactoring.ComputeRefactoring(context, binaryExpression);
            }

            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ExtractExpressionFromCondition,
                    RefactoringIdentifiers.JoinStringExpressions,
                    RefactoringIdentifiers.UseStringBuilderInsteadOfConcatenation)
                && !context.Span.IsEmpty
                && binaryExpression.IsKind(SyntaxKind.AddExpression, SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression))
            {
                ExpressionChain chain = binaryExpression.AsChain(context.Span);

                ExpressionChain.Enumerator en = chain.GetEnumerator();

                if (en.MoveNext()
                    && en.MoveNext())
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractExpressionFromCondition))
                        ExtractConditionRefactoring.ComputeRefactoring(context, chain);

                    if (binaryExpression.IsKind(SyntaxKind.AddExpression))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        StringConcatenationExpressionInfo concatenationInfo = SyntaxInfo.StringConcatenationExpressionInfo(chain, semanticModel, context.CancellationToken);
                        if (concatenationInfo.Success)
                        {
                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.JoinStringExpressions))
                                JoinStringExpressionsRefactoring.ComputeRefactoring(context, concatenationInfo);

                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseStringBuilderInsteadOfConcatenation))
                                UseStringBuilderInsteadOfConcatenationRefactoring.ComputeRefactoring(context, concatenationInfo);
                        }
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceAsWithCast)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(binaryExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (ReplaceAsWithCastAnalysis.IsFixable(binaryExpression, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Replace as with cast",
                        cancellationToken => ReplaceAsWithCastRefactoring.RefactorAsync(context.Document, binaryExpression, cancellationToken),
                        RefactoringIdentifiers.ReplaceAsWithCast);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InvertIsExpression))
                InvertIsExpressionRefactoring.ComputeRefactoring(context, binaryExpression);

            if (context.Span.IsContainedInSpanOrBetweenSpans(operatorToken))
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
        }
    }
}
