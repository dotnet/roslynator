// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class InterpolatedStringRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InsertStringInterpolation)
                && context.Span.IsEmpty)
            {
                foreach (InterpolatedStringContentSyntax content in interpolatedString.Contents)
                {
                    if (content.IsKind(SyntaxKind.InterpolatedStringText)
                        && content.Span.End == context.Span.End)
                    {
                        context.RegisterRefactoring("Insert interpolation",
                            cancellationToken =>
                            {
                                return InsertInterpolationRefactoring.RefactorAsync(
                                    context.Document,
                                    (InterpolatedStringTextSyntax)content,
                                    context.Span,
                                    cancellationToken);
                            });

                        break;
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceInterpolatedStringWithStringLiteral)
                && ReplaceInterpolatedStringWithStringLiteralRefactoring.CanRefactor(interpolatedString))
            {
                context.RegisterRefactoring("Remove '$'",
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
