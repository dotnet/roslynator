// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ConvertReturnToIf;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReturnStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ReturnStatementSyntax returnStatement)
        {
            ExpressionSyntax expression = returnStatement.Expression;

            if (expression != null
                && context.IsRefactoringEnabled(RefactoringDescriptors.ConvertReturnStatementToIf)
                && (context.Span.IsEmptyAndContainedInSpan(returnStatement.ReturnKeyword)
                    || context.Span.IsBetweenSpans(returnStatement)))
            {
                await ConvertReturnStatementToIfRefactoring.ConvertReturnToIfElse.ComputeRefactoringAsync(context, returnStatement).ConfigureAwait(false);
            }
        }
    }
}
