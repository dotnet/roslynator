// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class InterpolatedStringTextRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InterpolatedStringTextSyntax interpolatedStringText)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddInterpolation)
                && AddInterpolationRefactoring.CanRefactor(context, interpolatedStringText))
            {
                context.RegisterRefactoring("Add interpolation",
                    cancellationToken =>
                    {
                        return AddInterpolationRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedStringText,
                            context.Span,
                            cancellationToken);
                    });
            }
        }
    }
}
