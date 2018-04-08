// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddEmptyLineAfterLastStatementInDoStatementRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTriviaList trailingTrivia = statement.GetTrailingTrivia();

            int index = trailingTrivia.IndexOf(SyntaxKind.EndOfLineTrivia);

            SyntaxTriviaList newTrailingTrivia = trailingTrivia.Insert(index, CSharpFactory.NewLine());

            StatementSyntax newStatement = statement.WithTrailingTrivia(newTrailingTrivia);

            return document.ReplaceNodeAsync(statement, newStatement, cancellationToken);
        }
    }
}
