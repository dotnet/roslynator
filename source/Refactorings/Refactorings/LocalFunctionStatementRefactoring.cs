// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class LocalFunctionStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LocalFunctionStatementSyntax localFunctionStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddTypeParameter))
                AddTypeParameterRefactoring.ComputeRefactoring(context, localFunctionStatement);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && localFunctionStatement.Body?.Span.Contains(context.Span) == true
                && UseExpressionBodiedMemberRefactoring.CanRefactor(localFunctionStatement))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, localFunctionStatement, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseListInsteadOfYield)
                && localFunctionStatement.Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                UseListInsteadOfYieldRefactoring.ComputeRefactoring(context, localFunctionStatement, semanticModel);
            }
        }
    }
}
