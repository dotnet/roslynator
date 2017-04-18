// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ReplaceStatementWithIf;
using Roslynator.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpressionStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ExpressionStatementSyntax expressionStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddIdentifierToVariableDeclaration))
                await AddIdentifierToLocalDeclarationRefactoring.ComputeRefactoringAsync(context, expressionStatement).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceLocalFromStatementThatReturnsValue))
            {
                ExpressionSyntax expression = expressionStatement.Expression;

                if (expression?.IsMissing == false
                    && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(expression))
                {
                    await IntroduceLocalFromStatementThatReturnsValueRefactoring.ComputeRefactoringAsync(context, expressionStatement, expression).ConfigureAwait(false);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStatementWithIfStatement)
                && context.Span.IsBetweenSpans(expressionStatement))
            {
                var refactoring = new ReplaceExpressionStatementWithIfStatementRefactoring();
                await refactoring.ComputeRefactoringAsync(context, expressionStatement).ConfigureAwait(false);
            }
        }
    }
}