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
                    TypeSyntax type = GetReturnType(declaration);

                    if (type != null)
                    {
                        SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                        ITypeSymbol typeSymbol = semanticModel
                            .GetTypeInfo(type, context.CancellationToken)
                            .Type;

                        if (typeSymbol != null)
                        {
                            ITypeSymbol typeSymbol2 = semanticModel
                                .GetTypeInfo(returnStatement.Expression, context.CancellationToken)
                                .Type;

                            if (typeSymbol2 != null
                                && !typeSymbol.Equals(typeSymbol2))
                            {
                                TypeSyntax newType = TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol2);

                                if (newType != null)
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        $"Change {GetText(declaration)} type to '{typeSymbol2.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                                        cancellationToken =>
                                        {
                                            return ChangeReturnTypeAsync(
                                                context.Document,
                                                type,
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
