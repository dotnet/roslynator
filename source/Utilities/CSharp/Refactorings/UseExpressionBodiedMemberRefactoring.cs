// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class UseExpressionBodiedMemberRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newDeclaration = GetNewDeclaration(declaration)
                .WithTrailingTrivia(declaration.GetTrailingTrivia())
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(declaration, newDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static MemberDeclarationSyntax GetNewDeclaration(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return GetNewDeclaration((MethodDeclarationSyntax)declaration);
                case SyntaxKind.OperatorDeclaration:
                    return GetNewDeclaration((OperatorDeclarationSyntax)declaration);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return GetNewDeclaration((ConversionOperatorDeclarationSyntax)declaration);
                case SyntaxKind.PropertyDeclaration:
                    return GetNewDeclaration((PropertyDeclarationSyntax)declaration);
                case SyntaxKind.IndexerDeclaration:
                    return GetNewDeclaration((IndexerDeclarationSyntax)declaration);
                default:
                    return declaration;
            }
        }

        private static MemberDeclarationSyntax GetNewDeclaration(MethodDeclarationSyntax declaration)
        {
            return declaration
                .WithExpressionBody(ArrowExpressionClause(GetExpression(declaration.Body)))
                .WithBody(null)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static MemberDeclarationSyntax GetNewDeclaration(OperatorDeclarationSyntax declaration)
        {
            return declaration
                .WithExpressionBody(ArrowExpressionClause(GetExpression(declaration.Body)))
                .WithBody(null)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static MemberDeclarationSyntax GetNewDeclaration(ConversionOperatorDeclarationSyntax declaration)
        {
            return declaration
                .WithExpressionBody(ArrowExpressionClause(GetExpression(declaration.Body)))
                .WithBody(null)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static MemberDeclarationSyntax GetNewDeclaration(PropertyDeclarationSyntax declaration)
        {
            return declaration
                .WithExpressionBody(ArrowExpressionClause(GetExpression(declaration.AccessorList)))
                .WithAccessorList(null)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static MemberDeclarationSyntax GetNewDeclaration(IndexerDeclarationSyntax declaration)
        {
            return declaration
                .WithExpressionBody(ArrowExpressionClause(GetExpression(declaration.AccessorList)))
                .WithAccessorList(null)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static ExpressionSyntax GetExpression(BlockSyntax block)
        {
            var returnStatement = (ReturnStatementSyntax)block.Statements[0];

            return returnStatement.Expression;
        }

        private static ExpressionSyntax GetExpression(AccessorListSyntax accessorList)
        {
            var returnStatement = (ReturnStatementSyntax)accessorList.Accessors[0].Body.Statements[0];

            return returnStatement.Expression;
        }

        public static bool CanRefactor(MethodDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && !declaration.ReturnsVoid()
                && CanRefactor(declaration.Body);
        }

        public static bool CanRefactor(OperatorDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && CanRefactor(declaration.Body);
        }

        public static bool CanRefactor(ConversionOperatorDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && CanRefactor(declaration.Body);
        }

        public static bool CanRefactor(PropertyDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && CanRefactor(declaration.AccessorList);
        }

        public static bool CanRefactor(IndexerDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return declaration.ExpressionBody == null
                && CanRefactor(declaration.AccessorList);
        }

        private static bool CanRefactor(AccessorListSyntax accessorList)
        {
            if (accessorList?.Accessors.Count == 1)
            {
                AccessorDeclarationSyntax accessor = accessorList.Accessors[0];

                return accessor.IsKind(SyntaxKind.GetAccessorDeclaration)
                    && CanRefactor(accessor.Body);
            }

            return false;
        }

        private static bool CanRefactor(BlockSyntax block)
        {
            if (block?.Statements.Count > 0)
            {
                StatementSyntax statement = block.Statements[0];

                if (statement.IsKind(SyntaxKind.ReturnStatement))
                {
                    var returnStatement = (ReturnStatementSyntax)statement;

                    if (returnStatement?.Expression != null)
                        return true;
                }
            }

            return false;
        }
    }
}
