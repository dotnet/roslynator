// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.AddOrRemoveParameterName
{
    internal static class AddOrRemoveArgumentNameRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArgumentListSyntax argumentList)
        {
            if (!context.IsAnyRefactoringEnabled(
                RefactoringDescriptors.AddArgumentName,
                RefactoringDescriptors.RemoveArgumentName))
            {
                return;
            }

            if (!SeparatedSyntaxListSelection<ArgumentSyntax>.TryCreate(argumentList.Arguments, context.Span, out SeparatedSyntaxListSelection<ArgumentSyntax> selection))
                return;

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddArgumentName))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                AddArgumentNameRefactoring.ComputeRefactoring(context, argumentList, selection, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveArgumentName))
                RemoveArgumentNameRefactoring.ComputeRefactoring(context, argumentList, selection);
        }
    }
}
