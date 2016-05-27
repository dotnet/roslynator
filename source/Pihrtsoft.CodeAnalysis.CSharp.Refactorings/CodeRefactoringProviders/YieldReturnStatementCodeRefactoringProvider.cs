// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(YieldReturnStatementCodeRefactoringProvider))]
    public class YieldReturnStatementCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            YieldStatementSyntax yieldStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<YieldStatementSyntax>();

            if (yieldStatement == null)
                return;

            if (yieldStatement.IsYieldReturn()
                && yieldStatement.Expression?.Span.Contains(context.Span) == true
                && context.Document.SupportsSemanticModel)
            {
                MemberDeclarationSyntax declaration = GetDeclaration(yieldStatement);

                if (declaration != null)
                {
                    TypeSyntax memberType = GetMemberType(declaration);

                    if (memberType != null)
                    {
                        SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                        ITypeSymbol memberTypeSymbol = semanticModel
                            .GetTypeInfo(memberType, context.CancellationToken)
                            .Type;

                        if (memberTypeSymbol.SpecialType != SpecialType.System_Collections_IEnumerable)
                        {
                            ITypeSymbol typeSymbol = semanticModel
                                .GetTypeInfo(yieldStatement.Expression, context.CancellationToken)
                                .Type;

                            if (typeSymbol?.IsKind(SymbolKind.ErrorType) == false)
                            {
                                if (memberTypeSymbol == null
                                    || memberTypeSymbol.IsKind(SymbolKind.ErrorType)
                                    || !memberTypeSymbol.IsGenericIEnumerable()
                                    || !((INamedTypeSymbol)memberTypeSymbol).TypeArguments[0].Equals(typeSymbol))
                                {
                                    TypeSyntax newType = QualifiedName(
                                        ParseName("System.Collections.Generic"),
                                        GenericName(
                                            Identifier("IEnumerable"),
                                            TypeArgumentList(
                                                SingletonSeparatedList(
                                                    TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol)))));

                                    context.RegisterRefactoring(
                                        $"Change {GetText(declaration)} type to 'IEnumerable<{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}>'",
                                        cancellationToken =>
                                        {
                                            return ChangeReturnTypeAsync(
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

        private static TypeSyntax GetMemberType(MemberDeclarationSyntax declaration)
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

        private static MemberDeclarationSyntax GetDeclaration(YieldStatementSyntax yieldStatement)
        {
            if (yieldStatement.Parent?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)yieldStatement.Parent;

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
