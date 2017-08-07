// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class WhileStatementRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, WhileStatementSyntax whileStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceWhileStatementWithDoStatement)
                && (whileStatement.WhileKeyword.Span.Contains(context.Span)))
            {
                context.RegisterRefactoring(
                    "Replace while with do",
                    cancellationToken =>
                    {
                        return ReplaceWhileWithDoRefactoring.RefactorAsync(
                            context.Document,
                            whileStatement,
                            cancellationToken);
                    });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceWhileWithFor)
                && context.Span.IsBetweenSpans(whileStatement))
            {
                ReplaceWhileWithForRefactoring.ComputeRefactoring(context, whileStatement);
            }
        }
    }
}