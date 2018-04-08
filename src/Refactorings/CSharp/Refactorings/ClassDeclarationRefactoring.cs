// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddTypeParameter))
                AddTypeParameterRefactoring.ComputeRefactoring(context, classDeclaration);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, classDeclaration);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateBaseConstructors)
                && classDeclaration.Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                List<IMethodSymbol> constructors = GenerateBaseConstructorsAnalysis.GetMissingBaseConstructors(classDeclaration, semanticModel, context.CancellationToken);

                if (constructors?.Count > 0)
                {
                    context.RegisterRefactoring(
                        (constructors.Count == 1) ? "Generate base constructor" : "Generate base constructors",
                        cancellationToken => GenerateBaseConstructorsRefactoring.RefactorAsync(context.Document, classDeclaration, constructors.ToArray(), semanticModel, cancellationToken));
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ImplementIEquatableOfT))
                await ImplementIEquatableOfTRefactoring.ComputeRefactoringAsync(context, classDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SortMemberDeclarations)
                && classDeclaration.BracesSpan().Contains(context.Span))
            {
                SortMemberDeclarationsRefactoring.ComputeRefactoring(context, classDeclaration);
            }
        }
    }
}