// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal abstract class WrapStatementsRefactoring<TStatement> where TStatement : StatementSyntax
    {
        public static bool CanRefactor(RefactoringContext context, BlockSyntax block)
        {
            return GetSelectedStatements(block, context.Span).Any();
        }

        public static IEnumerable<StatementSyntax> GetSelectedStatements(BlockSyntax block, TextSpan span)
        {
            return block.Statements
                .SkipWhile(f => span.Start > f.Span.Start)
                .TakeWhile(f => span.End >= f.Span.End);
        }

        public abstract TStatement CreateStatement(ImmutableArray<StatementSyntax> statements);

        public async Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            ImmutableArray<StatementSyntax> statements = GetSelectedStatements(block, span).ToImmutableArray();

            int index = block.Statements.IndexOf(statements[0]);

            SyntaxList<StatementSyntax> newStatements = block.Statements;

            int cnt = statements.Length;

            while (cnt > 0)
            {
                newStatements = newStatements.RemoveAt(index);
                cnt--;
            }

            TStatement tryStatement = CreateStatement(statements);

            tryStatement = tryStatement.WithFormatterAnnotation();

            newStatements = newStatements.Insert(index, tryStatement);

            root = root.ReplaceNode(block, block.WithStatements(newStatements));

            return document.WithSyntaxRoot(root);
        }
    }
}
