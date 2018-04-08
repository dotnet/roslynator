// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddEmptyLineAfterEmbeddedStatementRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            StatementSyntax newNode = statement
                .AppendToTrailingTrivia(CSharpFactory.NewLine())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(statement, newNode, cancellationToken);
        }
    }
}
