// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EnumDeclarationRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EnumDeclarationSyntax enumDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractTypeDeclarationToNewFile))
                ExtractTypeDeclarationToNewFileRefactoring.ComputeRefactorings(context, enumDeclaration);

            if (enumDeclaration.BracesSpan().Contains(context.Span))
            {
                await SelectedEnumMemberDeclarationsRefactoring.ComputeRefactoringAsync(context, enumDeclaration).ConfigureAwait(false);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateEnumMember)
                    && context.Span.IsEmpty)
                {
                    await GenerateEnumMemberRefactoring.ComputeRefactoringAsync(context, enumDeclaration).ConfigureAwait(false);
                }
            }
        }
    }
}