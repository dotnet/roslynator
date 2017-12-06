// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.WrapStatements;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EmbeddedStatementRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            if (!statement.IsEmbedded())
                return;

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInCondition))
            {
                context.RegisterRefactoring(
                    WrapInIfStatementRefactoring.Title,
                    cancellationToken =>
                    {
                        var refactoring = new WrapInIfStatementRefactoring();
                        return refactoring.RefactorAsync(context.Document, statement, cancellationToken);
                    });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInTryCatch))
            {
                context.RegisterRefactoring(
                    WrapInTryCatchRefactoring.Title,
                    cancellationToken =>
                    {
                        var refactoring = new WrapInTryCatchRefactoring();
                        return refactoring.RefactorAsync(context.Document, statement, cancellationToken);
                    });
            }
        }
    }
}
