// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class BlockRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BlockSyntax block)
        {
            ReplaceBlockWithEmbeddedStatementRefactoring.ComputeRefactoring(context, block);

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.WrapStatementsInTryCatch)
                && WrapStatementsInTryCatchRefactoring.CanRefactor(context, block))
            {
                context.RegisterRefactoring(
                    "Wrap in try-catch",
                    cancellationToken =>
                    {
                        return WrapStatementsInTryCatchRefactoring.RefactorAsync(
                            context.Document,
                            block,
                            context.Span,
                            cancellationToken);
                    });
            }
        }
    }
}
