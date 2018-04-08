// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpandExpressionBodyRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ArrowExpressionClauseSyntax arrowExpressionClause,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newNode = Refactor(arrowExpressionClause, semanticModel, cancellationToken).WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(arrowExpressionClause.Parent, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static SyntaxNode Refactor(
            ArrowExpressionClauseSyntax arrowExpressionClause,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SyntaxNode node = arrowExpressionClause.Parent;

            ExpressionSyntax expression = arrowExpressionClause.Expression;

            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var method = (MethodDeclarationSyntax)node;

                        return method
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(method.ReturnType, expression, method.SemicolonToken, semanticModel, cancellationToken));
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructor = (ConstructorDeclarationSyntax)node;

                        return constructor
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(Block(ExpressionStatement(expression, constructor.SemicolonToken)));
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var destructor = (DestructorDeclarationSyntax)node;

                        return destructor
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(Block(ExpressionStatement(expression, destructor.SemicolonToken)));
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)node;

                        return operatorDeclaration
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression, operatorDeclaration.SemicolonToken));
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)node;

                        return conversionOperatorDeclaration
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression, conversionOperatorDeclaration.SemicolonToken));
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;

                        return propertyDeclaration
                            .WithAccessorList(CreateAccessorList(expression, propertyDeclaration.SemicolonToken))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;

                        return indexerDeclaration
                            .WithAccessorList(CreateAccessorList(expression, indexerDeclaration.SemicolonToken))
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken));
                    }
                case SyntaxKind.GetAccessorDeclaration:
                    {
                        var accessor = (AccessorDeclarationSyntax)node;

                        return accessor
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(expression, accessor.SemicolonToken));
                    }
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    {
                        var accessor = (AccessorDeclarationSyntax)node;

                        return accessor
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(Block(ExpressionStatement(expression, accessor.SemicolonToken)));
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunction = (LocalFunctionStatementSyntax)node;

                        return localFunction
                            .WithExpressionBody(null)
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithBody(CreateBlock(localFunction.ReturnType, expression, localFunction.SemicolonToken, semanticModel, cancellationToken));
                    }
                default:
                    {
                        Debug.Fail(node.Kind().ToString());
                        return node;
                    }
            }
        }

        private static BlockSyntax CreateBlock(ExpressionSyntax expression, SyntaxToken semicolon)
        {
            if (expression.Kind() == SyntaxKind.ThrowExpression)
            {
                return CreateBlockWithExpressionStatement(expression, semicolon);
            }
            else
            {
                return CreateBlockWithReturnStatement(expression, semicolon);
            }
        }

        private static BlockSyntax CreateBlock(
            TypeSyntax returnType,
            ExpressionSyntax expression,
            SyntaxToken semicolon,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (ShouldCreateExpressionStatement(returnType, expression, semanticModel, cancellationToken))
            {
                return CreateBlockWithExpressionStatement(expression, semicolon);
            }
            else
            {
                return CreateBlockWithReturnStatement(expression, semicolon);
            }
        }

        private static bool ShouldCreateExpressionStatement(
            TypeSyntax returnType,
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (returnType == null)
                return true;

            if (returnType.IsVoid())
                return true;

            SyntaxKind kind = expression.Kind();

            if (kind == SyntaxKind.ThrowExpression)
                return true;

            if (kind == SyntaxKind.AwaitExpression)
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(returnType, cancellationToken);

                if (typeSymbol?.Kind == SymbolKind.NamedType
                    && !((INamedTypeSymbol)typeSymbol).ConstructedFrom.EqualsOrInheritsFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T)))
                {
                    return true;
                }
            }

            return false;
        }

        private static AccessorListSyntax CreateAccessorList(ExpressionSyntax expression, SyntaxToken semicolon)
        {
            BlockSyntax block = CreateBlock(expression, semicolon);

            AccessorListSyntax accessorList = AccessorList(GetAccessorDeclaration(block));

            if (expression.IsSingleLine())
            {
                accessorList = accessorList
                    .RemoveWhitespace()
                    .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(NewLine()));
            }

            return accessorList;
        }

        private static BlockSyntax CreateBlockWithExpressionStatement(ExpressionSyntax expression, SyntaxToken semicolon)
        {
            return Block(ExpressionStatement(expression, semicolon));
        }

        private static BlockSyntax CreateBlockWithReturnStatement(ExpressionSyntax expression, SyntaxToken semicolon)
        {
            ReturnStatementSyntax returnStatement = ReturnStatement(
                ReturnKeyword().WithLeadingTrivia(expression.GetLeadingTrivia()),
                expression.WithoutLeadingTrivia(),
                semicolon);

            return Block(returnStatement);
        }
    }
}
