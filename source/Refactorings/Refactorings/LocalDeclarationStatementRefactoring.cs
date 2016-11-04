// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class LocalDeclarationStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddIdentifierToVariableDeclaration))
                await AddIdentifierToLocalDeclarationRefactoring.ComputeRefactoringAsync(context, localDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InitializeLocalWithDefaultValue)
                && context.SupportsSemanticModel)
            {
                await InitializeLocalWithDefaultValueRefactoring.ComputeRefactoringAsync(context, localDeclaration).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.PromoteLocalToParameter)
                && context.SupportsSemanticModel)
            {
                await PromoteLocalToParameterRefactoring.ComputeRefactoringAsync(context, localDeclaration).ConfigureAwait(false);
            }
        }
    }
}
