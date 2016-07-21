// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReturnStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ReturnStatementSyntax returnStatement)
        {
            if (returnStatement.Expression != null
                && context.SupportsSemanticModel)
            {
                MemberDeclarationSyntax declaration = GetDeclaration(returnStatement);

                if (declaration != null)
                {
                    TypeSyntax memberType = GetMemberType(declaration);

                    if (memberType != null)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync();

                        ITypeSymbol memberTypeSymbol = semanticModel
                            .GetTypeInfo(memberType, context.CancellationToken)
                            .Type;

                        if (memberTypeSymbol?.IsErrorType() == false)
                        {
                            ITypeSymbol expressionSymbol = semanticModel
                                .GetTypeInfo(returnStatement.Expression, context.CancellationToken)
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
                                                    returnStatement.Expression,
                                                    context.CancellationToken);
                                            });
                                    }
                                }

                                ISymbol memberSymbol = semanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

                                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMemberTypeAccordingToReturnExpression))
                                {
                                    ITypeSymbol newType = GetMemberNewType(memberSymbol, memberTypeSymbol, returnStatement.Expression, expressionSymbol, semanticModel, context.CancellationToken);

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
                                           returnStatement.Expression,
                                           castTypeSymbol,
                                           semanticModel);
                                    }
                                }
                            }
                        }
                    }
                }

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceReturnStatementWithIfStatement))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync();

                    ITypeSymbol expressionSymbol = semanticModel
                        .GetTypeInfo(returnStatement.Expression, context.CancellationToken)
                        .ConvertedType;

                    if (expressionSymbol?.SpecialType == SpecialType.System_Boolean)
                    {
                        context.RegisterRefactoring(
                            "Replace return statement with if statement",
                            cancellationToken =>
                            {
                                return ReplaceReturnStatementWithIfStatementRefactoring.RefactorAsync(
                                    context.Document,
                                    returnStatement,
                                    cancellationToken);
                            });
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
                    return "method's return";
                case SyntaxKind.PropertyDeclaration:
                    return "property";
                case SyntaxKind.IndexerDeclaration:
                    return "indexer";
                default:
                    return null;
            }
        }

        internal static MemberDeclarationSyntax GetDeclaration(StatementSyntax statement)
        {
            foreach (SyntaxNode ancestor in statement.Ancestors())
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
