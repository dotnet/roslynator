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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReplaceBlockWithEmbeddedStatementCodeFixProvider))]
    [Shared]
    public class ReplaceBlockWithEmbeddedStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.ReplaceBlockWithEmbeddedStatement);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            BlockSyntax block = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<BlockSyntax>();

            if (block == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Replace block with embedded statement",
                cancellationToken => ReplaceBlockWithEmbeddedStatementAsync(context.Document, block, cancellationToken),
                DiagnosticIdentifiers.ReplaceBlockWithEmbeddedStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> ReplaceBlockWithEmbeddedStatementAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            StatementSyntax statement = block.Statements[0].TrimLeadingWhitespace();

            statement = statement.WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(block, statement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}