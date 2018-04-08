// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ReplaceStatementWithIf;

namespace Roslynator.CSharp.Refactorings
{
    internal static class YieldStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, YieldStatementSyntax yieldStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CallToMethod)
                && yieldStatement.Kind() == SyntaxKind.YieldReturnStatement)
            {
                ExpressionSyntax expression = yieldStatement.Expression;

                if (expression?.IsMissing == false
                    && expression.Span.Contains(context.Span))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    (ISymbol memberSymbol, ITypeSymbol memberTypeSymbol) = ReturnExpressionRefactoring.GetContainingSymbolAndType(expression, semanticModel, context.CancellationToken);

                    if (memberSymbol != null
                        && (memberTypeSymbol is INamedTypeSymbol namedTypeSymbol)
                        && namedTypeSymbol.SpecialType != SpecialType.System_Collections_IEnumerable
                        && namedTypeSymbol.OriginalDefinition.IsIEnumerableOfT())
                    {
                        ITypeSymbol argumentSymbol = namedTypeSymbol.TypeArguments[0];

                        ITypeSymbol expressionTypeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                        if (argumentSymbol != expressionTypeSymbol)
                        {
                            ModifyExpressionRefactoring.ComputeRefactoring(
                               context,
                               expression,
                               argumentSymbol,
                               semanticModel,
                               addCastExpression: false);
                        }
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStatementWithIfElse)
                && (context.Span.IsEmptyAndContainedInSpan(yieldStatement.YieldKeyword)
                    || context.Span.IsEmptyAndContainedInSpan(yieldStatement.ReturnOrBreakKeyword)
                    || context.Span.IsBetweenSpans(yieldStatement)))
            {
                await ReplaceStatementWithIfStatementRefactoring.ReplaceYieldReturnWithIfElse.ComputeRefactoringAsync(context, yieldStatement).ConfigureAwait(false);
            }
        }
    }
}
