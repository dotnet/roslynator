// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

                        if (memberTypeSymbol?.IsKind(SymbolKind.ErrorType) == false)
                        {
                            ITypeSymbol expressionSymbol = semanticModel
                                .GetTypeInfo(returnStatement.Expression, context.CancellationToken)
                                .Type;

                            if (expressionSymbol?.IsKind(SymbolKind.ErrorType) == false)
                            {
                                if (memberTypeSymbol.SpecialType == SpecialType.System_Boolean
                                    && expressionSymbol.IsKind(SymbolKind.NamedType))
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

                                TypeSyntax newType = GetNewMemberType(memberSymbol, memberTypeSymbol, expressionSymbol, semanticModel);

                                if (newType != null)
                                {
                                    string newTypeName = expressionSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat);

                                    if (memberSymbol.IsKind(SymbolKind.Method)
                                        && ((IMethodSymbol)memberSymbol).IsAsync)
                                    {
                                        newTypeName = $"Task<'{newTypeName}'>";
                                    }

                                    context.RegisterRefactoring(
                                    $"Change {GetText(declaration)} type to '{newTypeName}'",
                                    cancellationToken =>
                                    {
                                        return ChangeMemberTypeRefactoring.RefactorAsync(
                                            context.Document,
                                            memberType,
                                            newType,
                                            cancellationToken);
                                    });
                                }

                                ITypeSymbol castTypeSymbol = GetCastTypeSymbol(memberSymbol, memberTypeSymbol, expressionSymbol, semanticModel);

                                if (castTypeSymbol != null)
                                    AddCastRefactoring.RegisterRefactoring(context, returnStatement.Expression, castTypeSymbol);
                            }
                        }
                    }
                }
            }
        }

        private static TypeSyntax GetNewMemberType(
            ISymbol memberSymbol,
            ITypeSymbol memberTypeSymbol,
            ITypeSymbol expressionSymbol,
            SemanticModel semanticModel)
        {
            if (memberSymbol.IsKind(SymbolKind.Method)
                && ((IMethodSymbol)memberSymbol).IsAsync)
            {
                if (ShouldRefactorAsyncMethodReturnType(memberTypeSymbol, expressionSymbol, semanticModel))
                {
                    return QualifiedName(
                        ParseName("System.Threading.Tasks"),
                        GenericName(
                            Identifier("Task"),
                            TypeArgumentList(
                                SingletonSeparatedList(
                                    TypeSyntaxRefactoring.CreateTypeSyntax(expressionSymbol)))));
                }
            }
            else if (!memberTypeSymbol.Equals(expressionSymbol))
            {
                return TypeSyntaxRefactoring.CreateTypeSyntax(expressionSymbol);
            }

            return null;
        }

        private static ITypeSymbol GetCastTypeSymbol(
            ISymbol memberSymbol,
            ITypeSymbol memberTypeSymbol,
            ITypeSymbol expressionSymbol,
            SemanticModel semanticModel)
        {
            if (memberSymbol.IsKind(SymbolKind.Method)
                && ((IMethodSymbol)memberSymbol).IsAsync)
            {
                if (memberTypeSymbol.IsKind(SymbolKind.NamedType))
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

        private static bool ShouldRefactorAsyncMethodReturnType(
            ITypeSymbol memberTypeSymbol,
            ITypeSymbol expressionSymbol,
            SemanticModel semanticModel)
        {
            INamedTypeSymbol taskSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
            INamedTypeSymbol taskOfTSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");

            if (memberTypeSymbol.SpecialType == SpecialType.System_Void)
                return true;

            if (memberTypeSymbol.Equals(taskSymbol))
                return true;

            if (memberTypeSymbol.IsKind(SymbolKind.NamedType))
            {
                var namedTypeSymbol = (INamedTypeSymbol)memberTypeSymbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(taskOfTSymbol)
                    && !namedTypeSymbol.TypeArguments[0].Equals(expressionSymbol))
                {
                    return true;
                }
            }

            return false;
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
