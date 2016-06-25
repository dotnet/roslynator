// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ForStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ForStatementSyntax forStatement)
        {
            if (context.SupportsSemanticModel
                && (await ConvertForToForEachRefactoring.CanRefactorAsync(context, forStatement)))
            {
                context.RegisterRefactoring(
                    "Convert for to foreach",
                    cancellationToken => ConvertForToForEachRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken));
            }

            if (forStatement.ForKeyword.Span.Contains(context.Span)
                && ReverseForRefactoring.CanRefactor(forStatement))
            {
                context.RegisterRefactoring(
                    "Reverse for loop",
                    cancellationToken => ReverseForRefactoring.RefactorAsync(context.Document, forStatement, cancellationToken));
            }
        }
    }
}
