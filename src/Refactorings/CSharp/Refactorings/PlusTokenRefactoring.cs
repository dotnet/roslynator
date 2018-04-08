// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class PlusTokenRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxToken token)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.JoinStringExpressions,
                    RefactoringIdentifiers.UseStringBuilderInsteadOfConcatenation)
                && context.Span.IsEmptyAndContainedInSpan(token)
                && token.IsParentKind(SyntaxKind.AddExpression))
            {
                var addExpresion = (BinaryExpressionSyntax)token.Parent;

                while (addExpresion.IsParentKind(SyntaxKind.AddExpression))
                    addExpresion = (BinaryExpressionSyntax)addExpresion.Parent;

                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                StringConcatenationExpressionInfo concatenationInfo = SyntaxInfo.StringConcatenationExpressionInfo(addExpresion, semanticModel, context.CancellationToken);

                if (concatenationInfo.Success)
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.JoinStringExpressions))
                        JoinStringExpressionsRefactoring.ComputeRefactoring(context, concatenationInfo);

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseStringBuilderInsteadOfConcatenation))
                        UseStringBuilderInsteadOfConcatenationRefactoring.ComputeRefactoring(context, concatenationInfo);
                }
            }
        }
    }
}
