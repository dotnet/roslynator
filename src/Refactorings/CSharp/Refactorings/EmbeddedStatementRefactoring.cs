// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.WrapStatements;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EmbeddedStatementRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            if (!statement.IsEmbedded(canBeIfInsideElse: false, canBeUsingInsideUsing: false))
                return;

            if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapStatementsInCondition))
            {
                context.RegisterRefactoring(
                    WrapInIfStatementRefactoring.Title,
                    ct => WrapInIfStatementRefactoring.Instance.RefactorAsync(context.Document, statement, ct),
                    RefactoringDescriptors.WrapStatementsInCondition);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapLinesInTryCatch))
            {
                context.RegisterRefactoring(
                    WrapLinesInTryCatchRefactoring.Title,
                    ct => WrapLinesInTryCatchRefactoring.Instance.RefactorAsync(context.Document, statement, ct),
                    RefactoringDescriptors.WrapLinesInTryCatch);
            }
        }
    }
}
