// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReturnExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression != null
                && context.SupportsSemanticModel)
            {
                MemberDeclarationSyntax declaration = GetDeclaration(expression);

                if (declaration != null)
                {
                    TypeSyntax memberType = GetMemberType(declaration);

                    if (memberType != null)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol memberTypeSymbol = semanticModel
                            .GetTypeInfo(memberType, context.CancellationToken)
                            .Type;

                        if (memberTypeSymbol?.IsErrorType() == false)
                        {
                            ITypeSymbol expressionSymbol = semanticModel
                                .GetTypeInfo(expression, context.CancellationToken)
                                .Type;

                            if (expressionSymbol?.IsErrorType() == false)
                            {
                                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddBooleanComparison)
                                    && memberTypeSymbol.SpecialType == SpecialType.System_Boolean
                                    && expressionSymbol.IsNamedType())
                                {
                                    var namedTypeSymbol = (INamedTypeSymbol)expressionSymbol;

                                    if (namedTypeSymbol?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                                        && namedTypeSymbol.TypeArguments[0].SpecialType == SpecialType.System_Boolean)
                                    {
                                        context.RegisterRefactoring(
                                            AddBooleanComparisonRefactoring.Title,
                                            cancellationToken =>
                                            {
                                                return AddBooleanComparisonRefactoring.RefactorAsync(
                                                    context.Document,
                                                    expression,
                                                    context.CancellationToken);
                                            });
                                    }
                                }

                                ISymbol memberSymbol = semanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

                                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMemberTypeAccordingToReturnExpression))
                                {
                                    ITypeSymbol newType = GetMemberNewType(memberSymbol, memberTypeSymbol, expression, expressionSymbol, semanticModel, context.CancellationToken);

                                    if (newType != null
                                        && !memberTypeSymbol.Equals(newType))
                                    {
                                        context.RegisterRefactoring(
                                        $"Change {GetText(declaration)} type to '{newType.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                                        cancellationToken =>
                                        {
                                            return TypeSyntaxRefactoring.ChangeTypeAsync(
                                                context.Document,
                                                memberType,
                                                newType,
                                                cancellationToken);
                                        });
                                    }
                                }

                                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddCastExpression))
                                {
                                    ITypeSymbol castTypeSymbol = GetCastTypeSymbol(memberSymbol, memberTypeSymbol, expressionSymbol, semanticModel);

                                    if (castTypeSymbol != null)
                                    {
                                        AddCastExpressionRefactoring.RegisterRefactoring(
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
                        var awaitableSymbol = semanticModel
                            .GetTypeInfo(awaitExpression.Expression, cancellationToken)
                            .Type as INamedTypeSymbol;

                        if (awaitableSymbol != null)
                        {
                            INamedTypeSymbol taskOfTSymbol = semanticModel
                                .Compilation
                                .GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                            if (awaitableSymbol.ConstructedFrom.Equals(taskOfTSymbol))
                                return awaitableSymbol;
                        }
                    }
                }
                else if (memberTypeSymbol.IsNamedType())
                {
                    INamedTypeSymbol taskOfTSymbol = semanticModel
                        .Compilation
                        .GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                    if (((INamedTypeSymbol)memberTypeSymbol).ConstructedFrom.Equals(taskOfTSymbol))
                    {
                        if (expressionSymbol.IsNamedType()
                            && ((INamedTypeSymbol)expressionSymbol).ConstructedFrom.Equals(taskOfTSymbol))
                        {
                            return null;
                        }

                        INamedTypeSymbol taskSymbol = semanticModel
                            .Compilation
                            .GetTypeByMetadataName("System.Threading.Tasks.Task");

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

                    INamedTypeSymbol taskOfTSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");

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

        internal static MemberDeclarationSyntax GetDeclaration(ExpressionSyntax expression)
        {
            foreach (SyntaxNode ancestor in expression.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                        return (MemberDeclarationSyntax)ancestor;
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                    case SyntaxKind.AnonymousMethodExpression:
                    case SyntaxKind.ConstructorDeclaration:
                    case SyntaxKind.DestructorDeclaration:
                    case SyntaxKind.EventDeclaration:
                    case SyntaxKind.ConversionOperatorDeclaration:
                    case SyntaxKind.OperatorDeclaration:
                    case SyntaxKind.IncompleteMember:
                        return null;
                }
            }

            return null;
        }
    }
}
