// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Refactorings.SortMemberDeclarations;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ClassDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddGenericParameterToDeclaration))
                AddGenericParameterToDeclarationRefactoring.ComputeRefactoring(context, classDeclaration);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, classDeclaration);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.GenerateBaseConstructors)
                && classDeclaration.Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                List<IMethodSymbol> constructors = GenerateBaseConstructorsAnalysis.GetMissingBaseConstructors(classDeclaration, semanticModel, context.CancellationToken);

                if (constructors?.Count > 0)
                {
                    context.RegisterRefactoring(
                        (constructors.Count == 1) ? "Generate base constructor" : "Generate base constructors",
                        ct => GenerateBaseConstructorsRefactoring.RefactorAsync(context.Document, classDeclaration, constructors.ToArray(), semanticModel, ct),
                        RefactoringDescriptors.GenerateBaseConstructors);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ImplementIEquatableOfT))
                await ImplementIEquatableOfTRefactoring.ComputeRefactoringAsync(context, classDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ImplementCustomEnumerator)
                && context.Span.IsEmptyAndContainedInSpan(classDeclaration.Identifier))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ImplementCustomEnumeratorRefactoring.ComputeRefactoring(context, classDeclaration, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.SortMemberDeclarations)
                && classDeclaration.BracesSpan().Contains(context.Span))
            {
                SortMemberDeclarationsRefactoring.ComputeRefactoring(context, classDeclaration);
            }
        }
    }
}
