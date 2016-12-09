// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidSemicolonAtEndOfDeclarationRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            SyntaxToken semicolon = GetSemicolon(node);

            if (!semicolon.IsKind(SyntaxKind.None) && !semicolon.IsMissing)
            {
                SyntaxToken closeBrace = GetCloseBrace(node);

                if (!closeBrace.IsKind(SyntaxKind.None) && !closeBrace.IsMissing)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AvoidSemicolonAtEndOfDeclaration,
                        semicolon.GetLocation());
                }
            }
        }

        private static SyntaxToken GetSemicolon(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)node).SemicolonToken;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).SemicolonToken;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).SemicolonToken;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).SemicolonToken;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).SemicolonToken;
            }

            return default(SyntaxToken);
        }

        private static SyntaxToken GetCloseBrace(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)node).CloseBraceToken;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).CloseBraceToken;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).CloseBraceToken;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).CloseBraceToken;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).CloseBraceToken;
            }

            return default(SyntaxToken);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newMemberDeclaration = GetNewMemberDeclaration(memberDeclaration);

            return await document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration).ConfigureAwait(false);
        }

        private static MemberDeclarationSyntax GetNewMemberDeclaration(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var declaration = (NamespaceDeclarationSyntax)memberDeclaration;
                        return declaration
                            .WithSemicolonToken(NoneToken())
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var declaration = (ClassDeclarationSyntax)memberDeclaration;
                        return declaration
                            .WithSemicolonToken(NoneToken())
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var declaration = (InterfaceDeclarationSyntax)memberDeclaration;
                        return declaration
                            .WithSemicolonToken(NoneToken())
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var declaration = (StructDeclarationSyntax)memberDeclaration;
                        return declaration
                            .WithSemicolonToken(NoneToken())
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.EnumDeclaration:
                    {
                        var declaration = (EnumDeclarationSyntax)memberDeclaration;
                        return declaration
                            .WithSemicolonToken(NoneToken())
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
            }

            return null;
        }

        private static SyntaxTriviaList GetNewTrailingTrivia(SyntaxToken closeBrace, SyntaxToken semicolon)
        {
            if (closeBrace.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && semicolon.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
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
