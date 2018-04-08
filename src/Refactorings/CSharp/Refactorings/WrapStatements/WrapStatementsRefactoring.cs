// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.WrapStatements
{
    internal abstract class WrapStatementsRefactoring<TStatement> where TStatement : StatementSyntax
    {
        public abstract TStatement CreateStatement(ImmutableArray<StatementSyntax> statements);

        public Task<Document> RefactorAsync(
            Document document,
            StatementListSelection selectedStatements,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementSyntax[] statements = selectedStatements.ToArray();

            int index = selectedStatements.FirstIndex;

            SyntaxTriviaList leadingTrivia = statements[0].GetLeadingTrivia();
            SyntaxTriviaList trailingTrivia = statements[statements.Length - 1].GetTrailingTrivia();

            statements[0] = statements[0].WithLeadingTrivia();
            statements[statements.Length - 1] = statements[statements.Length - 1].WithTrailingTrivia();

            SyntaxList<StatementSyntax> newStatements = selectedStatements.UnderlyingList;

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

            return document.ReplaceStatementsAsync(SyntaxInfo.StatementListInfo(selectedStatements), newStatements, cancellationToken);
        }

        public Task<Document> RefactorAsync(
            Document document,
            StatementSyntax  embeddedStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            BlockSyntax newNode = Block(CreateStatement(ImmutableArray.Create(embeddedStatement)))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(embeddedStatement, newNode, cancellationToken);
        }
    }
}
