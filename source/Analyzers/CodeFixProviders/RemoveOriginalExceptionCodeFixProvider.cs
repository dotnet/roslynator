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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveOriginalExceptionCodeFixProvider))]
    [Shared]
    public class RemoveOriginalExceptionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.RemoveOriginalExceptionFromThrowStatement);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            ThrowStatementSyntax node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ThrowStatementSyntax>();

            if (node == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove original exception from throw statement",
                cancellationToken => RemoveOriginalExceptionAsync(context.Document, node, cancellationToken),
                DiagnosticIdentifiers.RemoveOriginalExceptionFromThrowStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveOriginalExceptionAsync(
            Document document,
            ThrowStatementSyntax throwStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ThrowStatementSyntax newThrowStatement = throwStatement
                .RemoveNode(throwStatement.Expression, SyntaxRemoveOptions.KeepExteriorTrivia)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(throwStatement, newThrowStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
