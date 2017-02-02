// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EnumDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, EnumDeclarationSyntax enumDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, enumDeclaration);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SortMemberDeclarations)
                && enumDeclaration.BracesSpan().Contains(context.Span))
            {
                await SortEnumMemberDeclarationsRefactoring.ComputeRefactoringAsync(context, enumDeclaration).ConfigureAwait(false);
            }
        }
    }
}