// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class WhileStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, WhileStatementSyntax whileStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBooleanComparison)
                && whileStatement.Condition != null
                && whileStatement.Condition.Span.Contains(context.Span)
                && context.SupportsSemanticModel)
            {
                await AddBooleanComparisonRefactoring.ComputeRefactoringAsync(context, whileStatement.Condition).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceWhileStatementWithDoStatement)
                && (whileStatement.WhileKeyword.Span.Contains(context.Span)))
            {
                context.RegisterRefactoring(
                    "Replace while with do",
                    cancellationToken =>
                    {
                        return ReplaceWhileStatementWithDoStatementRefactoring.RefactorAsync(
                            context.Document,
                            whileStatement,
                            cancellationToken);
                    });
            }
        }
    }
}