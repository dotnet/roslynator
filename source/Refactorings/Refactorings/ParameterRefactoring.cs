// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactorings.IntroduceAndInitialize;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class ParameterRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            if (context.SupportsSemanticModel)
            {
                await AddOrRenameParameterRefactoring.ComputeRefactoringsAsync(context, parameter).ConfigureAwait(false);

                await CheckParameterForNullRefactoring.ComputeRefactoringAsync(context, parameter).ConfigureAwait(false);

                if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.IntroduceAndInitializeField,
                    RefactoringIdentifiers.IntroduceAndInitializeProperty))
                {
                    IntroduceAndInitializeRefactoring.ComputeRefactoring(context, parameter);
                }

                await AddDefaultValueToParameterRefactoring.ComputeRefactoringAsync(context, parameter).ConfigureAwait(false);
            }
        }
    }
}