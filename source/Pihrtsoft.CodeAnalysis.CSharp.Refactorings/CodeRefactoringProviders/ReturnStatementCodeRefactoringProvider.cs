// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ReturnStatementCodeRefactoringProvider))]
    public class ReturnStatementCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            ReturnStatementSyntax returnStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ReturnStatementSyntax>();

            if (returnStatement == null)
                return;

            if (returnStatement.Expression != null
                && returnStatement.Expression.Span.Contains(context.Span)
                && context.Document.SupportsSemanticModel)
            {
                MemberDeclarationSyntax declaration = GetDeclaration(returnStatement);

                if (declaration != null)
                {
                    TypeSyntax memberType = GetReturnType(declaration);

                    if (memberType != null)
                    {
                        SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                        ITypeSymbol memberTypeSymbol = semanticModel
                            .GetTypeInfo(memberType, context.CancellationToken)
                            .Type;

                        if (memberTypeSymbol != null)
                        {
                            ITypeSymbol typeSymbol = semanticModel
                                .GetTypeInfo(returnStatement.Expression, context.CancellationToken)
                                .Type;

                            if (typeSymbol != null)
                            {
                                if (memberTypeSymbol.SpecialType == SpecialType.System_Boolean
                                    && typeSymbol.IsKind(SymbolKind.NamedType))
                                {
                                    var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                                    if (namedTypeSymbol?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                                        && namedTypeSymbol.TypeArguments[0].SpecialType == SpecialType.System_Boolean)
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Add boolean comparison",
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
                                }
                            }
                        }
                    }
                }
            }
        }

        private static async Task<Document> ChangeReturnTypeAsync(
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

        private static TypeSyntax GetReturnType(MemberDeclarationSyntax declaration)
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

        private static string GetText(MemberDeclarationSyntax declaration)
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

        private static MemberDeclarationSyntax GetDeclaration(ReturnStatementSyntax returnStatement)
        {
            if (returnStatement.Parent?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)returnStatement.Parent;

                SyntaxNode node = block.Parent;

                if (block.Parent?.IsKind(SyntaxKind.GetAccessorDeclaration) == true
                    && block.Parent.Parent?.IsKind(SyntaxKind.AccessorList) == true)
                {
                    node = block.Parent.Parent.Parent;
                }

                switch (node?.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                        return (MemberDeclarationSyntax)node;
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
