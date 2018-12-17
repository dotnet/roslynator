// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EnumMemberDeclarationRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EnumMemberDeclarationSyntax enumMemberDeclaration)
        {
            if (context.Span.IsEmpty
                && enumMemberDeclaration.Parent is EnumDeclarationSyntax enumDeclaration)
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateEnumValues))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    GenerateEnumValuesRefactoring.ComputeRefactoring(context, enumDeclaration, semanticModel);
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateEnumMember))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    GenerateEnumMemberRefactoring.ComputeRefactoring(context, enumDeclaration, semanticModel);
                }
            }
        }
    }
}