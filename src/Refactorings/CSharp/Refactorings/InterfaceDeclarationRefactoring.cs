// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.SortMemberDeclarations;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InterfaceDeclarationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddGenericParameterToDeclaration))
                AddGenericParameterToDeclarationRefactoring.ComputeRefactoring(context, interfaceDeclaration);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, interfaceDeclaration);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.SortMemberDeclarations)
                && interfaceDeclaration.BracesSpan().Contains(context.Span))
            {
                SortMemberDeclarationsRefactoring.ComputeRefactoring(context, interfaceDeclaration);
            }
        }
    }
}
