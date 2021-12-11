// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InterpolatedStringRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.InsertStringInterpolation)
                && context.Span.IsEmpty
                && InsertInterpolationRefactoring.CanRefactor(context, interpolatedString))
            {
                context.RegisterRefactoring(
                    "Insert interpolation",
                    ct =>
                    {
                        return InsertInterpolationRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedString,
                            context.Span,
                            addNameOf: false,
                            cancellationToken: ct);
                    },
                    RefactoringDescriptors.InsertStringInterpolation);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertInterpolatedStringToStringLiteral)
                && ConvertInterpolatedStringToStringLiteralAnalysis.IsFixable(interpolatedString))
            {
                context.RegisterRefactoring(
                    "Remove '$'",
                    ct =>
                    {
                        return ConvertInterpolatedStringToStringLiteralRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedString,
                            ct);
                    },
                    RefactoringDescriptors.ConvertInterpolatedStringToStringLiteral);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ReplaceInterpolatedStringWithInterpolationExpression)
                && interpolatedString.Span.Contains(context.Span)
                && ReplaceInterpolatedStringWithInterpolationExpressionRefactoring.CanRefactor(interpolatedString))
            {
                ExpressionSyntax expression = ((InterpolationSyntax)(interpolatedString.Contents[0])).Expression;

                context.RegisterRefactoring(
                    $"Replace interpolated string with '{expression}'",
                    ct =>
                    {
                        return ReplaceInterpolatedStringWithInterpolationExpressionRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedString,
                            ct);
                    },
                    RefactoringDescriptors.ReplaceInterpolatedStringWithInterpolationExpression);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertInterpolatedStringToConcatenation)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(interpolatedString))
            {
                ConvertInterpolatedStringToConcatenationRefactoring.ComputeRefactoring(context, interpolatedString);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertInterpolatedStringToStringFormat)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(interpolatedString))
            {
                ConvertInterpolatedStringToStringFormatRefactoring.ComputeRefactoring(context, interpolatedString);
            }
        }
    }
}
