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
                    },
                    RefactoringIdentifiers.InsertStringInterpolation);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertInterpolatedStringToStringLiteral)
                && ConvertInterpolatedStringToStringLiteralAnalysis.IsFixable(interpolatedString))
            {
                context.RegisterRefactoring("Remove '$'",
                    cancellationToken =>
                    {
                        return ConvertInterpolatedStringToStringLiteralRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedString,
                            cancellationToken);
                    },
                    RefactoringIdentifiers.ConvertInterpolatedStringToStringLiteral);
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
                    },
                    RefactoringIdentifiers.ReplaceInterpolatedStringWithInterpolationExpression);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertInterpolatedStringToConcatenation)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(interpolatedString))
            {
                ConvertInterpolatedStringToConcatenationRefactoring.ComputeRefactoring(context, interpolatedString);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertInterpolatedStringToStringFormat)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(interpolatedString))
            {
                ConvertInterpolatedStringToStringFormatRefactoring.ComputeRefactoring(context, interpolatedString);
            }
        }
    }
}
