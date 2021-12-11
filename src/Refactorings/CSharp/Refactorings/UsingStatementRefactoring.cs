// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsingStatementRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, UsingStatementSyntax usingStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.IntroduceLocalVariable))
            {
                ExpressionSyntax expression = usingStatement.Expression;

                if (expression != null)
                {
                    context.RegisterRefactoring(
                        IntroduceLocalVariableRefactoring.GetTitle(expression),
                        ct => IntroduceLocalVariableRefactoring.RefactorAsync(context.Document, usingStatement, expression, ct),
                        RefactoringDescriptors.IntroduceLocalVariable);
                }
            }
        }
    }
}
