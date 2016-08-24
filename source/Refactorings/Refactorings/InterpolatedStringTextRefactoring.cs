// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class InterpolatedStringTextRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InterpolatedStringTextSyntax interpolatedStringText)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InsertStringInterpolation)
                && InsertInterpolationRefactoring.CanRefactor(interpolatedStringText))
            {
                context.RegisterRefactoring("Insert interpolation",
                    cancellationToken =>
                    {
                        return InsertInterpolationRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedStringText,
                            context.Span,
                            cancellationToken);
                    });
            }
        }
    }
}
