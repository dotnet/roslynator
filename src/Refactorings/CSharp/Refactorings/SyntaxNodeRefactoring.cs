// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.Refactorings.WrapSelectedLines;
using Roslynator.Text;

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
                //XPERF:
                TextLineCollectionSelection selectedLines = await SelectedLinesRefactoring.GetSelectedLinesAsync(context).ConfigureAwait(false);

                if (selectedLines != null)
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInRegion))
                    {
                        context.RegisterRefactoring(
                           "Wrap in region",
                           ct => WrapInRegionRefactoring.Instance.RefactorAsync(context.Document, selectedLines, ct));
                    }

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInIfDirective))
                    {
                        context.RegisterRefactoring(
                           "Wrap in #if",
                           ct => WrapInIfDirectiveRefactoring.Instance.RefactorAsync(context.Document, selectedLines, ct));
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveEmptyLines)
                && await RemoveEmptyLinesRefactoring.CanRefactorAsync(context, node).ConfigureAwait(false))
            {
                context.RegisterRefactoring(
                   "Remove empty lines",
                   ct => RemoveEmptyLinesRefactoring.RefactorAsync(context.Document, context.Span, ct));
            }
        }
    }
}
