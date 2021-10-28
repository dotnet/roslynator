// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenericNameRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, GenericNameSyntax genericName)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractGenericType)
                && ExtractGenericTypeRefactoring.CanRefactor(context, genericName))
            {
                context.RegisterRefactoring(
                    "Extract generic type",
                    ct => ExtractGenericTypeRefactoring.RefactorAsync(context.Document, genericName, ct),
                    RefactoringIdentifiers.ExtractGenericType);
            }
        }
    }
}
