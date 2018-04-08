// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AnonymousMethodExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseLambdaExpressionInsteadOfAnonymousMethod)
                && UseLambdaExpressionInsteadOfAnonymousMethodAnalysis.IsFixable(anonymousMethod))
            {
                context.RegisterRefactoring(
                    "Use lambda expression instead of anonymous method",
                    ct => UseLambdaExpressionInsteadOfAnonymousMethodRefactoring.RefactorAsync(context.Document, anonymousMethod, ct));
            }
        }
    }
}
