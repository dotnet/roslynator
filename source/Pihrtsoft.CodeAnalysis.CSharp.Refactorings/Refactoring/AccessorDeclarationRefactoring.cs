// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AccessorDeclarationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, AccessorDeclarationSyntax accessor)
        {
            if (accessor.Body?.Span.Contains(context.Span) == true
                && !accessor.Body.OpenBraceToken.IsMissing
                && !accessor.Body.CloseBraceToken.IsMissing
                && accessor.Body.IsSingleline())
            {
                context.RegisterRefactoring(
                    "Format braces on multiple lines",
                    cancellationToken => FormatAccessorBraceOnMultipleLinesRefactoring.RefactorAsync(context.Document, accessor, cancellationToken));
            }
        }
    }
}
