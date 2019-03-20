// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ReduceIfNesting
{
    internal static partial class ReduceIfNestingRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            SyntaxKind jumpKind,
            bool recursive,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(ifStatement);

            SyntaxNode node = statementsInfo.Parent;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var rewriter = new ReduceIfStatementRewriter(jumpKind, recursive, semanticModel, cancellationToken);

            SyntaxNode newNode = rewriter.Visit(node);

            return await document.ReplaceNodeAsync(node, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
