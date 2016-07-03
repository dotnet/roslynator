// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ForStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ForStatementSyntax forStatement)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceForWithForeach)
                && context.SupportsSemanticModel
                && (await ReplaceForWithForeachRefactoring.CanRefactorAsync(context, forStatement)))
            {
                context.RegisterRefactoring(
                    "Replace for with foreach",
                    cancellationToken => ReplaceForWithForeachRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken));
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReverseForLoop)
                && forStatement.ForKeyword.Span.Contains(context.Span)
                && ReverseForRefactoring.CanRefactor(forStatement))
            {
                context.RegisterRefactoring(
                    "Reverse for loop",
                    cancellationToken => ReverseForRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken));
            }
        }
    }
}
