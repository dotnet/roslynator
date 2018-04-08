// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.IntroduceAndInitialize;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ParameterRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            await AddOrRenameParameterRefactoring.ComputeRefactoringsAsync(context, parameter).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CheckParameterForNull))
                await CheckParameterForNullRefactoring.ComputeRefactoringAsync(context, parameter).ConfigureAwait(false);

            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.IntroduceAndInitializeField,
                RefactoringIdentifiers.IntroduceAndInitializeProperty))
            {
                IntroduceAndInitializeRefactoring.ComputeRefactoring(context, parameter);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddDefaultValueToParameter))
                await AddDefaultValueToParameterRefactoring.ComputeRefactoringAsync(context, parameter).ConfigureAwait(false);
        }
    }
}