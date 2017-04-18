// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ReplaceStatementWithIf;
using Roslynator.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReturnStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ReturnStatementSyntax returnStatement)
        {
            ExpressionSyntax expression = returnStatement.Expression;

            if (expression != null)
            {
                if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.AddBooleanComparison,
                    RefactoringIdentifiers.ChangeMemberTypeAccordingToReturnExpression,
                    RefactoringIdentifiers.AddCastExpression,
                    RefactoringIdentifiers.CallToMethod))
                {
                    await ReturnExpressionRefactoring.ComputeRefactoringsAsync(context, expression).ConfigureAwait(false);
                }

                if (context.Span.IsBetweenSpans(returnStatement))
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStatementWithIfStatement))
                    {
                        var refactoring = new ReplaceReturnStatementWithIfStatementRefactoring();
                        await refactoring.ComputeRefactoringAsync(context, returnStatement).ConfigureAwait(false);
                    }

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInElseClause))
                        WrapInElseClauseRefactoring.ComputeRefactoring(context, returnStatement);
                }
            }
            else if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddDefaultValueToReturnStatement))
            {
                await AddDefaultValueToReturnStatementRefactoring.ComputeRefactoringsAsync(context, returnStatement).ConfigureAwait(false);
            }
        }
    }
}
