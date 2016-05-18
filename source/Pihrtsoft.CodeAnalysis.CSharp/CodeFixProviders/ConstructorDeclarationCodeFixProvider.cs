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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConstructorDeclarationCodeFixProvider))]
    [Shared]
    public class ConstructorDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.RemoveRedundantBaseConstructorCall);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            ConstructorDeclarationSyntax constructor = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ConstructorDeclarationSyntax>();

            if (constructor == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove base constructor call",
                cancellationToken => RemoveBaseConstructorCallAsync(context.Document, constructor, cancellationToken),
                DiagnosticIdentifiers.RemoveRedundantBaseConstructorCall);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveBaseConstructorCallAsync(
            Document document,
            ConstructorDeclarationSyntax constructor,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            if (constructor.ParameterList.GetTrailingTrivia().IsWhitespaceOrEndOfLine()
                && constructor.Initializer.GetLeadingTrivia().IsWhitespaceOrEndOfLine())
            {
                ConstructorDeclarationSyntax newConstructor = constructor
                    .WithParameterList(constructor.ParameterList.WithTrailingTrivia(constructor.Initializer.GetTrailingTrivia()))
                    .WithInitializer(null);

                root = root.ReplaceNode(constructor, newConstructor);
            }
            else
            {
                root = root.RemoveNode(constructor.Initializer, SyntaxRemoveOptions.KeepExteriorTrivia);
            }

            return document.WithSyntaxRoot(root);
        }
    }
}
