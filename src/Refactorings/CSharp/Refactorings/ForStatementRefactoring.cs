// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ForStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ForStatementSyntax forStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertForToForEach)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(forStatement)
                && (await ConvertForToForEachRefactoring.CanRefactorAsync(context, forStatement).ConfigureAwait(false)))
            {
                context.RegisterRefactoring(
                    "Convert to 'foreach'",
                    cancellationToken => ConvertForToForEachRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken),
                    RefactoringIdentifiers.ConvertForToForEach);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertForToWhile)
                && (context.Span.IsEmptyAndContainedInSpan(forStatement.ForKeyword) || context.Span.IsBetweenSpans(forStatement)))
            {
                context.RegisterRefactoring(
                    "Convert to 'while'",
                    cancellationToken => ConvertForToWhileRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken),
                    RefactoringIdentifiers.ConvertForToWhile);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReverseForStatement)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(forStatement))
            {
                if (ReverseForStatementRefactoring.CanRefactor(forStatement))
                {
                    context.RegisterRefactoring(
                        "Reverse 'for' statement",
                        cancellationToken => ReverseForStatementRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken),
                        RefactoringIdentifiers.ReverseForStatement);
                }
                else if (ReverseReversedForStatementRefactoring.CanRefactor(forStatement))
                {
                    context.RegisterRefactoring(
                        "Reverse 'for' statement",
                        cancellationToken => ReverseReversedForStatementRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken),
                        RefactoringIdentifiers.ReverseForStatement);
                }
            }
        }
    }
}
