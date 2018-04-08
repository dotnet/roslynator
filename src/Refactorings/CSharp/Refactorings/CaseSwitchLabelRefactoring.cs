// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CaseSwitchLabelRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, CaseSwitchLabelSyntax caseLabel)
        {
            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.CallToMethod))
            {
                ExpressionSyntax value = caseLabel.Value;

                if (value?.Span.Contains(context.Span) == true
                    && (caseLabel.Parent?.Parent is SwitchStatementSyntax switchStatement))
                {
                    ExpressionSyntax expression = switchStatement.Expression;

                    if (expression?.IsMissing == false)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                        if (typeSymbol?.IsErrorType() == false)
                            ModifyExpressionRefactoring.ComputeRefactoring(context, value, typeSymbol, semanticModel);
                    }
                }
            }
        }
    }
}