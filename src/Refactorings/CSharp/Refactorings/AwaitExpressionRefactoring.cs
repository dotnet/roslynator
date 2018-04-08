// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AwaitExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, AwaitExpressionSyntax awaitExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CallConfigureAwait))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (CallConfigureAwaitRefactoring.CanRefactor(awaitExpression, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Call 'ConfigureAwait(false)'",
                        ct => CallConfigureAwaitRefactoring.RefactorAsync(context.Document, awaitExpression, ct));
                }
            }
        }
    }
}
