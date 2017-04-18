// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReturnExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression != null)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ISymbol memberSymbol = GetContainingMethodOrPropertySymbol(expression, semanticModel, context.CancellationToken);

                if (memberSymbol != null)
                {
                    SyntaxNode node = await memberSymbol
                        .DeclaringSyntaxReferences[0]
                        .GetSyntaxAsync(context.CancellationToken)
                        .ConfigureAwait(false);

                    var declaration = node as MemberDeclarationSyntax;

                    if (declaration != null)
                    {
                        TypeSyntax memberType = GetMemberType(declaration);

                        if (memberType != null)
                        {
                            ITypeSymbol memberTypeSymbol = semanticModel.GetTypeSymbol(memberType, context.CancellationToken);

                            if (memberTypeSymbol != null)
                            {
                                ITypeSymbol expressionSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                                if (expressionSymbol?.IsErrorType() == false)
                                {
                                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMemberTypeAccordingToReturnExpression))
                                    {
                                        ITypeSymbol newType = GetMemberNewType(memberSymbol, memberTypeSymbol, expression, expressionSymbol, semanticModel, context.CancellationToken);

                                        if (newType?.IsErrorType() == false
                                            && !memberTypeSymbol.Equals(newType)
                                            && !memberSymbol.IsOverride
                                            && !memberSymbol.ImplementsInterfaceMember())
                                        {
                                            if (newType.IsNamedType() && memberTypeSymbol.IsNamedType())
                                            {
                                                var newNamedType = (INamedTypeSymbol)newType;

                                                INamedTypeSymbol orderedEnumerableSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_IOrderedEnumerable_T);

                                                if (newNamedType.ConstructedFrom == orderedEnumerableSymbol)
                                                {
                                                    INamedTypeSymbol enumerableSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Collections_Generic_IEnumerable_T);

                                                    if (enumerableSymbol != null
                                                        && ((INamedTypeSymbol)memberTypeSymbol).ConstructedFrom != enumerableSymbol)
                                                    {
                                                        RegisterChangeType(context, declaration, memberType, enumerableSymbol.Construct(newNamedType.TypeArguments.ToArray()), semanticModel);
                                                    }
                                                }
                                            }

                                            RegisterChangeType(context, declaration, memberType, newType, semanticModel);
                                        }
                                    }

                                    if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.CallToMethod)
                                        && !memberTypeSymbol.IsErrorType())
                                    {
                                        ITypeSymbol castTypeSymbol = GetCastTypeSymbol(memberSymbol, memberTypeSymbol, expressionSymbol, semanticModel);

                                        if (castTypeSymbol != null)
                                        {
                                            ModifyExpressionRefactoring.ComputeRefactoring(
                                               context,
                                               expression,
                                               castTypeSymbol,
                                               semanticModel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void RegisterChangeType(RefactoringContext context, MemberDeclarationSyntax member, TypeSyntax type, ITypeSymbol newType, SemanticModel semanticModel)
        {
            context.RegisterRefactoring(
            $"Change {GetText(member)} type to '{SymbolDisplay.GetMinimalString(newType, semanticModel, type.Span.Start)}'",
            cancellationToken =>
            {
                return ChangeTypeRefactoring.ChangeTypeAsync(
                    context.Document,
                    type,
                    newType,
                    cancellationToken);
            });
        }

        private static ITypeSymbol GetMemberNewType(
            ISymbol memberSymbol,
            ITypeSymbol memberTypeSymbol,
            ExpressionSyntax expression,
            ITypeSymbol expressionSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (memberSymbol.IsAsyncMethod())
            {
                if (expression.IsKind(SyntaxKind.AwaitExpression))
                {
                    var awaitExpression = (AwaitExpressionSyntax)expression;

                    if (awaitExpression.Expression != null)
                    {
                        var awaitableSymbol = semanticModel.GetTypeSymbol(awaitExpression.Expression, cancellationToken) as INamedTypeSymbol;

                        if (awaitableSymbol != null)
                        {
                            INamedTypeSymbol taskOfTSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

                            if (awaitableSymbol.ConstructedFrom.Equals(taskOfTSymbol))
                                return awaitableSymbol;
                        }
                    }
                }
                else if (memberTypeSymbol.IsNamedType())
                {
                    INamedTypeSymbol taskOfTSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

                    if (((INamedTypeSymbol)memberTypeSymbol).ConstructedFrom.Equals(taskOfTSymbol))
                    {
                        if (expressionSymbol.IsNamedType()
                            && ((INamedTypeSymbol)expressionSymbol).ConstructedFrom.Equals(taskOfTSymbol))
                        {
                            return null;
                        }

                        INamedTypeSymbol taskSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task);

                        if (expressionSymbol.Equals(taskSymbol))
                            return null;

                        return taskOfTSymbol.Construct(expressionSymbol);
                    }
                }
            }
            else
            {
                return expressionSymbol;
            }

            return null;
        }

        private static ITypeSymbol GetCastTypeSymbol(
            ISymbol memberSymbol,
            ITypeSymbol memberTypeSymbol,
            ITypeSymbol expressionSymbol,
            SemanticModel semanticModel)
        {
            if (memberSymbol.IsAsyncMethod())
            {
                if (memberTypeSymbol.IsNamedType())
                {
                    var namedTypeSymbol = (INamedTypeSymbol)memberTypeSymbol;

                    INamedTypeSymbol taskOfTSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

                    if (taskOfTSymbol != null
                        && namedTypeSymbol.ConstructedFrom.Equals(taskOfTSymbol)
                        && !namedTypeSymbol.TypeArguments[0].Equals(expressionSymbol))
                    {
                        return namedTypeSymbol.TypeArguments[0];
                    }
                }
            }
            else if (!memberTypeSymbol.Equals(expressionSymbol))
            {
                return memberTypeSymbol;
            }

            return null;
        }

        internal static TypeSyntax GetMemberType(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).ReturnType;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).Type;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).Type;
                default:
                    return null;
            }
        }

        internal static string GetText(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return "return";
                case SyntaxKind.PropertyDeclaration:
                    return "property";
                case SyntaxKind.IndexerDeclaration:
                    return "indexer";
                default:
                    return null;
            }
        }

        internal static ISymbol GetContainingMethodOrPropertySymbol(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ISymbol symbol = semanticModel.GetEnclosingSymbol(expression.SpanStart, cancellationToken);

            if (symbol?.IsMethod() == true)
            {
                var methodsymbol = (IMethodSymbol)symbol;
                MethodKind methodKind = methodsymbol.MethodKind;

                if (methodKind == MethodKind.Ordinary)
                {
                    if (methodsymbol.PartialImplementationPart != null)
                        symbol = methodsymbol.PartialImplementationPart;
                }
                else if (methodKind == MethodKind.PropertyGet)
                {
                    symbol = methodsymbol.AssociatedSymbol;
                }
            }

            return symbol;
        }
    }
}
