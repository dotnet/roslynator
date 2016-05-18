// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NamespaceDeclarationCodeFixProvider))]
    [Shared]
    public class NamespaceDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.RemoveEmptyNamespaceDeclaration);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            NamespaceDeclarationSyntax declaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<NamespaceDeclarationSyntax>();

            if (declaration == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove empty namespace declaration",
                cancellationToken => RemoveEmptyNamespaceDeclarationAsync(context.Document, declaration, cancellationToken),
                DiagnosticIdentifiers.RemoveEmptyNamespaceDeclaration + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveEmptyNamespaceDeclarationAsync(
            Document document,
            NamespaceDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newRoot = oldRoot.RemoveNode(declaration, GetRemoveOptions(declaration));

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxRemoveOptions GetRemoveOptions(NamespaceDeclarationSyntax declaration)
        {
            if (declaration.GetLeadingTrivia().IsWhitespaceOrEndOfLine())
            {
                if (declaration.GetTrailingTrivia().IsWhitespaceOrEndOfLine())
                {
                    return SyntaxRemoveOptions.KeepNoTrivia;
                }
                else
                {
                    return SyntaxRemoveOptions.KeepTrailingTrivia;
                }
            }
            else if (declaration.GetTrailingTrivia().IsWhitespaceOrEndOfLine())
            {
                return SyntaxRemoveOptions.KeepLeadingTrivia;
            }
            else
            {
                return SyntaxRemoveOptions.KeepExteriorTrivia;
            }
        }
    }
}
