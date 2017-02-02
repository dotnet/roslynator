// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class StructDeclarationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, StructDeclarationSyntax structDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, structDeclaration);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SortMemberDeclarations)
                && structDeclaration.BracesSpan().Contains(context.Span))
            {
                SortMemberDeclarationsRefactoring.ComputeRefactoring(context, structDeclaration);
            }
        }
    }
}