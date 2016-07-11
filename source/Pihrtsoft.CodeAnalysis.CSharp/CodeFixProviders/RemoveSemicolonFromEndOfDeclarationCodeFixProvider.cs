// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveSemicolonFromEndOfDeclarationCodeFixProvider))]
    [Shared]
    public class RemoveSemicolonFromEndOfDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.AvoidSemicolonAtEndOfDeclaration);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SyntaxNode declaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (declaration == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove unnecessary semicolon",
                cancellationToken => RemoveSemicolonAsync(context.Document, declaration, cancellationToken),
                DiagnosticIdentifiers.AvoidSemicolonAtEndOfDeclaration + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveSemicolonAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newNode = GetNewNode(node);

            SyntaxNode newRoot = oldRoot.ReplaceNode(node, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode GetNewNode(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var declaration = (NamespaceDeclarationSyntax)node;
                        return declaration
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var declaration = (ClassDeclarationSyntax)node;
                        return declaration
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var declaration = (InterfaceDeclarationSyntax)node;
                        return declaration
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var declaration = (StructDeclarationSyntax)node;
                        return declaration
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None))
                            .WithCloseBraceToken(declaration.CloseBraceToken
                                .WithTrailingTrivia(GetNewTrailingTrivia(declaration.CloseBraceToken, declaration.SemicolonToken)));
                    }
                case SyntaxKind.EnumDeclaration:
                    {
                        var declaration = (EnumDeclarationSyntax)node;
                        return declaration
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None))
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
                SyntaxTriviaList list = SyntaxFactory.TriviaList(closeBrace.TrailingTrivia);
                list = list.AddRange(semicolon.LeadingTrivia);
                list = list.AddRange(semicolon.TrailingTrivia);

                return list;
            }
        }
    }
}
