// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ArrowExpressionClauseRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ArrowExpressionClauseSyntax arrowExpressionClause)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ExpandExpressionBodiedMember)
                && arrowExpressionClause.Parent?.SupportsExpressionBody() == true)
            {
                context.RegisterRefactoring(
                    "Expand expression-bodied member",
                    cancellationToken => ExpandExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, arrowExpressionClause, cancellationToken));
            }
        }
    }
}