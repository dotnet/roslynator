// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveBracesRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            StatementSyntax statement = block
                .Statements[0]
                .TrimTrivia()
                .PrependToLeadingTrivia(block.GetLeadingTrivia())
                .AppendToTrailingTrivia(block.GetTrailingTrivia())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, statement, cancellationToken);
        }
    }
}
