// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class BlockRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, BlockSyntax block)
        {
            if (SelectedStatementsRefactoring.IsAnyRefactoringEnabled(context)
                && StatementListSelection.TryCreate(block, context.Span, out StatementListSelection selectedStatements))
            {
                await SelectedStatementsRefactoring.ComputeRefactoringAsync(context, selectedStatements).ConfigureAwait(false);
            }
        }
    }
}
