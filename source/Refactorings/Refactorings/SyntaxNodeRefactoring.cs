// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.Refactorings.WrapSelectedLines;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SyntaxNodeRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxNode node)
        {
            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.WrapInRegion,
                RefactoringIdentifiers.WrapInIfDirective))
            {
                SelectedLinesInfo info = await SelectedLinesRefactoring.GetSelectedLinesInfoAsync(context, node).ConfigureAwait(false);

                if (info?.IsAnySelected == true)
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInRegion))
                    {
                        context.RegisterRefactoring(
                           "Wrap in region",
                           cancellationToken =>
                           {
                               var refactoring = new WrapInRegionRefactoring();

                               return refactoring.RefactorAsync(context.Document, info, cancellationToken);
                           });
                    }

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInIfDirective))
                    {
                        context.RegisterRefactoring(
                           "Wrap in #if",
                           cancellationToken =>
                           {
                               var refactoring = new WrapInIfDirectiveRefactoring();

                               return refactoring.RefactorAsync(context.Document, info, cancellationToken);
                           });
                    }
                }
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
