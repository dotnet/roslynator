// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class YieldReturnStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, YieldStatementSyntax yieldStatement)
        {
            if (yieldStatement.IsYieldReturn()
                && yieldStatement.Expression?.Span.Contains(context.Span) == true
                && context.SupportsSemanticModel)
            {
                MemberDeclarationSyntax declaration = ReturnStatementRefactoring.GetDeclaration(yieldStatement);

                if (declaration != null)
                {
                    TypeSyntax memberType = ReturnStatementRefactoring.GetMemberType(declaration);

                    if (memberType != null)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync();

                        ITypeSymbol memberTypeSymbol = semanticModel
                            .GetTypeInfo(memberType, context.CancellationToken)
                            .Type;

                        if (memberTypeSymbol.SpecialType != SpecialType.System_Collections_IEnumerable)
                        {
                            ITypeSymbol typeSymbol = semanticModel
                                .GetTypeInfo(yieldStatement.Expression, context.CancellationToken)
                                .Type;

                            if (typeSymbol?.IsKind(SymbolKind.ErrorType) == false
                                && (memberTypeSymbol == null
                                    || memberTypeSymbol.IsKind(SymbolKind.ErrorType)
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
                                    $"Change {ReturnStatementRefactoring.GetText(declaration)} type to 'IEnumerable<{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}>'",
                                    cancellationToken =>
                                    {
                                        return ChangeMemberTypeRefactoring.RefactorAsync(
                                            context.Document,
                                            memberType,
                                            newType,
                                            cancellationToken);
                                    });
                            }
                        }
                    }
                }
            }
        }
    }
}
