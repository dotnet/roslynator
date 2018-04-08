// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidSemicolonAtEndOfDeclarationRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newMemberDeclaration = GetNewMemberDeclaration(memberDeclaration);

            return document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration, cancellationToken);
        }

        private static MemberDeclarationSyntax GetNewMemberDeclaration(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var declaration = (NamespaceDeclarationSyntax)memberDeclaration;
                        return declaration
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var declaration = (ClassDeclarationSyntax)memberDeclaration;
                        return declaration
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var declaration = (InterfaceDeclarationSyntax)memberDeclaration;
                        return declaration
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var declaration = (StructDeclarationSyntax)memberDeclaration;
                        return declaration
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.EnumDeclaration:
                    {
                        var declaration = (EnumDeclarationSyntax)memberDeclaration;
                        return declaration
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
            }

            return null;
        }

        private static SyntaxTriviaList GetNewTrailingTrivia(SyntaxToken closeBrace, SyntaxToken semicolon)
        {
            if (closeBrace.TrailingTrivia.IsEmptyOrWhitespace()
                && semicolon.LeadingTrivia.IsEmptyOrWhitespace())
            {
                return semicolon.TrailingTrivia;
            }
            else
            {
                return SyntaxFactory.TriviaList(closeBrace.TrailingTrivia)
                    .AddRange(semicolon.LeadingTrivia)
                    .AddRange(semicolon.TrailingTrivia);
            }
        }
    }
}
