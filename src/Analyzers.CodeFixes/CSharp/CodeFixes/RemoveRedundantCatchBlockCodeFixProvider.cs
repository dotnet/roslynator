// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
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
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRedundantCatchBlockCodeFixProvider))]
[Shared]
public class RemoveRedundantCatchBlockCodeFixProvider : BaseCodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(DiagnosticRules.RemoveRedundantCatchBlock.Id);

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.Document.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out CatchClauseSyntax catchClause))
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Remove redundant catch block",
                createChangedDocument: c => RemoveRedundantCatchAsync(context.Document, catchClause, c),
                equivalenceKey: nameof(RemoveRedundantCatchBlockCodeFixProvider)),
            context.Diagnostics[0]);
    }

    private static async Task<Document> RemoveRedundantCatchAsync(Document document, CatchClauseSyntax catchClause, CancellationToken cancellationToken)
    {
        var tryStatement = (TryStatementSyntax)catchClause.Parent;

        SyntaxList<CatchClauseSyntax> catchClauses = tryStatement.Catches;
        SyntaxList<CatchClauseSyntax> newCatchClauses = SyntaxFactory.List(catchClauses.Take(catchClauses.Count - 1));

        if (!newCatchClauses.Any() && tryStatement.Finally is null)
        {
            IEnumerable<StatementSyntax> newNodes = tryStatement
                .Block
                .Statements
                .Select(f => f.WithFormatterAnnotation());

            return await document.ReplaceNodeAsync(tryStatement, newNodes, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            TryStatementSyntax newTryStatement = tryStatement.WithCatches(newCatchClauses);
            return await document.ReplaceNodeAsync(tryStatement, newTryStatement, cancellationToken).ConfigureAwait(false);
        }
    }
}
