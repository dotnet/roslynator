// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EnumMemberDeclarationRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EnumMemberDeclarationSyntax enumMemberDeclaration)
        {
            if (context.Span.IsEmptyAndContainedInSpan(enumMemberDeclaration))
            {
                SyntaxNode parent = enumMemberDeclaration.Parent;

                if (parent?.Kind() == SyntaxKind.EnumDeclaration)
                {
                    var enumDeclaration = (EnumDeclarationSyntax)parent;

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateEnumValues))
                        await GenerateEnumValuesRefactoring.ComputeRefactoringAsync(context, enumDeclaration).ConfigureAwait(false);

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateEnumMember))
                        await GenerateEnumMemberRefactoring.ComputeRefactoringAsync(context, enumDeclaration).ConfigureAwait(false);
                }
            }
        }
    }
}