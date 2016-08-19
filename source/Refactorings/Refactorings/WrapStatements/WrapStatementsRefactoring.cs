// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings.WrapStatements
{
    internal abstract class WrapStatementsRefactoring<TStatement> where TStatement : StatementSyntax
    {
        public abstract TStatement CreateStatement(ImmutableArray<StatementSyntax> statements);

        public async Task<Document> RefactorAsync(
            Document document,
            BlockSpan blockSpan,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            StatementSyntax[] statements = blockSpan.SelectedStatements().ToArray();

            int index = blockSpan.FirstSelectedStatementIndex;

            SyntaxTriviaList leadingTrivia = statements[0].GetLeadingTrivia();
            SyntaxTriviaList trailingTrivia = statements[statements.Length - 1].GetTrailingTrivia();

            statements[0] = statements[0].WithLeadingTrivia();
            statements[statements.Length - 1] = statements[statements.Length - 1].WithTrailingTrivia();

            SyntaxList<StatementSyntax> newStatements = blockSpan.Statements;

            int cnt = statements.Length;

            while (cnt > 0)
            {
                newStatements = newStatements.RemoveAt(index);
                cnt--;
            }

            TStatement statement = CreateStatement(statements.ToImmutableArray());

            statement = statement
                .WithLeadingTrivia(leadingTrivia)
                .WithTrailingTrivia(trailingTrivia)
                .WithFormatterAnnotation();

            newStatements = newStatements.Insert(index, statement);

            root = root.ReplaceNode(blockSpan.Block, blockSpan.Block.WithStatements(newStatements));

            return document.WithSyntaxRoot(root);
        }
    }
}
