// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InterfaceDeclarationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, interfaceDeclaration);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SortMemberDeclarations)
                && interfaceDeclaration.BracesSpan().Contains(context.Span))
            {
                SortMemberDeclarationsRefactoring.ComputeRefactoring(context, interfaceDeclaration);
            }
        }
    }
}