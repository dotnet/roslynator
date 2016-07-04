// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class InterpolatedStringRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddInterpolation)
                && context.Span.IsEmpty)
            {
                foreach (InterpolatedStringContentSyntax content in interpolatedString.Contents)
                {
                    if (content.IsKind(SyntaxKind.InterpolatedStringText)
                        && content.Span.End == context.Span.End)
                    {
                        context.RegisterRefactoring("Add interpolation",
                            cancellationToken =>
                            {
                                return AddInterpolationRefactoring.RefactorAsync(
                                    context.Document,
                                    (InterpolatedStringTextSyntax)content,
                                    context.Span,
                                    cancellationToken);
                            });

                        break;
                    }
                }
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceInterpolatedStringWithStringLiteral)
                && ReplaceInterpolatedStringWithStringLiteralRefactoring.CanRefactor(interpolatedString))
            {
                context.RegisterRefactoring("Replace interpolated string with string literal",
                    cancellationToken =>
                    {
                        return ReplaceInterpolatedStringWithStringLiteralRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedString,
                            cancellationToken);
                    });
            }
        }
    }
}
