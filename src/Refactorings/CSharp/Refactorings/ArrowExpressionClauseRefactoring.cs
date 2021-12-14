// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ArrowExpressionClauseRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ArrowExpressionClauseSyntax arrowExpressionClause)
        {
            ExpressionSyntax expression = arrowExpressionClause.Expression;

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertExpressionBodyToBlockBody)
                && (context.Span.IsEmptyAndContainedInSpan(arrowExpressionClause) || context.Span.IsBetweenSpans(expression))
                && ExpandExpressionBodyAnalysis.IsFixable(arrowExpressionClause))
            {
                context.RegisterRefactoring(
                    ConvertExpressionBodyToBlockBodyRefactoring.Title,
                    ct => ConvertExpressionBodyToBlockBodyRefactoring.RefactorAsync(context.Document, arrowExpressionClause, ct),
                    RefactoringDescriptors.ConvertExpressionBodyToBlockBody);
            }
        }
    }
}
