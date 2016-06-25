// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class LocalDeclarationStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            if (context.SupportsSemanticModel)
                await AddUsingStatementRefactoring.ComputeRefactoringAsync(context, localDeclaration);
        }
    }
}
