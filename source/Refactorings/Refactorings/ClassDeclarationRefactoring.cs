// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.SortMemberDeclarations;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ClassDeclarationRefactoring
    {
        public static async Task ComputeRefactorings(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddTypeParameter))
                AddTypeParameterRefactoring.ComputeRefactoring(context, classDeclaration);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, classDeclaration);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateBaseConstructors))
                await GenerateBaseConstructorsRefactoring.ComputeRefactoringAsync(context, classDeclaration).ConfigureAwait(false);

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