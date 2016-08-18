// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class ReturnStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ReturnStatementSyntax returnStatement)
        {
            if (context.SupportsSemanticModel)
            {
                if (returnStatement.Expression != null)
                {
                    if (context.Settings.IsAnyRefactoringEnabled(
                        RefactoringIdentifiers.AddBooleanComparison,
                        RefactoringIdentifiers.ChangeMemberTypeAccordingToReturnExpression,
                        RefactoringIdentifiers.AddCastExpression))
                    {
                        await ReturnExpressionRefactoring.ComputeRefactoringsAsync(context, returnStatement.Expression).ConfigureAwait(false);
                    }

                    if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceBooleanExpressionWithIfStatement))
                        await ReplaceBooleanExpressionWithIfStatementRefactoring.ComputeRefactoringAsync(context, returnStatement.Expression);
                }
                else if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddDefaultValueToReturnStatement))
                {
                    await AddDefaultValueToReturnStatementRefactoring.ComputeRefactoringsAsync(context, returnStatement);
                }
            }
        }
    }
}
