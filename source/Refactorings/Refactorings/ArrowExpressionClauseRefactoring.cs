// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ArrowExpressionClauseRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArrowExpressionClauseSyntax arrowExpressionClause)
        {
            if (arrowExpressionClause.Expression != null
                && context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.AddBooleanComparison,
                    RefactoringIdentifiers.ChangeMemberTypeAccordingToReturnExpression,
                    RefactoringIdentifiers.AddCastExpression,
                    RefactoringIdentifiers.AddToMethodInvocation)
                && context.SupportsSemanticModel)
            {
                await ReturnExpressionRefactoring.ComputeRefactoringsAsync(context, arrowExpressionClause.Expression).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandExpressionBodiedMember)
                && arrowExpressionClause.Parent?.SupportsExpressionBody() == true)
            {
                context.RegisterRefactoring(
                    "Expand expression-bodied member",
                    cancellationToken => ExpandExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, arrowExpressionClause, cancellationToken));
            }
        }
    }
}