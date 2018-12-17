// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.SortMemberDeclarations;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SelectedEnumMemberDeclarationsRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EnumDeclarationSyntax enumDeclaration)
        {
            if (!SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax>.TryCreate(enumDeclaration.Members, context.Span, out SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selection))
                return;

            if (selection.Count > 1)
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.SortMemberDeclarations))
                    await SortEnumMemberDeclarationsRefactoring.ComputeRefactoringAsync(context, enumDeclaration, selection).ConfigureAwait(false);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateCombinedEnumMember))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    GenerateCombinedEnumMemberRefactoring.ComputeRefactoring(context, enumDeclaration, selection, semanticModel);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveEnumMemberValue))
                RemoveEnumMemberValueRefactoring.ComputeRefactoring(context, enumDeclaration, selection);
        }
    }
}