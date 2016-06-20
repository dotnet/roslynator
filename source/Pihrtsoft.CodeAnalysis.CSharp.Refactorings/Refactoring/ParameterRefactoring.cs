// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ParameterRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            if (context.SupportsSemanticModel)
            {
                await RenameParameterAccordingToTypeNameRefactoring.RefactorAsync(context, parameter);

                await AddParameterNullCheckRefactoring.RefactorAsync(context, parameter);
            }
        }
    }
}