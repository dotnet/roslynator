// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ArrowExpressionClauseRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArrowExpressionClauseSyntax arrowExpressionClause)
        {
            ExpressionSyntax expression = arrowExpressionClause.Expression;

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CallToMethod))
                await ReturnExpressionRefactoring.ComputeRefactoringsAsync(context, expression).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertExpressionBodyToBlockBody)
                && (context.Span.IsEmptyAndContainedInSpan(arrowExpressionClause) || context.Span.IsBetweenSpans(expression))
                && ExpandExpressionBodyAnalysis.IsFixable(arrowExpressionClause))
            {
                context.RegisterRefactoring(
                    ConvertExpressionBodyToBlockBodyRefactoring.Title,
                    cancellationToken => ConvertExpressionBodyToBlockBodyRefactoring.RefactorAsync(context.Document, arrowExpressionClause, cancellationToken),
                    RefactoringIdentifiers.ConvertExpressionBodyToBlockBody);
            }
        }
    }
}