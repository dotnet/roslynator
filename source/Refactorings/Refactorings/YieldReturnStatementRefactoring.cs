// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class YieldReturnStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, YieldStatementSyntax yieldStatement)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ChangeMemberTypeAccordingToYieldReturnExpression,
                    RefactoringIdentifiers.AddCastExpression,
                    RefactoringIdentifiers.CreateConditionFromBooleanExpression)
                && yieldStatement.IsYieldReturn()
                && yieldStatement.Expression != null
                && context.SupportsSemanticModel)
            {
                if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ChangeMemberTypeAccordingToYieldReturnExpression,
                    RefactoringIdentifiers.AddCastExpression))
                {
                    MemberDeclarationSyntax containingMember = ReturnExpressionRefactoring.GetContainingMember(yieldStatement.Expression);

                    if (containingMember != null)
                    {
                        TypeSyntax memberType = ReturnExpressionRefactoring.GetMemberType(containingMember);

                        if (memberType != null)
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol memberTypeSymbol = semanticModel
                                .GetTypeInfo(memberType, context.CancellationToken)
                                .Type;

                            if (memberTypeSymbol?.SpecialType != SpecialType.System_Collections_IEnumerable)
                            {
                                ITypeSymbol typeSymbol = semanticModel
                                    .GetTypeInfo(yieldStatement.Expression, context.CancellationToken)
                                    .Type;

                                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMemberTypeAccordingToYieldReturnExpression)
                                    && typeSymbol?.IsErrorType() == false
                                    && (memberTypeSymbol == null
                                        || memberTypeSymbol.IsErrorType()
                                        || !memberTypeSymbol.IsGenericIEnumerable()
                                        || !((INamedTypeSymbol)memberTypeSymbol).TypeArguments[0].Equals(typeSymbol)))
                                {
                                    TypeSyntax newType = QualifiedName(
                                        ParseName("System.Collections.Generic"),
                                        GenericName(
                                            Identifier("IEnumerable"),
                                            TypeArgumentList(
                                                SingletonSeparatedList(
                                                    TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol)))));

                                    context.RegisterRefactoring(
                                        $"Change {ReturnExpressionRefactoring.GetText(containingMember)} type to 'IEnumerable<{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}>'",
                                        cancellationToken =>
                                        {
                                            return TypeSyntaxRefactoring.ChangeTypeAsync(
                                                context.Document,
                                                memberType,
                                                newType,
                                                cancellationToken);
                                        });
                                }

                                if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddCastExpression)
                                    && yieldStatement.Expression.Span.Contains(context.Span)
                                    && memberTypeSymbol?.IsNamedType() == true)
                                {
                                    var namedTypeSymbol = (INamedTypeSymbol)memberTypeSymbol;

                                    if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
                                    {
                                        ITypeSymbol argumentSymbol = namedTypeSymbol.TypeArguments[0];

                                        if (argumentSymbol != typeSymbol)
                                        {
                                            AddCastExpressionRefactoring.RegisterRefactoring(
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

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.CreateConditionFromBooleanExpression))
                    await CreateConditionFromBooleanExpressionRefactoring.ComputeRefactoringAsync(context, yieldStatement.Expression).ConfigureAwait(false);
            }
        }
    }
}
