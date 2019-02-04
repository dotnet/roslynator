// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Documentation;
using Roslynator.CSharp.Refactorings;
using Roslynator.Documentation;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    internal static class DocumentRefactorings
    {
        public static Task<Document> ChangeTypeAsync(
           Document document,
           TypeSyntax type,
           ITypeSymbol typeSymbol,
           CancellationToken cancellationToken = default)
        {
            TypeSyntax newType = ChangeType(type, typeSymbol);

            return document.ReplaceNodeAsync(type, newType, cancellationToken);
        }

        private static TypeSyntax ChangeType(TypeSyntax type, ITypeSymbol typeSymbol)
        {
            TypeSyntax newType = typeSymbol
                .ToTypeSyntax()
                .WithTriviaFrom(type);

            if (newType is TupleTypeSyntax tupleType)
            {
                SeparatedSyntaxList<TupleElementSyntax> newElements = tupleType
                    .Elements
                    .Select(tupleElement => tupleElement.WithType(tupleElement.Type.WithSimplifierAnnotation()))
                    .ToSeparatedSyntaxList();

                return tupleType.WithElements(newElements);
            }
            else
            {
                return newType.WithSimplifierAnnotation();
            }
        }

        public static Task<Document> ChangeTypeToVarAsync(
            Document document,
            TypeSyntax type,
            CancellationToken cancellationToken = default)
        {
            IdentifierNameSyntax newType = VarType().WithTriviaFrom(type);

            return document.ReplaceNodeAsync(type, newType, cancellationToken);
        }

        public static Task<Document> ChangeTypeAndAddAwaitAsync(
            Document document,
            VariableDeclarationSyntax variableDeclaration,
            VariableDeclaratorSyntax variableDeclarator,
            SyntaxNode containingDeclaration,
            ITypeSymbol newTypeSymbol,
            CancellationToken cancellationToken)
        {
            TypeSyntax type = variableDeclaration.Type;

            ExpressionSyntax value = variableDeclarator.Initializer.Value;

            AwaitExpressionSyntax newValue = AwaitExpression(value.WithoutTrivia()).WithTriviaFrom(value);

            TypeSyntax newType = ChangeType(type, newTypeSymbol);

            VariableDeclarationSyntax newVariableDeclaration = variableDeclaration
                .ReplaceNode(value, newValue)
                .WithType(newType);

            if (!SyntaxInfo.ModifierListInfo(containingDeclaration).IsAsync)
            {
                SyntaxNode newDeclaration = containingDeclaration
                    .ReplaceNode(variableDeclaration, newVariableDeclaration)
                    .InsertModifier(SyntaxKind.AsyncKeyword);

                return document.ReplaceNodeAsync(containingDeclaration, newDeclaration, cancellationToken);
            }

            return document.ReplaceNodeAsync(variableDeclaration, newVariableDeclaration, cancellationToken);
        }

        public static Task<Document> AddCastExpressionAsync(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            CancellationToken cancellationToken = default)
        {
            TypeSyntax type = destinationType.ToTypeSyntax().WithSimplifierAnnotation();

            return AddCastExpressionAsync(document, expression, type, cancellationToken);
        }

        public static Task<Document> AddCastExpressionAsync(
            Document document,
            ExpressionSyntax expression,
            TypeSyntax destinationType,
            CancellationToken cancellationToken = default)
        {
            ExpressionSyntax newExpression = expression
                .WithoutTrivia()
                .Parenthesize();

            ExpressionSyntax newNode = CastExpression(destinationType, newExpression)
                .WithTriviaFrom(expression)
                .Parenthesize();

            return document.ReplaceNodeAsync(expression, newNode, cancellationToken);
        }

        public static Task<Document> RemoveAsyncAwaitAsync(
            Document document,
            SyntaxToken asyncKeyword,
            CancellationToken cancellationToken = default)
        {
            return RemoveAsyncAwait.RefactorAsync(document, asyncKeyword, cancellationToken);
        }

        public static Task<Document> SwapBinaryOperandsAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxToken token = binaryExpression.OperatorToken;

            ExpressionSyntax newBinaryExpressions = binaryExpression.Update(
                left: right.WithTriviaFrom(left),
                operatorToken: Token(token.LeadingTrivia, GetOperatorTokenKind(token.Kind()), token.TrailingTrivia),
                right: left.WithTriviaFrom(right));

            return document.ReplaceNodeAsync(binaryExpression, newBinaryExpressions, cancellationToken);

            SyntaxKind GetOperatorTokenKind(SyntaxKind kind)
            {
                switch (kind)
                {
                    case SyntaxKind.LessThanToken:
                        return SyntaxKind.GreaterThanToken;
                    case SyntaxKind.LessThanEqualsToken:
                        return SyntaxKind.GreaterThanEqualsToken;
                    case SyntaxKind.GreaterThanToken:
                        return SyntaxKind.LessThanToken;
                    case SyntaxKind.GreaterThanEqualsToken:
                        return SyntaxKind.LessThanEqualsToken;
                    default:
                        return kind;
                }
            }
        }

        public static async Task<Document> AddNewDocumentationCommentsAsync(
            Document document,
            DocumentationCommentGeneratorSettings settings = null,
            bool skipNamespaceDeclaration = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var rewriter = new AddNewDocumentationCommentRewriter(settings, skipNamespaceDeclaration);

            SyntaxNode newRoot = rewriter.Visit(root);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> AddBaseOrNewDocumentationCommentsAsync(
            Document document,
            SemanticModel semanticModel,
            DocumentationCommentGeneratorSettings settings = null,
            bool skipNamespaceDeclaration = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var rewriter = new AddBaseOrNewDocumentationCommentRewriter(semanticModel, settings, skipNamespaceDeclaration, cancellationToken);

            SyntaxNode newRoot = rewriter.Visit(root);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
