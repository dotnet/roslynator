// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidSemicolonAtEndOfDeclarationRefactoring
    {
        public static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (NamespaceDeclarationSyntax)context.Node;

            SyntaxToken semicolon = declaration.SemicolonToken;

            if (!semicolon.IsKind(SyntaxKind.None)
                && !semicolon.IsMissing)
            {
                SyntaxToken closeBrace = declaration.CloseBraceToken;

                if (!closeBrace.IsKind(SyntaxKind.None)
                    && !closeBrace.IsMissing)
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.AvoidSemicolonAtEndOfDeclaration, semicolon);
                }
            }
        }

        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ClassDeclarationSyntax)context.Node;

            SyntaxToken semicolon = declaration.SemicolonToken;

            if (!semicolon.IsKind(SyntaxKind.None)
                && !semicolon.IsMissing)
            {
                SyntaxToken closeBrace = declaration.CloseBraceToken;

                if (!closeBrace.IsKind(SyntaxKind.None)
                    && !closeBrace.IsMissing)
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.AvoidSemicolonAtEndOfDeclaration, semicolon);
                }
            }
        }

        public static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (InterfaceDeclarationSyntax)context.Node;

            SyntaxToken semicolon = declaration.SemicolonToken;

            if (!semicolon.IsKind(SyntaxKind.None)
                && !semicolon.IsMissing)
            {
                SyntaxToken closeBrace = declaration.CloseBraceToken;

                if (!closeBrace.IsKind(SyntaxKind.None)
                    && !closeBrace.IsMissing)
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.AvoidSemicolonAtEndOfDeclaration, semicolon);
                }
            }
        }

        public static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (StructDeclarationSyntax)context.Node;

            SyntaxToken semicolon = declaration.SemicolonToken;

            if (!semicolon.IsKind(SyntaxKind.None)
                && !semicolon.IsMissing)
            {
                SyntaxToken closeBrace = declaration.CloseBraceToken;

                if (!closeBrace.IsKind(SyntaxKind.None)
                    && !closeBrace.IsMissing)
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.AvoidSemicolonAtEndOfDeclaration, semicolon);
                }
            }
        }

        public static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (EnumDeclarationSyntax)context.Node;

            SyntaxToken semicolon = declaration.SemicolonToken;

            if (!semicolon.IsKind(SyntaxKind.None)
                && !semicolon.IsMissing)
            {
                SyntaxToken closeBrace = declaration.CloseBraceToken;

                if (!closeBrace.IsKind(SyntaxKind.None)
                    && !closeBrace.IsMissing)
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.AvoidSemicolonAtEndOfDeclaration, semicolon);
                }
            }
        }

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
