// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DoStatementRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, DoStatementSyntax doStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertDoToWhile)
                && (doStatement.DoKeyword.Span.Contains(context.Span)))
            {
                context.RegisterRefactoring(
                    "Convert to 'while'",
                    ct => ConvertDoToWhileRefactoring.RefactorAsync(context.Document, doStatement, ct),
                    RefactoringIdentifiers.ConvertDoToWhile);
            }
        }
    }
}
