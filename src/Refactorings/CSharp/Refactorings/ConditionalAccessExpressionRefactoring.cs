// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConditionalAccessExpressionRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ConditionalAccessExpressionSyntax conditionalAccessExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapCallChain))
                await WrapCallChainRefactoring.ComputeRefactoringsAsync(context, conditionalAccessExpression).ConfigureAwait(false);
        }
    }
}
