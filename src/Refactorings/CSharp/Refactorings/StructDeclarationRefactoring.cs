// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.SortMemberDeclarations;

namespace Roslynator.CSharp.Refactorings
{
    internal static class StructDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, StructDeclarationSyntax structDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddGenericParameterToDeclaration))
                AddGenericParameterToDeclarationRefactoring.ComputeRefactoring(context, structDeclaration);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, structDeclaration);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ImplementIEquatableOfT))
                await ImplementIEquatableOfTRefactoring.ComputeRefactoringAsync(context, structDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ImplementCustomEnumerator)
                && context.Span.IsEmptyAndContainedInSpan(structDeclaration.Identifier))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ImplementCustomEnumeratorRefactoring.ComputeRefactoring(context, structDeclaration, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.SortMemberDeclarations)
                && structDeclaration.BracesSpan().Contains(context.Span))
            {
                SortMemberDeclarationsRefactoring.ComputeRefactoring(context, structDeclaration);
            }
        }
    }
}
