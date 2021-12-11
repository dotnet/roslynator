// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EnumDeclarationRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EnumDeclarationSyntax enumDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, enumDeclaration);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveEnumMemberValue)
                && context.Span.IsEmptyAndContainedInSpan(enumDeclaration.Identifier))
            {
                RemoveEnumMemberValueRefactoring.ComputeRefactoring(context, enumDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.GenerateEnumValues)
                && context.Span.IsEmpty)
            {
                if (enumDeclaration.BracesSpan().Contains(context.Span))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    GenerateEnumValuesRefactoring.ComputeRefactoring(context, enumDeclaration, semanticModel);
                }

                if (enumDeclaration.Identifier.Span.Contains(context.Span))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    GenerateAllEnumValuesRefactoring.ComputeRefactoring(context, enumDeclaration, semanticModel);
                }
            }

            await SelectedEnumMemberDeclarationsRefactoring.ComputeRefactoringAsync(context, enumDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.GenerateEnumMember)
                && context.Span.IsEmpty
                && enumDeclaration.BracesSpan().Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                GenerateEnumMemberRefactoring.ComputeRefactoring(context, enumDeclaration, semanticModel);
            }
        }
    }
}
