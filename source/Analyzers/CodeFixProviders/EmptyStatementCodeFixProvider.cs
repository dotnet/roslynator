// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyStatementCodeFixProvider))]
    [Shared]
    public class EmptyStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.RemoveEmptyStatement);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            EmptyStatementSyntax emptyStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<EmptyStatementSyntax>();

            if (emptyStatement == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove empty statement",
                cancellationToken => RemoveEmptyStatemntAsync(context.Document, emptyStatement, cancellationToken),
                DiagnosticIdentifiers.RemoveEmptyStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveEmptyStatemntAsync(
            Document document,
            EmptyStatementSyntax emptyStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = oldRoot.RemoveNode(emptyStatement, SyntaxRemoveOptions.KeepExteriorTrivia);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
