// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeElseClauseWithNestedIfStatementRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ElseClauseSyntax elseClause,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var block = (BlockSyntax)elseClause.Statement;

            var ifStatement = (IfStatementSyntax)block.Statements[0];

            ElseClauseSyntax newElseClause = elseClause
                .WithStatement(ifStatement)
                .WithElseKeyword(elseClause.ElseKeyword.WithoutTrailingTrivia())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(elseClause, newElseClause, cancellationToken);
        }
    }
}
