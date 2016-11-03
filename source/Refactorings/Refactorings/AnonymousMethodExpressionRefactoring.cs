// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AnonymousMethodExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceAnonymousMethodWithLambdaExpression)
                && ReplaceAnonymousMethodWithLambdaExpressionRefactoring.CanRefactor(anonymousMethod))
            {
                context.RegisterRefactoring(
                    "Replace anonymous method with lambda expression",
                    cancellationToken =>
                    {
                        return ReplaceAnonymousMethodWithLambdaExpressionRefactoring.RefactorAsync(
                            context.Document,
                            anonymousMethod,
                            cancellationToken);
                    });
            }
        }
    }
}
