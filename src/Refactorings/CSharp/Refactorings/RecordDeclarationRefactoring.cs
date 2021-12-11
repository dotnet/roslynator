// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Refactorings.SortMemberDeclarations;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RecordDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, RecordDeclarationSyntax recordDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddGenericParameterToDeclaration))
                AddGenericParameterToDeclarationRefactoring.ComputeRefactoring(context, recordDeclaration);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, recordDeclaration);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.GenerateBaseConstructors)
                && recordDeclaration.Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                List<IMethodSymbol> constructors = GenerateBaseConstructorsAnalysis.GetMissingBaseConstructors(recordDeclaration, semanticModel, context.CancellationToken);

                if (constructors?.Count > 0)
                {
                    context.RegisterRefactoring(
                        (constructors.Count == 1) ? "Generate base constructor" : "Generate base constructors",
                        ct => GenerateBaseConstructorsRefactoring.RefactorAsync(context.Document, recordDeclaration, constructors.ToArray(), semanticModel, ct),
                        RefactoringDescriptors.GenerateBaseConstructors);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ImplementCustomEnumerator)
                && context.Span.IsEmptyAndContainedInSpan(recordDeclaration.Identifier))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ImplementCustomEnumeratorRefactoring.ComputeRefactoring(context, recordDeclaration, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExpandPositionalConstructor)
                && recordDeclaration.ParameterList != null
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(recordDeclaration.ParameterList.Parameters))
            {
                ExpandPositionalConstructorRefactoring.ComputeRefactoring(context, recordDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.SortMemberDeclarations)
                && recordDeclaration.BracesSpan().Contains(context.Span))
            {
                SortMemberDeclarationsRefactoring.ComputeRefactoring(context, recordDeclaration);
            }
        }
    }
}
