// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberDeclarationCodeFixProvider))]
    [Shared]
    public class MemberDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.FormatDeclarationBraces);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax declaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (declaration == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Format braces",
                cancellationToken => FormatBracesAsync(context.Document, declaration, cancellationToken),
                DiagnosticIdentifiers.FormatDeclarationBraces);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> FormatBracesAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            MemberDeclarationSyntax newNode = GetNewDeclaration(declaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(declaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static MemberDeclarationSyntax GetNewDeclaration(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)declaration;

                        return classDeclaration
                            .WithOpenBraceToken(classDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(classDeclaration.CloseBraceToken.WithLeadingTrivia(SyntaxHelper.NewLine));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)declaration;

                        return structDeclaration
                            .WithOpenBraceToken(structDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(structDeclaration.CloseBraceToken.WithLeadingTrivia(SyntaxHelper.NewLine));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)declaration;

                        return interfaceDeclaration
                            .WithOpenBraceToken(interfaceDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(interfaceDeclaration.CloseBraceToken.WithLeadingTrivia(SyntaxHelper.NewLine));
                    }
            }

            return declaration;
        }
    }
}
