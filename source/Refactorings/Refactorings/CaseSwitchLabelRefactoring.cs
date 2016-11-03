// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CaseSwitchLabelRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, CaseSwitchLabelSyntax caseLabel)
        {
            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.AddToMethodInvocation)
                && caseLabel.Value?.Span.Contains(context.Span) == true)
            {
                var switchStatement = caseLabel.Parent?.Parent as SwitchStatementSyntax;

                if (switchStatement?.Expression?.IsMissing == false)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol typeSymbol = semanticModel
                        .GetTypeInfo(switchStatement.Expression, context.CancellationToken)
                        .Type;

                    if (typeSymbol?.IsErrorType() == false)
                        ModifyExpressionRefactoring.ComputeRefactoring(context, caseLabel.Value, typeSymbol, semanticModel);
                }
            }
        }
    }
}