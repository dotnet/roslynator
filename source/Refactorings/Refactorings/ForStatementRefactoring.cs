// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ForStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ForStatementSyntax forStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceForWithForEach)
                && context.SupportsSemanticModel
                && context.Span.IsEmpty
                && forStatement.Span.Contains(context.Span)
                && (await ReplaceForWithForEachRefactoring.CanRefactorAsync(context, forStatement).ConfigureAwait(false)))
            {
                context.RegisterRefactoring(
                    "Replace for with foreach",
                    cancellationToken => ReplaceForWithForEachRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReverseForLoop)
                && context.Span.IsEmpty
                && forStatement.Span.Contains(context.Span))
            {
                if (ReverseForLoopRefactoring.CanRefactor(forStatement))
                {
                    context.RegisterRefactoring(
                        "Reverse for loop",
                        cancellationToken => ReverseForLoopRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken));
                }
                else if (ReverseReversedForLoopRefactoring.CanRefactor(forStatement))
                {
                    context.RegisterRefactoring(
                        "Reverse for loop",
                        cancellationToken => ReverseReversedForLoopRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken));
                }
            }
        }
    }
}
