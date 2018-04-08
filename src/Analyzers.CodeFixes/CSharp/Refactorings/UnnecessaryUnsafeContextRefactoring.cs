// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UnnecessaryUnsafeContextRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            UnsafeStatementSyntax unsafeStatement,
            CancellationToken cancellationToken)
        {
            SyntaxToken keyword = unsafeStatement.UnsafeKeyword;

            BlockSyntax block = unsafeStatement.Block;

            IEnumerable<SyntaxTrivia> leadingTrivia = keyword.LeadingTrivia
                .AddRange(keyword.TrailingTrivia.EmptyIfWhitespace())
                .AddRange(block.GetLeadingTrivia().EmptyIfWhitespace());

            BlockSyntax newBlock = block.WithLeadingTrivia(leadingTrivia);

            return document.ReplaceNodeAsync(unsafeStatement, newBlock, cancellationToken);
        }
    }
}
