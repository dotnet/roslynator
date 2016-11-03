// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FinallyClauseCodeFixProvider))]
    [Shared]
    public class FinallyClauseCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.RemoveEmptyFinallyClause);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            FinallyClauseSyntax finallyClause = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<FinallyClauseSyntax>();

            if (finallyClause == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove empty finally clause",
                cancellationToken => RemoveEmptyFinallyClauseAsync(context.Document, finallyClause, cancellationToken),
                DiagnosticIdentifiers.RemoveEmptyFinallyClause + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveEmptyFinallyClauseAsync(
            Document document,
            FinallyClauseSyntax finallyClause,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = GetNewRoot(oldRoot, finallyClause);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode GetNewRoot(SyntaxNode oldRoot, FinallyClauseSyntax finallyClause)
        {
            if (finallyClause.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                var tryStatement = (TryStatementSyntax)finallyClause.Parent;

                CatchClauseSyntax lastCatch = tryStatement.Catches[tryStatement.Catches.Count - 1];

                if (lastCatch.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    TryStatementSyntax newTryStatement = tryStatement
                        .WithCatches(tryStatement.Catches.Replace(lastCatch, lastCatch.WithTrailingTrivia(finallyClause.GetTrailingTrivia())))
                        .WithFinally(null);

                    return oldRoot.ReplaceNode(tryStatement, newTryStatement);
                }
            }

            return oldRoot.RemoveNode(finallyClause, SyntaxRemoveOptions.KeepExteriorTrivia);
        }
    }
}
