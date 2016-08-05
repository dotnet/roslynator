// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SyntaxNodeRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxNode node)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddRegion)
                && await SelectedLinesRefactoring.CanRefactorAsync(context, node).ConfigureAwait(false))
            {
                context.RegisterRefactoring(
                   "Add region",
                   cancellationToken =>
                   {
                       var refactoring = new AddRegionRefactoring();

                       return refactoring.RefactorAsync(
                           context.Document,
                           node,
                           context.Span,
                           cancellationToken);
                   });
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddIfDirective)
                && await SelectedLinesRefactoring.CanRefactorAsync(context, node).ConfigureAwait(false))
            {
                context.RegisterRefactoring(
                   "Add #if",
                   cancellationToken =>
                   {
                       var refactoring = new AddIfDirectiveRefactoring();

                       return refactoring.RefactorAsync(
                           context.Document,
                           node,
                           context.Span,
                           cancellationToken);
                   });
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemoveEmptyLines)
                && await RemoveEmptyLinesRefactoring.CanRefactorAsync(context, node).ConfigureAwait(false))
            {
                context.RegisterRefactoring(
                   "Remove empty lines",
                   cancellationToken =>
                   {
                       return RemoveEmptyLinesRefactoring.RefactorAsync(
                           context.Document,
                           node,
                           context.Span,
                           cancellationToken);
                   });
            }
        }
    }
}
