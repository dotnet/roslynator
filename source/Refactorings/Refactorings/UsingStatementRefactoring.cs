// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsingStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, UsingStatementSyntax usingStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractDeclarationFromUsingStatement))
                await ExtractDeclarationFromUsingStatementRefactoring.ComputeRefactoringAsync(context, usingStatement).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceLocalVariable))
                IntroduceLocalVariableRefactoring.ComputeRefactoring(context, usingStatement);
        }
    }
}