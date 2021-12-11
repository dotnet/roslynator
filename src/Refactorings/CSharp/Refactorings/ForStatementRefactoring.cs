// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ForStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ForStatementSyntax forStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertForToForEach)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(forStatement)
                && (await ConvertForToForEachRefactoring.CanRefactorAsync(context, forStatement).ConfigureAwait(false)))
            {
                context.RegisterRefactoring(
                    "Convert to 'foreach'",
                    ct => ConvertForToForEachRefactoring.RefactorAsync(context.Document, forStatement, ct),
                    RefactoringDescriptors.ConvertForToForEach);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertForToWhile)
                && (context.Span.IsEmptyAndContainedInSpan(forStatement.ForKeyword) || context.Span.IsBetweenSpans(forStatement)))
            {
                context.RegisterRefactoring(
                    "Convert to 'while'",
                    ct => ConvertForToWhileRefactoring.RefactorAsync(context.Document, forStatement, ct),
                    RefactoringDescriptors.ConvertForToWhile);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ReverseForStatement)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(forStatement))
            {
                if (ReverseForStatementRefactoring.CanRefactor(forStatement))
                {
                    context.RegisterRefactoring(
                        "Reverse 'for' statement",
                        ct => ReverseForStatementRefactoring.RefactorAsync(context.Document, forStatement, ct),
                        RefactoringDescriptors.ReverseForStatement);
                }
                else if (ReverseReversedForStatementRefactoring.CanRefactor(forStatement))
                {
                    context.RegisterRefactoring(
                        "Reverse 'for' statement",
                        ct => ReverseReversedForStatementRefactoring.RefactorAsync(context.Document, forStatement, ct),
                        RefactoringDescriptors.ReverseForStatement);
                }
            }
        }
    }
}
