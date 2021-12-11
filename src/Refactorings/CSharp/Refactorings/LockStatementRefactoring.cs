// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class LockStatementRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, LockStatementSyntax lockStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.IntroduceFieldToLockOn))
            {
                ExpressionSyntax expression = lockStatement.Expression;

                if (expression != null)
                {
                    if (expression.IsMissing || expression.IsKind(SyntaxKind.ThisExpression))
                    {
                        if (context.Span.IsContainedInSpanOrBetweenSpans(expression))
                        {
                            context.RegisterRefactoring(
                                "Introduce field to lock on",
                                ct => IntroduceFieldToLockOnRefactoring.RefactorAsync(context.Document, lockStatement, ct),
                                RefactoringDescriptors.IntroduceFieldToLockOn);
                        }
                    }
                }
            }
        }
    }
}
