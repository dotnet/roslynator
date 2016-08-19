// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp.Refactorings.WrapSelectedLines;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class SyntaxNodeRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxNode node)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInRegion)
                && await SelectedLinesRefactoring.CanRefactorAsync(context, node).ConfigureAwait(false))
            {
                context.RegisterRefactoring(
                   "Wrap in region",
                   cancellationToken =>
                   {
                       var refactoring = new WrapInRegionRefactoring();

                       return refactoring.RefactorAsync(
                           context.Document,
                           node,
                           context.Span,
                           cancellationToken);
                   });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInIfDirective)
                && await SelectedLinesRefactoring.CanRefactorAsync(context, node).ConfigureAwait(false))
            {
                context.RegisterRefactoring(
                   "Wrap in #if",
                   cancellationToken =>
                   {
                       var refactoring = new WrapInIfDirectiveRefactoring();

                       return refactoring.RefactorAsync(
                           context.Document,
                           node,
                           context.Span,
                           cancellationToken);
                   });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveEmptyLines)
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
