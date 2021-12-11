// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AnonymousMethodExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.UseLambdaInsteadOfAnonymousMethod)
                && UseLambdaInsteadOfAnonymousMethodAnalysis.IsFixable(anonymousMethod))
            {
                context.RegisterRefactoring(
                    "Use lambda instead of anonymous method",
                    ct => UseLambdaInsteadOfAnonymousMethodRefactoring.RefactorAsync(context.Document, anonymousMethod, ct),
                    RefactoringDescriptors.UseLambdaInsteadOfAnonymousMethod);
            }
        }
    }
}
