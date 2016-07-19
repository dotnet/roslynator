// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class LocalDeclarationStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddIdentifierToVariableDeclaration))
                await AddIdentifierToLocalDeclarationRefactoring.ComputeRefactoringAsync(context, localDeclaration);

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.InitializeLocalWithDefaultValue)
                && context.SupportsSemanticModel)
            {
                await InitializeLocalWithDefaultValueRefactoring.ComputeRefactoringAsync(context, localDeclaration);
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.WrapDeclarationInUsingStatement)
                && context.SupportsSemanticModel)
            {
                await WrapDeclarationInUsingStatementRefactoring.ComputeRefactoringAsync(context, localDeclaration);
            }
        }
    }
}
