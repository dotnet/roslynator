// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimpleMemberAccessExpressionRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddUsingStaticDirective))
                await AddUsingStaticDirectiveRefactoring.ComputeRefactoringsAsync(context, memberAccess).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapCallChain))
                await WrapCallChainRefactoring.ComputeRefactoringsAsync(context, memberAccess).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertMethodGroupToLambda))
                await ConvertMethodGroupToLambdaRefactoring.ComputeRefactoringAsync(context, memberAccess).ConfigureAwait(false);
        }
    }
}
