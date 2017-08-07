// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ReplaceStatementWithIf;

namespace Roslynator.CSharp.Refactorings
{
    internal static class YieldStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, YieldStatementSyntax yieldStatement)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ChangeMemberTypeAccordingToYieldReturnExpression,
                    RefactoringIdentifiers.AddCastExpression,
                    RefactoringIdentifiers.CallToMethod)
                && yieldStatement.IsYieldReturn()
                && yieldStatement.Expression != null)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ISymbol memberSymbol = ReturnExpressionRefactoring.GetContainingMethodOrPropertySymbol(yieldStatement.Expression, semanticModel, context.CancellationToken);

                if (memberSymbol != null)
                {
                    SyntaxNode node = await memberSymbol
                        .DeclaringSyntaxReferences[0]
                        .GetSyntaxAsync(context.CancellationToken)
                        .ConfigureAwait(false);

                    TypeSyntax type = ReturnExpressionRefactoring.GetTypeOrReturnType(node);

                    if (type != null)
                    {
                        ITypeSymbol memberTypeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                        if (memberTypeSymbol?.SpecialType != SpecialType.System_Collections_IEnumerable)
                        {
                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(yieldStatement.Expression, context.CancellationToken);

                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMemberTypeAccordingToYieldReturnExpression)
                                && typeSymbol?.IsErrorType() == false
                                && !typeSymbol.IsVoid()
                                && !memberSymbol.IsOverride
                                && (memberTypeSymbol == null
                                    || memberTypeSymbol.IsErrorType()
                                    || !memberTypeSymbol.IsConstructedFromIEnumerableOfT()
                                    || !((INamedTypeSymbol)memberTypeSymbol).TypeArguments[0].Equals(typeSymbol)))
                            {
                                INamedTypeSymbol newTypeSymbol = semanticModel
                                    .Compilation
                                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
                                    .Construct(typeSymbol);

                                TypeSyntax newType = newTypeSymbol.ToMinimalTypeSyntax(semanticModel, type.SpanStart);

                                context.RegisterRefactoring(
                                    $"Change {ReturnExpressionRefactoring.GetText(node)} type to '{SymbolDisplay.GetMinimalString(newTypeSymbol, semanticModel, type.SpanStart)}'",
                                    cancellationToken =>
                                    {
                                        return ChangeTypeRefactoring.ChangeTypeAsync(
                                            context.Document,
                                            type,
                                            newType,
                                            cancellationToken);
                                    });
                            }

                            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.CallToMethod)
                                && yieldStatement.Expression.Span.Contains(context.Span)
                                && memberTypeSymbol?.IsNamedType() == true)
                            {
                                var namedTypeSymbol = (INamedTypeSymbol)memberTypeSymbol;

                                if (namedTypeSymbol.IsConstructedFromIEnumerableOfT())
                                {
                                    ITypeSymbol argumentSymbol = namedTypeSymbol.TypeArguments[0];

                                    if (argumentSymbol != typeSymbol)
                                    {
                                        ModifyExpressionRefactoring.ComputeRefactoring(
                                           context,
                                           yieldStatement.Expression,
                                           argumentSymbol,
                                           semanticModel);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStatementWithIfStatement)
                && context.Span.IsBetweenSpans(yieldStatement))
            {
                var refactoring = new ReplaceYieldStatementWithIfStatementRefactoring();
                await refactoring.ComputeRefactoringAsync(context, yieldStatement).ConfigureAwait(false);
            }
        }
    }
}
