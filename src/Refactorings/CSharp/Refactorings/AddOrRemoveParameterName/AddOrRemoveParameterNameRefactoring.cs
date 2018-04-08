// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.AddOrRemoveParameterName
{
    internal static class AddOrRemoveParameterNameRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArgumentListSyntax argumentList)
        {
            if (!context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.AddParameterNameToArgument,
                RefactoringIdentifiers.RemoveParameterNameFromArgument))
            {
                return;
            }

            if (!SeparatedSyntaxListSelection<ArgumentSyntax>.TryCreate(argumentList.Arguments, context.Span, out SeparatedSyntaxListSelection<ArgumentSyntax> selection))
                return;

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddParameterNameToArgument))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                AddParameterNameRefactoring.ComputeRefactoring(context, argumentList, selection, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveParameterNameFromArgument))
                RemoveParameterNameRefactoring.ComputeRefactoring(context, argumentList, selection);
        }
    }
}
