// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsingStatementRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, UsingStatementSyntax usingStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceLocalVariable))
            {
                ExpressionSyntax expression = usingStatement.Expression;

                if (expression != null)
                {
                    context.RegisterRefactoring(
                        IntroduceLocalVariableRefactoring.GetTitle(expression),
                        cancellationToken => IntroduceLocalVariableRefactoring.RefactorAsync(context.Document, usingStatement, expression, cancellationToken));
                }
            }
        }
    }
}