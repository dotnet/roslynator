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

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberDeclarationCodeFixProvider))]
    [Shared]
    public class MemberDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.FormatDeclarationBraces); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax memberDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (memberDeclaration == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Format braces",
                cancellationToken => RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                DiagnosticIdentifiers.FormatDeclarationBraces + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax newNode = GetNewDeclaration(declaration)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(declaration, newNode);

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
                            .WithCloseBraceToken(classDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLineTrivia()));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)declaration;

                        return structDeclaration
                            .WithOpenBraceToken(structDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(structDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLineTrivia()));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)declaration;

                        return interfaceDeclaration
                            .WithOpenBraceToken(interfaceDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(interfaceDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLineTrivia()));
                    }
            }

            return declaration;
        }
    }
}
