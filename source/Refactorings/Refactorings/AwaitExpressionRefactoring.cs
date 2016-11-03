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
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddConfigureAwait))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (AddConfigureAwaitRefactoring.CanRefactor(awaitExpression, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Add 'ConfigureAwait(false)'",
                        cancellationToken =>
                        {
                            return AddConfigureAwaitRefactoring.RefactorAsync(
                                context.Document,
                                awaitExpression,
                                context.CancellationToken);
                        });
                }
            }
        }
    }
}
