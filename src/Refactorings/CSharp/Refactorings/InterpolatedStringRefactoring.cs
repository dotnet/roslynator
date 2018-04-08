// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InterpolatedStringRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InsertStringInterpolation)
                && context.Span.IsEmpty
                && InsertInterpolationRefactoring.CanRefactor(context, interpolatedString))
            {
                context.RegisterRefactoring("Insert interpolation",
                    cancellationToken =>
                    {
                        return InsertInterpolationRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedString,
                            context.Span,
                            addNameOf: false,
                            cancellationToken: cancellationToken);
                    });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceInterpolatedStringWithStringLiteral)
                && ReplaceInterpolatedStringWithStringLiteralAnalysis.IsFixable(interpolatedString))
            {
                context.RegisterRefactoring("Remove $",
                    cancellationToken =>
                    {
                        return ReplaceInterpolatedStringWithStringLiteralRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedString,
                            cancellationToken);
                    });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceInterpolatedStringWithInterpolationExpression)
                && interpolatedString.Span.Contains(context.Span)
                && ReplaceInterpolatedStringWithInterpolationExpressionRefactoring.CanRefactor(interpolatedString))
            {
                ExpressionSyntax expression = ((InterpolationSyntax)(interpolatedString.Contents[0])).Expression;

                context.RegisterRefactoring(
                    $"Replace interpolated string with '{expression}'",
                    cancellationToken =>
                    {
                        return ReplaceInterpolatedStringWithInterpolationExpressionRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedString,
                            cancellationToken);
                    });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceInterpolatedStringWithConcatenation)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(interpolatedString))
            {
                ReplaceInterpolatedStringWithConcatenationRefactoring.ComputeRefactoring(context, interpolatedString);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceInterpolatedStringWithStringFormat)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(interpolatedString))
            {
                ReplaceInterpolatedStringWithStringFormatRefactoring.ComputeRefactoring(context, interpolatedString);
            }
        }
    }
}
