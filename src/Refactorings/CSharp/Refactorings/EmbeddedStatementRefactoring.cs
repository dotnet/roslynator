// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInCondition))
            {
                context.RegisterRefactoring(
                    WrapInIfStatementRefactoring.Title,
                    ct => WrapInIfStatementRefactoring.Instance.RefactorAsync(context.Document, statement, ct));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInTryCatch))
            {
                context.RegisterRefactoring(
                    WrapInTryCatchRefactoring.Title,
                    ct => WrapInTryCatchRefactoring.Instance.RefactorAsync(context.Document, statement, ct));
            }
        }
    }
}
