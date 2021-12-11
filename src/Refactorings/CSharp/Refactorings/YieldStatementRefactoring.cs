// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ConvertReturnToIf;

namespace Roslynator.CSharp.Refactorings
{
    internal static class YieldStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, YieldStatementSyntax yieldStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertReturnStatementToIf)
                && (context.Span.IsEmptyAndContainedInSpan(yieldStatement.YieldKeyword)
                    || context.Span.IsEmptyAndContainedInSpan(yieldStatement.ReturnOrBreakKeyword)
                    || context.Span.IsBetweenSpans(yieldStatement)))
            {
                await ConvertReturnStatementToIfRefactoring.ConvertYieldReturnToIfElse.ComputeRefactoringAsync(context, yieldStatement).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.UseListInsteadOfYield)
                && yieldStatement.IsYieldReturn()
                && context.Span.IsEmptyAndContainedInSpan(yieldStatement.YieldKeyword))
            {
                SyntaxNode declaration = yieldStatement.FirstAncestor(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement, SyntaxKind.GetAccessorDeclaration, ascendOutOfTrivia: false);

                Debug.Assert(declaration != null);

                if (declaration != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    UseListInsteadOfYieldRefactoring.ComputeRefactoring(context, declaration, semanticModel);
                }
            }
        }
    }
}
