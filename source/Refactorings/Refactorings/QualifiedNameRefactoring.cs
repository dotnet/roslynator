// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class QualifiedNameRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, QualifiedNameSyntax qualifiedName)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddUsingDirective)
                && context.SupportsSemanticModel)
            {
                await AddUsingDirectiveRefactoring.ComputeRefactoringsAsync(context, qualifiedName);
            }
        }
    }
}
