// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class LocalDeclarationStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.InitializeLocalVariableWithDefaultValue))
                await InitializeLocalVariableWithDefaultValueRefactoring.ComputeRefactoringAsync(context, localDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.PromoteLocalVariableToParameter))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                PromoteLocalToParameterRefactoring.ComputeRefactoring(context, localDeclaration, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveInstantiationOfLocalVariable))
                await RemoveInstantiationOfLocalVariableRefactoring.ComputeRefactoringAsync(context, localDeclaration).ConfigureAwait(false);
        }
    }
}
