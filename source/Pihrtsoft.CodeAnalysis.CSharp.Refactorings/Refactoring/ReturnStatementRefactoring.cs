// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReturnStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ReturnStatementSyntax returnStatement)
        {
            if (returnStatement.Expression != null
                && returnStatement.Expression.Span.Contains(context.Span)
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
                            ITypeSymbol typeSymbol = semanticModel
                                .GetTypeInfo(returnStatement.Expression, context.CancellationToken)
                                .Type;

                            if (typeSymbol?.IsKind(SymbolKind.ErrorType) == false)
                            {
                                if (memberTypeSymbol.SpecialType == SpecialType.System_Boolean
                                    && typeSymbol.IsKind(SymbolKind.NamedType))
                                {
                                    var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                                    if (namedTypeSymbol?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                                        && namedTypeSymbol.TypeArguments[0].SpecialType == SpecialType.System_Boolean)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            AddBooleanComparisonRefactoring.Title,
                                            cancellationToken =>
                                            {
                                                return AddBooleanComparisonRefactoring.RefactorAsync(
                                                    context.Document,
                                                    returnStatement.Expression,
                                                    context.CancellationToken);
                                            });

                                        context.RegisterRefactoring(codeAction);
                                    }
                                }

                                if (!memberTypeSymbol.Equals(typeSymbol))
                                {
                                    TypeSyntax newType = TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol);

                                    if (newType != null)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            $"Change {GetText(declaration)} type to '{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                                            cancellationToken =>
                                            {
                                                return ChangeReturnTypeAsync(
                                                    context.Document,
                                                    memberType,
                                                    newType,
                                                    cancellationToken);
                                            });

                                        context.RegisterRefactoring(codeAction);
                                    }

                                    AddCastRefactoring.Refactor(context, returnStatement.Expression, memberTypeSymbol);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static async Task<Document> ChangeReturnTypeAsync(
            Document document,
            TypeSyntax type,
            TypeSyntax newType,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newNode = SetNewType(
                type.Parent,
                newType.WithAdditionalAnnotations(Simplifier.Annotation));

            SyntaxNode newRoot = oldRoot.ReplaceNode(type.Parent, newNode);

            return document.WithSyntaxRoot(newRoot);
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

        private static SyntaxNode SetNewType(SyntaxNode node, TypeSyntax newType)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var declaration = (MethodDeclarationSyntax)node;
                        return declaration.WithReturnType(newType.WithTriviaFrom(declaration.ReturnType));
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var declaration = (PropertyDeclarationSyntax)node;
                        return declaration.WithType(newType.WithTriviaFrom(declaration.Type));
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var declaration = (IndexerDeclarationSyntax)node;
                        return declaration.WithType(newType.WithTriviaFrom(declaration.Type));
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
