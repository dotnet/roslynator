// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes;

internal static class FinallyClauseCodeFixProvider
{
    internal static async Task<Document> RemoveEmptyFinallyClauseAsync(
        Document document,
        FinallyClauseSyntax finallyClause,
        CancellationToken cancellationToken)
    {
        var tryStatement = (TryStatementSyntax)finallyClause.Parent;

        SyntaxList<CatchClauseSyntax> catches = tryStatement.Catches;

        if (catches.Any())
        {
            if (finallyClause.GetLeadingTrivia().IsEmptyOrWhitespace())
            {
                CatchClauseSyntax lastCatch = catches.Last();

                if (lastCatch.GetTrailingTrivia().IsEmptyOrWhitespace())
                {
                    TryStatementSyntax newTryStatement = tryStatement
                        .WithCatches(catches.Replace(lastCatch, lastCatch.WithTrailingTrivia(finallyClause.GetTrailingTrivia())))
                        .WithFinally(null);

                    return await document.ReplaceNodeAsync(tryStatement, newTryStatement, cancellationToken).ConfigureAwait(false);
                }
            }

            return await document.RemoveNodeAsync(finallyClause, SyntaxRemoveOptions.KeepExteriorTrivia, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            IEnumerable<StatementSyntax> newNodes = tryStatement
                .Block
                .Statements
                .Select(f => f.WithFormatterAnnotation());

            return await document.ReplaceNodeAsync(tryStatement, newNodes, cancellationToken).ConfigureAwait(false);
        }
    }
}
