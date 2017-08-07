// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CastExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, CastExpressionSyntax castExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceCastWithAs)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(castExpression))
            {
                ReplaceCastWithAsRefactoring.ComputeRefactoring(context, castExpression);
            }
        }
    }
}