// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AccessorDeclarationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body?.Span.Contains(context.Span) == true
                && !body.OpenBraceToken.IsMissing
                && !body.CloseBraceToken.IsMissing)
            {
                if (body.IsSingleline())
                {
                    context.RegisterRefactoring(
                        "Format braces on multiple lines",
                        cancellationToken => FormatAccessorBraceOnMultipleLinesRefactoring.RefactorAsync(context.Document, accessor, cancellationToken));
                }
                else if (body.Statements.Count == 1
                    && body.Statements[0].IsSingleline())
                {
                    context.RegisterRefactoring(
                        "Format braces on single line",
                        cancellationToken => FormatAccessorBraceOnSingleLineRefactoring.RefactorAsync(context.Document, accessor, cancellationToken));
                }
            }
        }
    }
}
