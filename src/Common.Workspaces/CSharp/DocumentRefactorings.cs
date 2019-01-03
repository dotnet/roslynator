// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;
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
            TypeSyntax newType = SyntaxRefactorings.ChangeType(type, typeSymbol);

            return document.ReplaceNodeAsync(type, newType, cancellationToken);
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
            VariableDeclarationSyntax newVariableDeclaration = SyntaxRefactorings.ChangeTypeAndAddAwait(variableDeclaration, variableDeclarator, newTypeSymbol);

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
    }
}
