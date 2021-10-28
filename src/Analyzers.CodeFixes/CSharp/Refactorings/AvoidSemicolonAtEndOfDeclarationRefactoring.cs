// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            switch (memberDeclaration)
            {
                case NamespaceDeclarationSyntax declaration:
                    {
                        return declaration
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case ClassDeclarationSyntax declaration:
                    {
                        return declaration
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case InterfaceDeclarationSyntax declaration:
                    {
                        return declaration
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case StructDeclarationSyntax declaration:
                    {
                        return declaration
                            .WithSemicolonToken(default(SyntaxToken))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case EnumDeclarationSyntax declaration:
                    {
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
