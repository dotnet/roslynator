// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AnonymousMethodExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseLambdaExpressionInsteadOfAnonymousMethod)
                && UseLambdaExpressionInsteadOfAnonymousMethodRefactoring.CanRefactor(anonymousMethod))
            {
                context.RegisterRefactoring(
                    "Use lambda expression instead of anonymous method",
                    cancellationToken =>
                    {
                        return UseLambdaExpressionInsteadOfAnonymousMethodRefactoring.RefactorAsync(
                            context.Document,
                            anonymousMethod,
                            cancellationToken);
                    });
            }
        }
    }
}
