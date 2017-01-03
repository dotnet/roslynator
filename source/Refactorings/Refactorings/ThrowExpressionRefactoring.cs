// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.AddExceptionToDocumentationComment;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ThrowExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ThrowExpressionSyntax throwExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddExceptionToDocumentationComment))
                await AddExceptionToDocumentationCommentRefactoring.ComputeRefactoringAsync(context, throwExpression).ConfigureAwait(false);
        }
    }
}